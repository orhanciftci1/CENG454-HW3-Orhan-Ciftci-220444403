using CoreBreach.Core;
using UnityEngine;

namespace CoreBreach.Pickups
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class RepairPickup : MonoBehaviour
    {
        [SerializeField] private float repairAmount = 18f;
        [SerializeField] private float lifetime = 9f;
        [SerializeField] private CoreHealth core;

        private float timer;

        private void OnEnable()
        {
            timer = lifetime;
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            transform.Rotate(0f, 0f, 120f * Time.deltaTime);

            if (timer <= 0f)
            {
                Destroy(gameObject);
            }
        }

        public void Configure(CoreHealth targetCore)
        {
            core = targetCore;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<Player.PlayerController>(out _))
            {
                return;
            }

            if (core != null)
            {
                core.Heal(repairAmount);
            }

            Destroy(gameObject);
        }
    }
}
