using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class GameStateManager : MonoBehaviour
    {
        public GameState CurrentState { get; private set; } = GameState.Playing;

        private void Awake()    => GameEventBus.OnGameOver += HandleGameOver;
        private void OnDestroy() => GameEventBus.OnGameOver -= HandleGameOver;

        private void HandleGameOver()
        {
            if (CurrentState == GameState.GameOver) return;
            CurrentState = GameState.GameOver;
            GameEventBus.RaiseStateChanged(GameState.GameOver);
        }
    }
}
