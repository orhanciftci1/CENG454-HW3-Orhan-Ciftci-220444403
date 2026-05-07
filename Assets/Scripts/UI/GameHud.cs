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
        [SerializeField] private PlayerWeapon playerWeapon;
        [SerializeField] private GameStateController gameState;
        [SerializeField] private Text statusText;

        private float currentCore;
        private float maxCore;
        private int enemiesAlive;

        private void OnEnable()
        {
            if (core != null)
            {
                core.OnHealthChanged += HandleCoreHealthChanged;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnEnemyCountChanged += HandleEnemyCountChanged;
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
            }
        }

        private void Update()
        {
            string result = gameState != null && gameState.IsGameOver ? "\n" + gameState.ResultText : "";
            string weapon = playerWeapon != null ? playerWeapon.ActiveWeaponName : "None";
            statusText.text = $"Core: {currentCore:0}/{maxCore:0}\nEnemies: {enemiesAlive}\nWeapon: {weapon}\n1/2 switch, Q boost{result}";
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
    }
}
