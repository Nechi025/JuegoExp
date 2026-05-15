using Core.Services;
using Game;
using UnityEngine;

namespace Game.Panels.Customers
{
    public class CustomerPanel : MonoBehaviour
    {
        [SerializeField] private Customer[]       customers;
        [SerializeField] private DraggableBottle[] bottles;
        [SerializeField] private Transform        recycleBinZone;

        private GameConfig _config;
        private GameState  _gameState = GameState.Playing;
        private float      _spawnTimer;

        private void Start()
        {
            _config = ServiceLocator.Get<GameManager>().Config;
            foreach (var c in customers) c.gameObject.SetActive(false);
            foreach (var b in bottles)  b.gameObject.SetActive(false);
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
            for (int i = 0; i < bottles.Length; i++)
            {
                if (bottles[i].IsActive) continue;
                bottles[i].Activate(this, customers, recycleBinZone, isPure, _config.bottleTimeout);
                Debug.Log($"[CustomerPanel] Bottle {i} activated — isPure: {isPure}");
                return;
            }
            Debug.LogWarning("[CustomerPanel] No inactive bottle available — bottle lost.");
        }

        private void Update()
        {
            if (_gameState != GameState.Playing) return;
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer < _config.customerSpawnInterval) return;
            _spawnTimer = 0f;
            TryShowCustomer();
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

        public void OnBottleDropped(DraggableBottle bottle, int slotIndex)
        {
            if (!bottle.IsActive) return;
            if (slotIndex < 0 || slotIndex >= customers.Length) return;
            if (!customers[slotIndex].IsActive)
            {
                Debug.Log($"[CustomerPanel] Slot {slotIndex} has no active customer — returning bottle");
                return;
            }

            if (!bottle.IsPure)
            {
                Debug.Log($"[CustomerPanel] Tainted bottle delivered to slot {slotIndex} — mistake");
                GameEventBus.RaiseMistake();
            }
            else
            {
                Debug.Log($"[CustomerPanel] Pure bottle delivered to slot {slotIndex} — success");
                GameEventBus.RaiseDeliverySuccess();
            }

            customers[slotIndex].Serve(bottle.IsPure);
            bottle.Deactivate();
        }

        public void OnBottleRecycled(DraggableBottle bottle)
        {
            if (!bottle.IsActive) return;
            Debug.Log("[CustomerPanel] Bottle recycled");
            bottle.Deactivate();
        }

        public void OnBottleTimedOut(DraggableBottle bottle)
        {
            Debug.Log("[CustomerPanel] Bottle timed out — mistake");
            bottle.Deactivate();
            GameEventBus.RaiseMistake();
        }
    }
}
