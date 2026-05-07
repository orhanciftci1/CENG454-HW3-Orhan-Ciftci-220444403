using CoreBreach.Pooling;
using UnityEngine;

namespace CoreBreach.Strategies
{
    [CreateAssetMenu(menuName = "Core Breach/Weapons/Single Shot")]
    public sealed class SingleShotWeaponStrategy : WeaponStrategyBase
    {
        public override void Fire(Transform muzzle, ProjectilePool pool, int ownerLayer, float damageMultiplier)
        {
            Projectile projectile = pool.Take();
            projectile.Launch(muzzle.position, muzzle.right, BaseDamage * damageMultiplier, ownerLayer);
        }
    }
}
