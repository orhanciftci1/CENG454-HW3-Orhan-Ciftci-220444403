using System;
using CoreBreach.Interfaces;
using UnityEngine;

namespace CoreBreach.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class EnemyUnit : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 30f;
        [SerializeField] private float moveSpeed = 2.1f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private float attackCooldown = 0.8f;
        [SerializeField] private int scoreValue = 100;
        [SerializeField] private MonoBehaviour movementStrategyComponent;

        public event Action<EnemyUnit> OnEnemyKilled;

        private Rigidbody2D body;
        private IEnemyMovementStrategy movementStrategy;
        private IDamageable targetDamageable;
        private float currentHealth;
        private float attackTimer;

        public bool IsAlive => currentHealth > 0f;
        public Transform Target { get; private set; }
        public float MoveSpeed => moveSpeed;
        public int ScoreValue => scoreValue;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            currentHealth = maxHealth;
            ResolveStrategy();
        }

        private void Update()
        {
            if (!IsAlive || Target == null)
            {
                return;
            }

            attackTimer -= Time.deltaTime;
            movementStrategy?.Move(this, Time.deltaTime);
        }

        public void Configure(Transform target)
        {
            Target = target;
            targetDamageable = target != null ? target.GetComponent<IDamageable>() : null;
            currentHealth = maxHealth;
            attackTimer = 0f;
            ResolveStrategy();
        }

        public void UseMovementStrategy(MonoBehaviour strategyComponent)
        {
            movementStrategyComponent = strategyComponent;
            ResolveStrategy();
        }

        public void MoveBy(Vector2 displacement)
        {
            body.MovePosition(body.position + displacement);
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive)
            {
                return;
            }

            currentHealth = Mathf.Max(0f, currentHealth - amount);
            if (currentHealth <= 0f)
            {
                OnEnemyKilled?.Invoke(this);
                Destroy(gameObject);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (targetDamageable == null || !targetDamageable.IsAlive || attackTimer > 0f)
            {
                return;
            }

            if (collision.transform == Target)
            {
                targetDamageable.TakeDamage(contactDamage);
                attackTimer = attackCooldown;
            }
        }

        private void ResolveStrategy()
        {
            movementStrategy = movementStrategyComponent as IEnemyMovementStrategy;
            if (movementStrategy == null)
            {
                movementStrategy = GetComponent<IEnemyMovementStrategy>();
            }
        }
    }
}
