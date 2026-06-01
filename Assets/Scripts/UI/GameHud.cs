using CoreBreach.Combat;
using CoreBreach.Core;
using CoreBreach.Player;
using UnityEngine;
using UnityEngine.UI;

namespace CoreBreach.UI
{
    public sealed class GameHud : MonoBehaviour
    {
        [SerializeField] private CoreHealth core;
        [SerializeField] private WaveSpawner waveSpawner;
        [SerializeField] private ScoreKeeper scoreKeeper;
        [SerializeField] private PlayerWeapon playerWeapon;
        [SerializeField] private GameStateController gameState;
        [SerializeField] private Text statusText;

        private float currentCore;
        private float maxCore;
        private int enemiesAlive;
        private int currentWave;
        private int totalWaves;
        private int score;

        private void OnEnable()
        {
            if (core != null)
            {
                core.OnHealthChanged += HandleCoreHealthChanged;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnEnemyCountChanged += HandleEnemyCountChanged;
                waveSpawner.OnWaveChanged += HandleWaveChanged;
            }

            if (scoreKeeper != null)
            {
                scoreKeeper.OnScoreChanged += HandleScoreChanged;
            }
        }

        private void OnDisable()
        {
            if (core != null)
            {
                core.OnHealthChanged -= HandleCoreHealthChanged;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnEnemyCountChanged -= HandleEnemyCountChanged;
                waveSpawner.OnWaveChanged -= HandleWaveChanged;
            }

            if (scoreKeeper != null)
            {
                scoreKeeper.OnScoreChanged -= HandleScoreChanged;
            }
        }

        private void Update()
        {
            string result = gameState != null && gameState.IsGameOver ? "\n" + gameState.ResultText : "";
            string weapon = playerWeapon != null ? playerWeapon.ActiveWeaponName : "None";
            statusText.text = $"Core: {currentCore:0}/{maxCore:0}\nWave: {currentWave}/{totalWaves}\nEnemies: {enemiesAlive}\nScore: {score}\nWeapon: {weapon}\n1/2 switch, Q boost\nGreen pickups repair the core{result}";
        }

        private void HandleCoreHealthChanged(float current, float max)
        {
            currentCore = current;
            maxCore = max;
        }

        private void HandleEnemyCountChanged(int count)
        {
            enemiesAlive = count;
        }

        private void HandleWaveChanged(int wave, int waveTotal)
        {
            currentWave = wave;
            totalWaves = waveTotal;
        }

        private void HandleScoreChanged(int newScore)
        {
            score = newScore;
        }
    }
}
