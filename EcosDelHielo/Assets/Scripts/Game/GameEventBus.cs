using System;

namespace Game
{
    public static class GameEventBus
    {
        public static event Action OnMistake;
        public static event Action OnIceCubeSpawned;
        public static event Action<bool> OnBottleReady;
        public static event Action OnDeliverySuccess;
        public static event Action OnGameOver;
        public static event Action<GameState> OnStateChanged;

        public static void RaiseMistake()          => OnMistake?.Invoke();
        public static void RaiseIceCubeSpawned()   => OnIceCubeSpawned?.Invoke();
        public static void RaiseBottleReady(bool isPure) => OnBottleReady?.Invoke(isPure);
        public static void RaiseDeliverySuccess()  => OnDeliverySuccess?.Invoke();
        public static void RaiseGameOver()         => OnGameOver?.Invoke();
        public static void RaiseStateChanged(GameState state) => OnStateChanged?.Invoke(state);

        public static void ClearAll()
        {
            OnMistake          = null;
            OnIceCubeSpawned   = null;
            OnBottleReady      = null;
            OnDeliverySuccess  = null;
            OnGameOver         = null;
            OnStateChanged     = null;
        }
    }
}
