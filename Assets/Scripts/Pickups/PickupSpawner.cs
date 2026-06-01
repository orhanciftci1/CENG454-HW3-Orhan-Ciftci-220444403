using CoreBreach.Combat;
using CoreBreach.Core;
using CoreBreach.Enemies;
using UnityEngine;

namespace CoreBreach.Pickups
{
    public sealed class PickupSpawner : MonoBehaviour
    {
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private CoreHealth core;
        [SerializeField] private RepairPickup repairPickupPrefab;
        [SerializeField, Range(0f, 1f)] private float dropChance = 0.28f;

        private void OnEnable()
        {
            if (waveSpawner != null)
            {
                waveSpawner.OnEnemyDefeated += HandleEnemyDefeated;
            }
        }

        private void OnDisable()
        {
            if (waveSpawner != null)
            {
                waveSpawner.OnEnemyDefeated -= HandleEnemyDefeated;
            }
        }

        private void HandleEnemyDefeated(EnemyUnit enemy, int scoreValue)
        {
            if (repairPickupPrefab == null || core == null || Random.value > dropChance)
            {
                return;
            }

            RepairPickup pickup = Instantiate(repairPickupPrefab, enemy.transform.position, Quaternion.identity);
            pickup.Configure(core);
            pickup.gameObject.SetActive(true);
        }
    }
}
