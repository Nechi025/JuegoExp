using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class MistakeManager : MonoBehaviour
    {
        private bool _isGameOver;

        public int CurrentMistakes { get; private set; }

        private void Awake()
        {
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
            if (_isGameOver) return;
            CurrentMistakes++;
        }
    }
}
