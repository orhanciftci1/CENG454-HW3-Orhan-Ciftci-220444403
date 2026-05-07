using CoreBreach.Pooling;
using UnityEngine;

namespace CoreBreach.Strategies
{
    [CreateAssetMenu(menuName = "Core Breach/Weapons/Damage Boost Decorator")]
    public sealed class DamageBoostWeaponDecorator : WeaponStrategyBase
    {
        [SerializeField] private WeaponStrategyBase wrappedWeapon;
        [SerializeField] private float extraDamageMultiplier = 1.75f;

        public string WrappedWeaponName => wrappedWeapon != null ? wrappedWeapon.DisplayName : "None";

        public override void Fire(Transform muzzle, ProjectilePool pool, int ownerLayer, float damageMultiplier)
        {
            if (wrappedWeapon == null)
            {
                return;
            }

            wrappedWeapon.Fire(muzzle, pool, ownerLayer, damageMultiplier * extraDamageMultiplier);
        }
    }
}
