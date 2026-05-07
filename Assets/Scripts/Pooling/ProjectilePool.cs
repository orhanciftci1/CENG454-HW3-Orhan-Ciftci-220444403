using System.Collections.Generic;
using CoreBreach.Interfaces;
using UnityEngine;

namespace CoreBreach.Pooling
{
    public sealed class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private int initialSize = 24;

        private readonly Queue<Projectile> available = new();

        public int AvailableCount => available.Count;

        private void Awake()
        {
            for (int i = 0; i < initialSize; i++)
            {
                available.Enqueue(CreateProjectile());
            }
        }

        public Projectile Take()
        {
            Projectile projectile = available.Count > 0 ? available.Dequeue() : CreateProjectile();
            projectile.gameObject.SetActive(true);
            projectile.OnTakenFromPool();
            return projectile;
        }

        public void Return(Projectile projectile)
        {
            if (projectile == null)
            {
                return;
            }

            projectile.OnReturnedToPool();
            projectile.gameObject.SetActive(false);
            available.Enqueue(projectile);
        }

        private Projectile CreateProjectile()
        {
            Projectile projectile = Instantiate(projectilePrefab, transform);
            projectile.BindPool(this);
            projectile.gameObject.SetActive(false);
            return projectile;
        }
    }
}
