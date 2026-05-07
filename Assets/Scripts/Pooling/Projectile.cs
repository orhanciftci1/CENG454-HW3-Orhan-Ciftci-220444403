using CoreBreach.Core;
using CoreBreach.Interfaces;
using UnityEngine;

namespace CoreBreach.Pooling
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private float speed = 11f;
        [SerializeField] private float lifetime = 1.8f;
        [SerializeField] private LayerMask damageMask = ~0;

        private ProjectilePool pool;
        private Rigidbody2D body;
        private float remainingLifetime;
        private float damage;
        private int ownerLayer;
        private bool isReturning;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void BindPool(ProjectilePool projectilePool)
        {
            pool = projectilePool;
        }

        public void Launch(Vector2 position, Vector2 direction, float damageAmount, int sourceLayer)
        {
            transform.position = position;
            transform.right = direction.normalized;
            damage = damageAmount;
            ownerLayer = sourceLayer;
            body.linearVelocity = direction.normalized * speed;
        }

        private void Update()
        {
            remainingLifetime -= Time.deltaTime;
            if (remainingLifetime <= 0f)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (ownerLayer > 0 && other.gameObject.layer == ownerLayer)
            {
                return;
            }

            if ((damageMask.value & (1 << other.gameObject.layer)) == 0)
            {
                return;
            }

            if (other.TryGetComponent(out CoreHealth _))
            {
                return;
            }

            if (other.TryGetComponent(out IDamageable damageable) && damageable.IsAlive)
            {
                damageable.TakeDamage(damage);
                ReturnToPool();
            }
        }

        public void OnTakenFromPool()
        {
            isReturning = false;
            remainingLifetime = lifetime;
            damage = 0f;
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }

        public void OnReturnedToPool()
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
            damage = 0f;
            ownerLayer = -1;
        }

        private void ReturnToPool()
        {
            if (isReturning)
            {
                return;
            }

            isReturning = true;
            pool.Return(this);
        }
    }
}
