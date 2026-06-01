using System;
using UnityEngine;

namespace CoreBreach.Combat
{
    public sealed class ScoreKeeper : MonoBehaviour
    {
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private int waveClearBonus = 500;

        public event Action<int> OnScoreChanged;

        public int Score { get; private set; }

        private void OnEnable()
        {
            if (waveSpawner != null)
            {
                waveSpawner.OnEnemyDefeated += HandleEnemyDefeated;
                waveSpawner.OnWaveCleared += HandleWaveCleared;
            }
        }

        private void OnDisable()
        {
            if (waveSpawner != null)
            {
                waveSpawner.OnEnemyDefeated -= HandleEnemyDefeated;
                waveSpawner.OnWaveCleared -= HandleWaveCleared;
            }
        }

        private void HandleEnemyDefeated(Enemies.EnemyUnit enemy, int scoreValue)
        {
            AddScore(scoreValue);
        }

        private void HandleWaveCleared(int waveNumber)
        {
            AddScore(waveClearBonus * waveNumber);
        }

        private void AddScore(int amount)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }
    }
}
