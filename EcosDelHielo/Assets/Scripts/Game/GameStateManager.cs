using UnityEngine;

namespace Game
{
    public class GameStateManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; } = GameState.Playing;

        private void OnEnable()  => GameEventBus.OnGameOver += HandleGameOver;
        private void OnDisable() => GameEventBus.OnGameOver -= HandleGameOver;

        private void HandleGameOver()
        {
            if (CurrentState == GameState.GameOver) return;
            CurrentState = GameState.GameOver;
            GameEventBus.RaiseStateChanged(GameState.GameOver);
        }
    }
}
