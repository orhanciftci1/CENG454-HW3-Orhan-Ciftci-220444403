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
        [SerializeField] private int totalWaves = 3;
        [SerializeField] private int enemiesInFirstWave = 8;
        [SerializeField] private int enemiesAddedPerWave = 5;
        [SerializeField] private float spawnInterval = 1.45f;
        [SerializeField] private float timeBetweenWaves = 3f;

        public event Action<int> OnEnemyCountChanged;
        public event Action<int, int> OnWaveChanged;
        public event Action<int> OnWaveCleared;
        public event Action<EnemyUnit, int> OnEnemyDefeated;
        public event Action OnWaveCompleted;

        private int spawned;
        private int spawnedThisWave;
        private int enemiesThisWave;
        private int alive;
        private int currentWave;
        private bool isRunning;

        public int CurrentWave => currentWave;
        public int TotalWaves => totalWaves;

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
            for (currentWave = 1; currentWave <= totalWaves; currentWave++)
            {
                enemiesThisWave = enemiesInFirstWave + (currentWave - 1) * enemiesAddedPerWave;
                spawnedThisWave = 0;
                OnWaveChanged?.Invoke(currentWave, totalWaves);

                while (spawnedThisWave < enemiesThisWave && core != null && core.IsAlive)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(spawnInterval);
                }

                while (alive > 0 && core != null && core.IsAlive)
                {
                    yield return null;
                }

                if (core == null || !core.IsAlive)
                {
                    yield break;
                }

                OnWaveCleared?.Invoke(currentWave);

                if (currentWave < totalWaves)
                {
                    yield return new WaitForSeconds(timeBetweenWaves);
                }
            }

            OnWaveCompleted?.Invoke();
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
            spawnedThisWave++;
            alive++;
            OnEnemyCountChanged?.Invoke(alive);
        }

        private void HandleEnemyKilled(EnemyUnit enemy)
        {
            enemy.OnEnemyKilled -= HandleEnemyKilled;
            alive--;
            OnEnemyCountChanged?.Invoke(alive);
            OnEnemyDefeated?.Invoke(enemy, enemy.ScoreValue);
        }
    }
}
