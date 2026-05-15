using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Funnel
{
    public class IceCube : MonoBehaviour
    {
        [SerializeField] private Sprite[] purifySprites;

        private SpriteRenderer _sr;
        private GameConfig     _config;
        private GameConfig     Config => _config ??= ServiceLocator.Get<GameManager>().Config;

        private int  _purifyClicks;
        private int  _purifyClicksRequired;
        private bool _hasReachedFunnel;

        public bool IsPure           => _purifyClicks >= _purifyClicksRequired;
        public bool HasReachedFunnel => _hasReachedFunnel;

        private void Awake()
        {
            _sr                   = GetComponent<SpriteRenderer>();
            _purifyClicksRequired = Config.purifyClicksRequired;
            UpdateSprite();
        }

        private void OnMouseDown() => OnClick();

        public void OnClick()
        {
            if (_hasReachedFunnel) return;
            _purifyClicks++;
            Debug.Log($"[IceCube] Purify click {_purifyClicks}/{_purifyClicksRequired}");
            UpdateSprite();
        }

        public void ReachFunnel()
        {
            if (_hasReachedFunnel) return;
            _hasReachedFunnel = true;
            Debug.Log($"[IceCube] Reached funnel — IsPure: {IsPure}");
            if (!IsPure) GameEventBus.RaiseMistake();
        }

        private void UpdateSprite()
        {
            if (_sr == null || purifySprites.Length == 0) return;
            float t   = _purifyClicksRequired > 0
                        ? Mathf.Clamp01((float)_purifyClicks / _purifyClicksRequired)
                        : 1f;
            int   idx = Mathf.Clamp(Mathf.RoundToInt(t * (purifySprites.Length - 1)), 0, purifySprites.Length - 1);
            _sr.sprite = purifySprites[idx];
        }
    }
}
