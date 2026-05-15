using System.Collections.Generic;
using Core;
using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Funnel
{
    public class FunnelPanel : MonoBehaviour
    {
        [Header("Bottle Path")]
        [SerializeField] private BottleConveyor bottleConveyor;

        [Header("Ice Ball")]
        [SerializeField] private Transform   iceBallSpawnPoint;
        [SerializeField] private Collider2D  funnelMouth;
        [SerializeField] private GameObject  iceCubePrefab;
        [SerializeField] private float           spawnSpeed    = 4f;
        [SerializeField] [Range(0f, 45f)] private float maxSpawnAngle = 20f;

        private GameConfig      _config;
        private GameState       _gameState = GameState.Playing;
        private Bottle          _currentBottle;
        private TriggerNotifier _funnelMouthNotifier;

        private readonly List<IceCube>  _activeCubes = new List<IceCube>();
        private readonly Queue<IceCube> _parkedQueue  = new Queue<IceCube>();

        private void Awake()
        {
            _funnelMouthNotifier = funnelMouth.GetComponent<TriggerNotifier>()
                                ?? funnelMouth.gameObject.AddComponent<TriggerNotifier>();
            _funnelMouthNotifier.OnEntered += OnIceCubeEnteredMouth;

            if (bottleConveyor == null)
            {
                Debug.LogError("[FunnelPanel] bottleConveyor is not assigned in the Inspector.", this);
                return;
            }
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
            bottleConveyor.BeginCycle();
        }

        private void HandleStateChanged(GameState state) => _gameState = state;

        private void HandleIceCubeSpawned()
        {
            var go   = Instantiate(iceCubePrefab, iceBallSpawnPoint.position, Quaternion.identity);
            var cube = go.GetComponent<IceCube>();
            _activeCubes.Add(cube);

            float angle = Random.Range(-maxSpawnAngle, maxSpawnAngle);
            var   dir   = Quaternion.Euler(0f, 0f, angle) * Vector2.down;
            var   rb    = go.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = (Vector2)dir * spawnSpeed;
        }

        private void OnIceCubeEnteredMouth(Collider2D other)
        {
            if (_gameState != GameState.Playing) return;
            if (_parkedQueue.Count > 0) return;
            var cube = _activeCubes.Find(c => c != null && c.gameObject == other.gameObject);
            if (cube == null) return;

            if (bottleConveyor.IsReadyToFill)
                DepositCube(cube);
            else
                ParkCube(cube);
        }

        private void ParkCube(IceCube cube)
        {
            if (_parkedQueue.Contains(cube)) return;
            var rb = cube.GetComponent<Rigidbody2D>();
            if (rb != null) { rb.velocity = Vector2.zero; rb.isKinematic = true; }
            cube.transform.position = funnelMouth.transform.position;
            _parkedQueue.Enqueue(cube);
        }

        private void OnBottleArrived()
        {
            if (_parkedQueue.Count > 0)
                DepositCube(_parkedQueue.Dequeue());
        }

        private void OnBottleDeparted(bool _) =>
            bottleConveyor.BeginCycle();

        private void DepositCube(IceCube cube)
        {
            _activeCubes.Remove(cube);
            var rb = cube.GetComponent<Rigidbody2D>();
            if (rb != null) rb.isKinematic = false;

            cube.ReachFunnel();
            _currentBottle.AddCube(cube.IsPure);
            Destroy(cube.gameObject);

            if (!_currentBottle.IsFull) return;
            bottleConveyor.Depart(!_currentBottle.IsTainted);
            _currentBottle = new Bottle(_config.cubesPerBottle);
        }
    }
}
