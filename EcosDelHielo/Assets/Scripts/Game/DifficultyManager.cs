using Core.Services;
using UnityEngine;

namespace Game
{
    public class DifficultyManager : MonoBehaviour
    {
        [SerializeField] private AnimationCurve decayRateCurve  = AnimationCurve.Linear(0, 1, 1, 3);
        [SerializeField] private AnimationCurve fallSpeedCurve  = AnimationCurve.Linear(0, 1, 1, 4);
        [SerializeField] private AnimationCurve patienceCurve   = AnimationCurve.Linear(0, 1, 1, 0.4f);
        [SerializeField] private AnimationCurve beltSpeedCurve  = AnimationCurve.Linear(0, 1, 1, 3);

        private GameConfig _config;
        private float _baseDecayRate;
        private float _baseFallSpeed;
        private float _basePatienceTime;
        private float _baseBeltSpeed;
        private float _elapsed;
        private bool  _isGameOver;

        private void Awake()
        {
            _config          = ServiceLocator.Get<GameConfig>();
            _baseDecayRate   = _config.glacierPassiveDecayRate;
            _baseFallSpeed   = _config.iceCubeFallSpeed;
            _basePatienceTime = _config.customerPatienceTime;
            _baseBeltSpeed   = _config.beltMoveSpeed;
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
            _config.glacierPassiveDecayRate = _baseDecayRate   * decayRateCurve.Evaluate(t);
            _config.iceCubeFallSpeed        = _baseFallSpeed   * fallSpeedCurve.Evaluate(t);
            _config.customerPatienceTime    = _basePatienceTime * patienceCurve.Evaluate(t);
            _config.beltMoveSpeed           = _baseBeltSpeed   * beltSpeedCurve.Evaluate(t);
        }

        private void Update() => Tick(Time.deltaTime);

        private void OnDestroy()
        {
            if (_config == null) return;
            _config.glacierPassiveDecayRate = _baseDecayRate;
            _config.iceCubeFallSpeed        = _baseFallSpeed;
            _config.customerPatienceTime    = _basePatienceTime;
            _config.beltMoveSpeed           = _baseBeltSpeed;
        }
    }
}
