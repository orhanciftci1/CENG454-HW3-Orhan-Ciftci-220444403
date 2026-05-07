using System;
using CoreBreach.Interfaces;
using UnityEngine;

namespace CoreBreach.Core
{
    public sealed class CoreHealth : MonoBehaviour, IDamageable, ITargetProvider
    {
        [SerializeField] private float maxHealth = 100f;

        public event Action<float, float> OnHealthChanged;
        public event Action OnCoreDestroyed;

        private float currentHealth;

        public bool IsAlive => currentHealth > 0f;
        public Transform Target => transform;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        private void Start()
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive)
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0f)
            {
                OnCoreDestroyed?.Invoke();
            }
        }
    }
}
