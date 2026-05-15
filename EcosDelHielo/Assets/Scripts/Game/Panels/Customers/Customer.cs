using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Customers
{
    public class Customer : MonoBehaviour
    {
        private GameConfig _config;
        private GameConfig Config => _config ??= ServiceLocator.Get<GameManager>().Config;

        private float _patience;
        private bool  _isServed;
        private bool  _hasLeft;
        private bool  _initialized;

        public bool  IsActive          => !_isServed && !_hasLeft;
        public float PatienceNormalized => IsActive ? _patience / Config.customerPatienceTime : 0f;

        private void Awake()
        {
            _patience    = Config.customerPatienceTime;
            _initialized = true;
        }

        public void Serve()
        {
            if (!IsActive) return;
            _isServed = true;
        }

        public void Tick(float deltaTime)
        {
            if (!IsActive) return;
            _patience -= deltaTime;
            if (_patience <= 0f) Leave();
        }

        private void Leave()
        {
            _hasLeft = true;
            GameEventBus.RaiseMistake();
        }

        private void Update() => Tick(Time.deltaTime);
    }
}
