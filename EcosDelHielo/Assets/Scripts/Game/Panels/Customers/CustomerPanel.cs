using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Customers
{
    public class CustomerPanel : MonoBehaviour
    {
        [SerializeField] private Transform[]  customerSlots;   // 4 Transforms: TL, TR, BL, BR
        [SerializeField] private Transform    bottleSpawnPoint;
        [SerializeField] private Transform    recycleBinZone;
        [SerializeField] private GameObject   customerPrefab;
        [SerializeField] private GameObject   bottlePrefab;

        private GameConfig  _config;
        private Customer[]  _customers;
        private GameObject  _pendingBottle;
        private bool        _isPureBottle;
        private GameState   _gameState = GameState.Playing;
        private float       _spawnTimer;
        private float       _bottleTimer;

        private void Awake()
        {
            _config    = ServiceLocator.Get<GameConfig>();
            _customers = new Customer[customerSlots.Length];
        }

        private void OnEnable()
        {
            GameEventBus.OnBottleReady  += HandleBottleReady;
            GameEventBus.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            GameEventBus.OnBottleReady  -= HandleBottleReady;
            GameEventBus.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state) => _gameState = state;

        private void HandleBottleReady(bool isPure)
        {
            if (_pendingBottle != null) return;
            _isPureBottle  = isPure;
            _pendingBottle = Instantiate(bottlePrefab, bottleSpawnPoint.position, Quaternion.identity);
        }

        private void Update()
        {
            if (_gameState != GameState.Playing) return;
            TickCustomerSpawn(Time.deltaTime);
            TickBottleTimeout(Time.deltaTime);
        }

        private void TickCustomerSpawn(float deltaTime)
        {
            _spawnTimer += deltaTime;
            if (_spawnTimer < _config.customerSpawnInterval) return;
            _spawnTimer = 0f;
            TrySpawnCustomer();
        }

        private void TickBottleTimeout(float deltaTime)
        {
            if (_pendingBottle == null) { _bottleTimer = 0f; return; }
            _bottleTimer += deltaTime;
            if (_bottleTimer < _config.bottleTimeout) return;
            _bottleTimer = 0f;
            Destroy(_pendingBottle);
            _pendingBottle = null;
            GameEventBus.RaiseMistake();
        }

        private void TrySpawnCustomer()
        {
            for (int i = 0; i < _customers.Length; i++)
            {
                if (_customers[i] != null && _customers[i].IsActive) continue;
                SpawnCustomerAt(i);
                return;
            }
        }

        private void SpawnCustomerAt(int slotIndex)
        {
            if (_customers[slotIndex] != null)
                Destroy(_customers[slotIndex].gameObject);

            var go = Instantiate(customerPrefab, customerSlots[slotIndex].position, Quaternion.identity);
            go.transform.SetParent(customerSlots[slotIndex]);
            _customers[slotIndex] = go.GetComponent<Customer>();
        }

        // Called by the bottle's drag handler when dropped on a customer slot.
        public void OnBottleDroppedOnSlot(int slotIndex)
        {
            if (_pendingBottle == null) return;
            if (slotIndex < 0 || slotIndex >= _customers.Length) return;
            if (_customers[slotIndex] == null || !_customers[slotIndex].IsActive) return;

            if (!_isPureBottle)
                GameEventBus.RaiseMistake();
            else
                GameEventBus.RaiseDeliverySuccess();

            _customers[slotIndex].Serve();
            Destroy(_pendingBottle);
            _pendingBottle = null;
            _bottleTimer   = 0f;
        }

        // Called by the bottle's drag handler when dropped on the recycle bin.
        public void OnBottleDroppedOnRecycleBin()
        {
            if (_pendingBottle == null) return;
            Destroy(_pendingBottle);
            _pendingBottle = null;
            _bottleTimer   = 0f;
        }
    }
}
