using UnityEngine;
using CoreBreach.Pooling;

namespace CoreBreach.Interfaces
{
    public interface IWeaponStrategy
    {
        string DisplayName { get; }
        float Cooldown { get; }
        void Fire(Transform muzzle, ProjectilePool pool, int ownerLayer, float damageMultiplier);
    }
}
