using Core.Services;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class MistakeManager : MonoBehaviour
    {
        private GameConfig _config;
        private bool _isGameOver;
        private bool _initialized;

        public int CurrentMistakes { get; private set; }

        private void Init()
        {
            if (_initialized) return;
            if (!ServiceLocator.TryGet<GameConfig>(out _config)) return;
            _initialized = true;
        }

        private void Awake()
        {
            Init();
            GameEventBus.OnMistake      += HandleMistake;
            GameEventBus.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            GameEventBus.OnMistake      -= HandleMistake;
            GameEventBus.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state) =>
            _isGameOver = state == GameState.GameOver;

        private void HandleMistake()
        {
            Init();
            if (!_initialized || _isGameOver) return;
            CurrentMistakes++;
            if (CurrentMistakes >= _config.maxMistakes)
            {
                _isGameOver = true;
                GameEventBus.RaiseGameOver();
            }
        }
    }
}
