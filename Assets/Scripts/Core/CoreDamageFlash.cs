using UnityEngine;

namespace CoreBreach.Core
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class CoreDamageFlash : MonoBehaviour
    {
        [SerializeField] private CoreHealth coreHealth;
        [SerializeField] private Color safeColor = new(0.25f, 0.95f, 1f);
        [SerializeField] private Color dangerColor = new(1f, 0.22f, 0.12f);

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (coreHealth == null)
            {
                coreHealth = GetComponent<CoreHealth>();
            }
        }

        private void OnEnable()
        {
            if (coreHealth != null)
            {
                coreHealth.OnHealthChanged += HandleHealthChanged;
            }
        }

        private void OnDisable()
        {
            if (coreHealth != null)
            {
                coreHealth.OnHealthChanged -= HandleHealthChanged;
            }
        }

        private void HandleHealthChanged(float current, float max)
        {
            float danger = 1f - Mathf.Clamp01(current / max);
            spriteRenderer.color = Color.Lerp(safeColor, dangerColor, danger);
        }
    }
}
