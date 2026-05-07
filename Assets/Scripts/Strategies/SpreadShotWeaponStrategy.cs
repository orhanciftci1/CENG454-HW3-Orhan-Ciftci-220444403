using CoreBreach.Pooling;
using UnityEngine;

namespace CoreBreach.Strategies
{
    [CreateAssetMenu(menuName = "Core Breach/Weapons/Spread Shot")]
    public sealed class SpreadShotWeaponStrategy : WeaponStrategyBase
    {
        [SerializeField] private float spreadAngle = 18f;

        public override void Fire(Transform muzzle, ProjectilePool pool, int ownerLayer, float damageMultiplier)
        {
            FireAtAngle(muzzle, pool, ownerLayer, damageMultiplier, -spreadAngle);
            FireAtAngle(muzzle, pool, ownerLayer, damageMultiplier, 0f);
            FireAtAngle(muzzle, pool, ownerLayer, damageMultiplier, spreadAngle);
        }

        private void FireAtAngle(Transform muzzle, ProjectilePool pool, int ownerLayer, float damageMultiplier, float angle)
        {
            Projectile projectile = pool.Take();
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * muzzle.right;
            projectile.Launch(muzzle.position, direction, BaseDamage * damageMultiplier, ownerLayer);
        }
    }
}
