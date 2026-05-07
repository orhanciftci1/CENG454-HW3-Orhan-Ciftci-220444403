using System;
using System.Collections;
using CoreBreach.Core;
using CoreBreach.Enemies;
using UnityEngine;

namespace CoreBreach.Combat
{
    public sealed class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private EnemyUnit directEnemyPrefab;
        [SerializeField] private EnemyUnit strafeEnemyPrefab;
        [SerializeField] private CoreHealth core;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int totalEnemies = 28;
        [SerializeField] private float spawnInterval = 2.2f;

        public event Action<int> OnEnemyCountChanged;
        public event Action OnWaveCompleted;

        private int spawned;
        private int alive;
        private bool isRunning;

        private void Start()
        {
            StartWave();
        }

        public void StartWave()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (spawned < totalEnemies && core != null && core.IsAlive)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnEnemy()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return;
            }

            EnemyUnit prefab = spawned % 3 == 0 ? strafeEnemyPrefab : directEnemyPrefab;
            Transform point = spawnPoints[spawned % spawnPoints.Length];
            EnemyUnit enemy = Instantiate(prefab, point.position, Quaternion.identity);
            enemy.Configure(core.transform);
            enemy.gameObject.SetActive(true);
            enemy.OnEnemyKilled += HandleEnemyKilled;
            spawned++;
            alive++;
            OnEnemyCountChanged?.Invoke(alive);
        }

        private void HandleEnemyKilled(EnemyUnit enemy)
        {
            enemy.OnEnemyKilled -= HandleEnemyKilled;
            alive--;
            OnEnemyCountChanged?.Invoke(alive);

            if (spawned >= totalEnemies && alive <= 0)
            {
                OnWaveCompleted?.Invoke();
            }
        }
    }
}
