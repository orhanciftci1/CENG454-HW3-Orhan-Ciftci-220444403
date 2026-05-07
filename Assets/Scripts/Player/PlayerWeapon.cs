using CoreBreach.Pooling;
using CoreBreach.Strategies;
using UnityEngine;

namespace CoreBreach.Player
{
    public sealed class PlayerWeapon : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private WeaponStrategyBase primaryWeapon;
        [SerializeField] private WeaponStrategyBase alternateWeapon;
        [SerializeField] private WeaponStrategyBase boostedWeapon;

        private WeaponStrategyBase activeWeapon;
        private float nextFireTime;

        public string ActiveWeaponName => activeWeapon != null ? activeWeapon.DisplayName : "None";

        private void Awake()
        {
            activeWeapon = primaryWeapon;
            if (muzzle == null)
            {
                muzzle = transform;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                activeWeapon = primaryWeapon;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                activeWeapon = alternateWeapon;
            }
            else if (Input.GetKeyDown(KeyCode.Q) && boostedWeapon != null)
            {
                activeWeapon = boostedWeapon;
            }

            if (Input.GetMouseButton(0))
            {
                TryFire();
            }
        }

        private void TryFire()
        {
            if (activeWeapon == null || projectilePool == null || Time.time < nextFireTime)
            {
                return;
            }

            activeWeapon.Fire(muzzle, projectilePool, gameObject.layer, 1f);
            nextFireTime = Time.time + activeWeapon.Cooldown;
        }
    }
}
