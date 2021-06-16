using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnboundLib;

namespace DamageIndicators
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, "0.0.1")]
    [BepInProcess("Rounds.exe")]
    public class EntryPoint : BaseUnityPlugin
    {
        private const string ModId = "com.willis.rounds.damageindicators";
        private const string ModName = "Damage Indicators";

        private static bool ModActive = true;
        private static Canvas _canvas;
        private static Canvas canvas
        {
            get
            {
                if (_canvas == null)
                {
                    _canvas = new GameObject("Damage Indicator Canvas").AddComponent<Canvas>();
                    _canvas.renderMode = RenderMode.WorldSpace;
                    _canvas.pixelPerfect = false;
                    DontDestroyOnLoad(_canvas);
                }
                return _canvas;
            }
        }

        void Awake()
        {
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        void Start()
        {
            Unbound.RegisterGUI(ModName, DrawGUI);
        }

        void DrawGUI()
        {
            ModActive = GUILayout.Toggle(ModActive, "Enabled");
        }

        public static void SpawnDamageIndicator(Player target, float damage, bool healing = false)
        {
            if (!ModActive) return;

            new GameObject("Damage Indicator")
                .AddComponent<DamageIndicator>()
                .SetText(damage.ToString("####"))
                .SetTarget(target.transform)
                .SetIsHealing(healing)
                .transform.SetParent(canvas.transform);
        }
    }
    
    [HarmonyPatch]
    class Patches
    {
        [HarmonyPatch(typeof(CharacterStatModifiers), "DealtDamage")]
        [HarmonyPostfix]
        static void DealtDamage_Postfix(Vector2 damage, bool selfDamage, Player damagedPlayer)
        {
            if (damagedPlayer == null) return;
            EntryPoint.SpawnDamageIndicator(damagedPlayer, damage.magnitude);
        }

        [HarmonyPatch(typeof(HealthHandler), "Heal")]
        [HarmonyPostfix]
        static void Heal_Postfix(CharacterData ___data, float healAmount)
        {
            EntryPoint.SpawnDamageIndicator(___data.player, healAmount, true);
        }
    }
}
