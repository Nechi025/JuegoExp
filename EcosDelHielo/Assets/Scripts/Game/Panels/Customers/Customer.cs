using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Customers
{
    public class Customer : MonoBehaviour
    {
        private GameConfig _config;
        private float _patience;
        private bool  _isServed;
        private bool  _hasLeft;
        private bool  _initialized;

        public bool  IsActive          => !_isServed && !_hasLeft;
        public float PatienceNormalized { get { Init(); return _patience / _config.customerPatienceTime; } }

        private void Init()
        {
            if (_initialized) return;
            _config      = ServiceLocator.Get<GameConfig>();
            _patience    = _config.customerPatienceTime;
            _initialized = true;
        }

        private void Awake() => Init();

        public void Serve()
        {
            if (!IsActive) return;
            _isServed = true;
        }

        public void Tick(float deltaTime)
        {
            Init();
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
