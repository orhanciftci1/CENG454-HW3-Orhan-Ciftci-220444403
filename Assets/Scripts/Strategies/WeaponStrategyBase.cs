using CoreBreach.Interfaces;
using CoreBreach.Pooling;
using UnityEngine;

namespace CoreBreach.Strategies
{
    public abstract class WeaponStrategyBase : ScriptableObject, IWeaponStrategy
    {
        [SerializeField] private string displayName = "Weapon";
        [SerializeField] private float cooldown = 0.25f;
        [SerializeField] private float damage = 12f;

        public string DisplayName => displayName;
        public float Cooldown => cooldown;
        protected float BaseDamage => damage;

        public abstract void Fire(Transform muzzle, ProjectilePool pool, int ownerLayer, float damageMultiplier);
    }
}
