using Core.Services;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class ScoreManager : MonoBehaviour
    {
        private GameConfig _config;
        private bool _isGameOver;

        public int   Score               { get; private set; }
        public float TimeAlive           { get; private set; }
        public int   DeliveriesCompleted { get; private set; }

        private void Awake()
        {
            if (ServiceLocator.TryGet<GameManager>(out var gm)) _config = gm.Config;
            GameEventBus.OnDeliverySuccess += HandleDelivery;
            GameEventBus.OnStateChanged    += HandleStateChanged;
        }

        private void OnDestroy()
        {
            GameEventBus.OnDeliverySuccess -= HandleDelivery;
            GameEventBus.OnStateChanged    -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state) =>
            _isGameOver = state == GameState.GameOver;

        private void HandleDelivery()
        {
            if (_isGameOver || _config == null) return;
            Score += _config.scorePerDelivery;
            DeliveriesCompleted++;
        }

        public void Tick(float deltaTime)
        {
            if (_isGameOver || _config == null) return;
            TimeAlive += deltaTime;
            Score     += Mathf.FloorToInt(_config.scorePerSecond * deltaTime);
        }

        private void Update() => Tick(Time.deltaTime);
    }
}
