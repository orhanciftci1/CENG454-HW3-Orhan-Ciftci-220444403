using CoreBreach.Core;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CoreBreach.Combat
{
    public sealed class GameStateController : MonoBehaviour
    {
        [SerializeField] private CoreHealth core;
        [SerializeField] private WaveSpawner waveSpawner;

        public bool IsGameOver { get; private set; }
        public string ResultText { get; private set; } = "";

        private void OnEnable()
        {
            if (core != null)
            {
                core.OnCoreDestroyed += HandleCoreDestroyed;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnWaveCompleted += HandleWaveCompleted;
            }
        }

        private void OnDisable()
        {
            if (core != null)
            {
                core.OnCoreDestroyed -= HandleCoreDestroyed;
            }

            if (waveSpawner != null)
            {
                waveSpawner.OnWaveCompleted -= HandleWaveCompleted;
            }
        }

        private void HandleCoreDestroyed()
        {
            EndGame("CORE LOST - Press R to retry");
        }

        private void HandleWaveCompleted()
        {
            EndGame("FACILITY HELD - Press R to retry");
        }

        private void Update()
        {
            if (IsGameOver && RetryPressed())
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
        }

        private void EndGame(string result)
        {
            if (IsGameOver)
            {
                return;
            }

            IsGameOver = true;
            ResultText = result;
            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        private static bool RetryPressed()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                return Keyboard.current.rKey.wasPressedThisFrame;
            }
#endif
            return Input.GetKeyDown(KeyCode.R);
        }
    }
}
