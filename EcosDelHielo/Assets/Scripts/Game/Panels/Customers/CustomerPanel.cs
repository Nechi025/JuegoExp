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

        private void Awake()  => _customers = new Customer[customerSlots.Length];
        private void Start()  => _config    = ServiceLocator.Get<GameManager>().Config;

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
            _pendingBottle = Instantiate(bottlePrefab, bottleSpawnPoint.position, Quaternion.identity,
                                         bottleSpawnPoint.parent);

            var draggable = _pendingBottle.GetComponent<DraggableBottle>()
                         ?? _pendingBottle.AddComponent<DraggableBottle>();
            draggable.Init(this, customerSlots, recycleBinZone);
            Debug.Log($"[CustomerPanel] Bottle spawned — isPure: {isPure}");
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
            Debug.Log("[CustomerPanel] Bottle timed out — mistake");
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
            Debug.Log($"[CustomerPanel] Customer spawned at slot {slotIndex}");
        }

        public void OnBottleDroppedOnSlot(int slotIndex)
        {
            if (_pendingBottle == null) return;
            if (slotIndex < 0 || slotIndex >= _customers.Length) return;
            if (_customers[slotIndex] == null || !_customers[slotIndex].IsActive)
            {
                Debug.Log($"[CustomerPanel] Slot {slotIndex} has no active customer — returning bottle");
                return;
            }

            if (!_isPureBottle)
            {
                Debug.Log($"[CustomerPanel] Tainted bottle delivered to slot {slotIndex} — mistake");
                GameEventBus.RaiseMistake();
            }
            else
            {
                Debug.Log($"[CustomerPanel] Pure bottle delivered to slot {slotIndex} — success");
                GameEventBus.RaiseDeliverySuccess();
            }

            _customers[slotIndex].Serve();
            Destroy(_pendingBottle);
            _pendingBottle = null;
            _bottleTimer   = 0f;
        }

        public void OnBottleDroppedOnRecycleBin()
        {
            if (_pendingBottle == null) return;
            Debug.Log("[CustomerPanel] Bottle recycled");
            Destroy(_pendingBottle);
            _pendingBottle = null;
            _bottleTimer   = 0f;
        }
    }
}
