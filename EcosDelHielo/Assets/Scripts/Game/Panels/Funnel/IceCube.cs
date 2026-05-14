using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Funnel
{
    public class IceCube : MonoBehaviour
    {
        private GameConfig _config;
        private int  _purifyClicks;
        private bool _hasReachedFunnel;

        public bool IsPure           => _purifyClicks >= _config.purifyClicksRequired;
        public bool HasReachedFunnel => _hasReachedFunnel;

        private void Awake() => _config = ServiceLocator.Get<GameConfig>();

        public void OnClick()
        {
            if (_hasReachedFunnel) return;
            _purifyClicks++;
        }

        public void ReachFunnel()
        {
            if (_hasReachedFunnel) return;
            _hasReachedFunnel = true;
            if (!IsPure) GameEventBus.RaiseMistake();
        }
    }
}
