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

        private readonly List<IceCube>  _activeCubes  = new List<IceCube>();
        private readonly List<IceCube>  _insideMouth  = new List<IceCube>();
        private readonly Queue<IceCube> _parkedQueue  = new Queue<IceCube>();

        private void Awake()
        {
            _funnelMouthNotifier = funnelMouth.GetComponent<TriggerNotifier>()
                                ?? funnelMouth.gameObject.AddComponent<TriggerNotifier>();
            _funnelMouthNotifier.OnEntered += OnIceCubeEnteredMouth;
            _funnelMouthNotifier.OnExited  += OnIceCubeExitedMouth;

            if (bottleConveyor == null)
            {
                Debug.LogError("[FunnelPanel] bottleConveyor is not assigned in the Inspector.", this);
                return;
            }
            bottleConveyor.OnArrived += OnBottleArrived;

            GameEventBus.OnIceCubeSpawned += HandleIceCubeSpawned;
            GameEventBus.OnStateChanged   += HandleStateChanged;
        }

        private void OnDestroy()
        {
            _funnelMouthNotifier.OnEntered -= OnIceCubeEnteredMouth;
            _funnelMouthNotifier.OnExited  -= OnIceCubeExitedMouth;

            bottleConveyor.OnArrived -= OnBottleArrived;

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
            var cube = _activeCubes.Find(c => c != null && c.gameObject == other.gameObject);
            if (cube == null) return;

            if (!_insideMouth.Contains(cube))
                _insideMouth.Add(cube);

            // Only one cube waits at a time; others stay free until that slot clears
            if (_parkedQueue.Count > 0) return;

            if (bottleConveyor.IsReadyToFill)
                DepositCube(cube);
            else
                ParkCube(cube);
        }

        private void OnIceCubeExitedMouth(Collider2D other)
        {
            var cube = _insideMouth.Find(c => c != null && c.gameObject == other.gameObject);
            if (cube != null) _insideMouth.Remove(cube);
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
            while (_parkedQueue.Count > 0 && _parkedQueue.Peek() == null)
                _parkedQueue.Dequeue();

            if (_parkedQueue.Count > 0)
            {
                DepositCube(_parkedQueue.Dequeue());
                return;
            }

            // Fallback: cube already inside collider that won't re-trigger enter
            for (int i = _insideMouth.Count - 1; i >= 0; i--)
            {
                if (_insideMouth[i] == null) { _insideMouth.RemoveAt(i); continue; }
                var waiting = _insideMouth[i];
                var rb = waiting.GetComponent<Rigidbody2D>();
                if (rb != null) { rb.velocity = Vector2.zero; rb.isKinematic = true; }
                DepositCube(waiting);
                return;
            }
        }

        private void DepositCube(IceCube cube)
        {
            _activeCubes.Remove(cube);
            _insideMouth.Remove(cube);
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
