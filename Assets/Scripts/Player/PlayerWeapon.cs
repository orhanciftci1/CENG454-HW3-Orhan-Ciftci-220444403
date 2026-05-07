using CoreBreach.Pooling;
using CoreBreach.Strategies;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
            if (WasPressedThisFrame(KeyCode.Alpha1))
            {
                activeWeapon = primaryWeapon;
            }
            else if (WasPressedThisFrame(KeyCode.Alpha2))
            {
                activeWeapon = alternateWeapon;
            }
            else if (WasPressedThisFrame(KeyCode.Q) && boostedWeapon != null)
            {
                activeWeapon = boostedWeapon;
            }

            if (IsFireHeld())
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

        private static bool WasPressedThisFrame(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return key switch
                {
                    KeyCode.Alpha1 => Keyboard.current.digit1Key.wasPressedThisFrame,
                    KeyCode.Alpha2 => Keyboard.current.digit2Key.wasPressedThisFrame,
                    KeyCode.Q => Keyboard.current.qKey.wasPressedThisFrame,
                    _ => false
                };
            }
#endif
            return Input.GetKeyDown(key);
        }

        private static bool IsFireHeld()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                return Mouse.current.leftButton.isPressed;
            }
#endif
            return Input.GetMouseButton(0);
        }
    }
}
