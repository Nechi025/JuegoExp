using Core;
using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Funnel
{
    public class FunnelPanel : MonoBehaviour
    {
        [Header("Bottle Path")]
        [SerializeField] private Transform      bottleSpawnPoint;
        [SerializeField] private Transform      bottleFillPoint;
        [SerializeField] private Transform      bottleLeavePoint;
        [SerializeField] private BottleConveyor bottleConveyor;

        [Header("Ice Ball")]
        [SerializeField] private Transform   iceBallSpawnPoint;
        [SerializeField] private Collider2D  funnelMouth;
        [SerializeField] private GameObject  iceCubePrefab;
        [SerializeField] private float           spawnSpeed    = 4f;
        [SerializeField] [Range(0f, 45f)] private float maxSpawnAngle = 20f;

        private GameConfig      _config;
        private GameState       _gameState = GameState.Playing;
        private IceCube         _fallingCube;
        private Bottle          _currentBottle;
        private bool            _cubeParked;
        private TriggerNotifier _funnelMouthNotifier;

        private void Awake()
        {
            _funnelMouthNotifier = funnelMouth.GetComponent<TriggerNotifier>()
                                ?? funnelMouth.gameObject.AddComponent<TriggerNotifier>();
            _funnelMouthNotifier.OnEntered += OnIceCubeEnteredMouth;

            bottleConveyor.OnArrived  += OnBottleArrived;
            bottleConveyor.OnDeparted += OnBottleDeparted;

            GameEventBus.OnIceCubeSpawned += HandleIceCubeSpawned;
            GameEventBus.OnStateChanged   += HandleStateChanged;
        }

        private void OnDestroy()
        {
            _funnelMouthNotifier.OnEntered -= OnIceCubeEnteredMouth;

            bottleConveyor.OnArrived  -= OnBottleArrived;
            bottleConveyor.OnDeparted -= OnBottleDeparted;

            GameEventBus.OnIceCubeSpawned -= HandleIceCubeSpawned;
            GameEventBus.OnStateChanged   -= HandleStateChanged;
        }

        private void Start()
        {
            _config        = ServiceLocator.Get<GameManager>().Config;
            _currentBottle = new Bottle(_config.cubesPerBottle);
            bottleConveyor.BeginCycle(bottleSpawnPoint.position, bottleFillPoint.position);
        }

        private void HandleStateChanged(GameState state) => _gameState = state;

        private void HandleIceCubeSpawned()
        {
            if (_fallingCube != null) return;
            var go = Instantiate(iceCubePrefab, iceBallSpawnPoint.position, Quaternion.identity);
            _fallingCube = go.GetComponent<IceCube>();
            _cubeParked  = false;

            float angle = Random.Range(-maxSpawnAngle, maxSpawnAngle);
            var   dir   = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
            var   rb    = go.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = (Vector2)dir * spawnSpeed;
        }

        private void OnIceCubeEnteredMouth(Collider2D other)
        {
            if (_gameState != GameState.Playing) return;
            if (_fallingCube == null || other.gameObject != _fallingCube.gameObject) return;

            if (!bottleConveyor.IsReadyToFill)
                ParkCube();
            else
                DepositCube();
        }

        private void ParkCube()
        {
            if (_cubeParked) return;
            _cubeParked = true;
            var rb = _fallingCube.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity    = Vector2.zero;
                rb.isKinematic = true;
            }
            _fallingCube.transform.position = funnelMouth.transform.position;
        }

        private void OnBottleArrived()
        {
            if (_fallingCube != null && _cubeParked)
                DepositCube();
        }

        private void OnBottleDeparted(bool _) =>
            bottleConveyor.BeginCycle(bottleSpawnPoint.position, bottleFillPoint.position);

        private void DepositCube()
        {
            var rb = _fallingCube.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = false;

            _fallingCube.ReachFunnel();
            _currentBottle.AddCube(_fallingCube.IsPure);
            Destroy(_fallingCube.gameObject);
            _fallingCube = null;
            _cubeParked  = false;

            if (!_currentBottle.IsFull) return;

            bottleConveyor.Depart(bottleLeavePoint.position, !_currentBottle.IsTainted);
            _currentBottle = new Bottle(_config.cubesPerBottle);
        }

        public void OnCubeClicked() => _fallingCube?.OnClick();
    }
}
