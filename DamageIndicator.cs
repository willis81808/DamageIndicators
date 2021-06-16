using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DamageIndicators
{

    [RequireComponent(typeof(CanvasGroup))]
    public class DamageIndicator : MonoBehaviour
    {
        private static Dictionary<Transform, float> timeSinceMap = new Dictionary<Transform, float>();

        private CanvasGroup canvasGroup;
        private TextMeshProUGUI textComponent;

        private Transform target;
        private string text;
        private bool healing = false;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            textComponent = new GameObject("Damage Text").AddComponent<TextMeshProUGUI>();
        }

        void Start()
        {
            textComponent.transform.SetParent(transform);
            textComponent.transform.position = transform.position;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontSize = 0.75f;
            textComponent.color = healing ? Color.green : Color.red;
            textComponent.text = text;

            StartCoroutine(FadeOut());
        }

        void Update()
        {
            transform.position = target.transform.position + Vector3.up;
        }

        IEnumerator FadeOut()
        {
            var startTime = Time.time;
            var duration = 1f;

            while (Time.time - startTime <= duration)
            {
                var progress = (Time.time - startTime) / duration;
                canvasGroup.alpha = 1 - progress;
                textComponent.transform.position += Vector3.up * Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 0;
            Destroy(gameObject);
        }

        public DamageIndicator SetTarget(Transform target)
        {
            this.target = target;
            return this;
        }

        public DamageIndicator SetText(string text)
        {
            this.text = text;
            return this;
        }

        public DamageIndicator SetIsHealing(bool healing)
        {
            this.healing = healing;
            return this;
        }
    }
}
