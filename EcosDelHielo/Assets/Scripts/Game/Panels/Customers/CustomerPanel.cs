using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Customers
{
    public class CustomerPanel : MonoBehaviour
    {
        [SerializeField] private Customer[]      customers;
        [SerializeField] private DraggableBottle bottle;
        [SerializeField] private Transform       recycleBinZone;

        private GameConfig _config;
        private bool       _isPureBottle;
        private GameState  _gameState = GameState.Playing;
        private float      _spawnTimer;
        private float      _bottleTimer;

        private void Start()
        {
            _config = ServiceLocator.Get<GameManager>().Config;
            foreach (var c in customers) c.gameObject.SetActive(false);
            bottle.gameObject.SetActive(false);
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
            if (bottle.IsActive) return;
            _isPureBottle = isPure;
            bottle.Activate(this, customers, recycleBinZone);
            Debug.Log($"[CustomerPanel] Bottle activated — isPure: {isPure}");
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
            TryShowCustomer();
        }

        private void TickBottleTimeout(float deltaTime)
        {
            if (!bottle.IsActive) { _bottleTimer = 0f; return; }
            _bottleTimer += deltaTime;
            if (_bottleTimer < _config.bottleTimeout) return;
            _bottleTimer = 0f;
            Debug.Log("[CustomerPanel] Bottle timed out — mistake");
            bottle.Deactivate();
            GameEventBus.RaiseMistake();
        }

        private void TryShowCustomer()
        {
            for (int i = 0; i < customers.Length; i++)
            {
                if (customers[i].IsActive) continue;
                customers[i].Show();
                Debug.Log($"[CustomerPanel] Customer shown at slot {i}");
                return;
            }
        }

        public void OnBottleDroppedOnSlot(int slotIndex)
        {
            if (!bottle.IsActive) return;
            if (slotIndex < 0 || slotIndex >= customers.Length) return;
            if (!customers[slotIndex].IsActive)
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

            customers[slotIndex].Serve(_isPureBottle);
            bottle.Deactivate();
            _bottleTimer = 0f;
        }

        public void OnBottleDroppedOnRecycleBin()
        {
            if (!bottle.IsActive) return;
            Debug.Log("[CustomerPanel] Bottle recycled");
            bottle.Deactivate();
            _bottleTimer = 0f;
        }
    }
}
