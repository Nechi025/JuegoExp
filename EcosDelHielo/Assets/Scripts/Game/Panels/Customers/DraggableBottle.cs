using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Panels.Customers
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DraggableBottle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float snapRadius = 150f;

        private CustomerPanel _panel;
        private Transform[]   _slots;
        private Transform     _recycleBin;
        private Canvas        _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup   _canvasGroup;
        private Vector3       _startWorldPos;

        public void Init(CustomerPanel panel, Transform[] slots, Transform recycleBin)
        {
            _panel         = panel;
            _slots         = slots;
            _recycleBin    = recycleBin;
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup   = GetComponent<CanvasGroup>();
            _canvas        = GetComponentInParent<Canvas>();
            _startWorldPos = transform.position;
            Debug.Log("[DraggableBottle] Spawned and ready");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            Debug.Log("[DraggableBottle] Begin drag");
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                eventData.position,
                _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                out var localPoint);
            _rectTransform.localPosition = localPoint;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;

            if (_recycleBin != null)
            {
                float dist = Vector2.Distance(transform.position, _recycleBin.position);
                if (dist <= snapRadius)
                {
                    Debug.Log("[DraggableBottle] Dropped on recycle bin");
                    _panel.OnBottleDroppedOnRecycleBin();
                    return;
                }
            }

            int   closest     = -1;
            float closestDist = snapRadius;
            for (int i = 0; i < _slots.Length; i++)
            {
                float d = Vector2.Distance(transform.position, _slots[i].position);
                if (d < closestDist) { closestDist = d; closest = i; }
            }

            if (closest >= 0)
            {
                Debug.Log($"[DraggableBottle] Dropped on customer slot {closest}");
                _panel.OnBottleDroppedOnSlot(closest);
            }
            else
            {
                Debug.Log("[DraggableBottle] No valid target — returning to spawn");
                transform.position = _startWorldPos;
            }
        }
    }
}
