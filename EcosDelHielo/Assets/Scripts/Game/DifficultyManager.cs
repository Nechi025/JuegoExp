using Core.Services;
using UnityEngine;

namespace Game
{
    public class DifficultyManager : MonoBehaviour
    {
        [SerializeField] private AnimationCurve patienceCurve    = AnimationCurve.Linear(0, 1, 1, 0.4f);
        [SerializeField] private AnimationCurve purifyClicksCurve = AnimationCurve.Linear(0, 1, 1, 3);

        private GameConfig _config;
        private float _basePatienceTime;
        private int   _basePurifyClicks;
        private float _elapsed;
        private bool  _isGameOver;

        private void Start()
        {
            _config           = ServiceLocator.Get<GameManager>().Config;
            _basePatienceTime = _config.customerPatienceTime;
            _basePurifyClicks = _config.purifyClicksRequired;
        }

        private void OnEnable()  => GameEventBus.OnStateChanged += HandleStateChanged;
        private void OnDisable() => GameEventBus.OnStateChanged -= HandleStateChanged;

        private void HandleStateChanged(GameState state) =>
            _isGameOver = state == GameState.GameOver;

        public void Tick(float deltaTime)
        {
            if (_isGameOver) return;
            _elapsed += deltaTime;
            float t = Mathf.Clamp01(_elapsed / _config.difficultyRampDuration);

            _config.customerPatienceTime = _basePatienceTime * patienceCurve.Evaluate(t);
            _config.purifyClicksRequired = Mathf.Max(1, Mathf.RoundToInt(_basePurifyClicks * purifyClicksCurve.Evaluate(t)));
        }

        private void Update() => Tick(Time.deltaTime);

        private void OnDestroy()
        {
            if (_config == null) return;
            _config.customerPatienceTime = _basePatienceTime;
            _config.purifyClicksRequired = _basePurifyClicks;
        }
    }
}
