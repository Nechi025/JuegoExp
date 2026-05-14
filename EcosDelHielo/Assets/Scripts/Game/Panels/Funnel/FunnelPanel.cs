using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Funnel
{
    public class FunnelPanel : MonoBehaviour
    {
        [SerializeField] private Transform  fallStartPoint;
        [SerializeField] private Transform  funnelMouthPoint;
        [SerializeField] private GameObject iceCubePrefab;

        private GameConfig  _config;
        private GameState   _gameState = GameState.Playing;
        private IceCube     _fallingCube;
        private Bottle      _currentBottle;

        private void Awake()
        {
            _config        = ServiceLocator.Get<GameConfig>();
            _currentBottle = new Bottle(_config.cubesPerBottle);
        }

        private void OnEnable()
        {
            GameEventBus.OnIceCubeSpawned += HandleIceCubeSpawned;
            GameEventBus.OnStateChanged   += HandleStateChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnIceCubeSpawned -= HandleIceCubeSpawned;
            GameEventBus.OnStateChanged   -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state) => _gameState = state;

        private void HandleIceCubeSpawned()
        {
            if (_fallingCube != null) return;
            var go = Instantiate(iceCubePrefab, fallStartPoint.position, Quaternion.identity);
            _fallingCube = go.GetComponent<IceCube>();
        }

        private void Update()
        {
            if (_gameState != GameState.Playing) return;
            if (_fallingCube == null) return;

            _fallingCube.transform.position = Vector3.MoveTowards(
                _fallingCube.transform.position,
                funnelMouthPoint.position,
                _config.iceCubeFallSpeed * Time.deltaTime
            );

            if (Vector3.Distance(_fallingCube.transform.position, funnelMouthPoint.position) < 0.05f)
                DepositCube();
        }

        private void DepositCube()
        {
            _fallingCube.ReachFunnel();
            _currentBottle.AddCube(_fallingCube.IsPure);
            Destroy(_fallingCube.gameObject);
            _fallingCube = null;

            if (!_currentBottle.IsFull) return;

            GameEventBus.RaiseBottleReady(!_currentBottle.IsTainted);
            _currentBottle = new Bottle(_config.cubesPerBottle);
        }

        public void OnCubeClicked()
        {
            _fallingCube?.OnClick();
        }
    }
}
