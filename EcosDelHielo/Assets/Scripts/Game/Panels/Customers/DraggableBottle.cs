using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Panels.Customers
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DraggableBottle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private float      snapRadius = 150f;
        [SerializeField] private Image      bottleImage;
        [SerializeField] private Sprite     purifiedSprite;
        [SerializeField] private Sprite     contaminatedSprite;
        [SerializeField] private GameObject dirtyWaterLayer;

        private CustomerPanel _panel;
        private Customer[]    _customers;
        private Transform     _recycleBin;
        private Canvas        _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup   _canvasGroup;
        private Transform     _homeParent;
        private Vector2       _homeAnchoredPos;
        private bool          _activated;
        private float         _timer;
        private float         _timeout;

        public bool IsPure   { get; private set; }
        public bool IsActive => _activated;

        private void Awake()
        {
            _rectTransform   = GetComponent<RectTransform>();
            _canvasGroup     = GetComponent<CanvasGroup>();
            _canvas          = GetComponentInParent<Canvas>(true);
            _homeParent      = transform.parent;
            _homeAnchoredPos = _rectTransform.anchoredPosition;
            gameObject.SetActive(false);
        }

        public void Activate(CustomerPanel panel, Customer[] customers, Transform recycleBin, bool isPure, float timeout)
        {
            _panel      = panel;
            _customers  = customers;
            _recycleBin = recycleBin;
            IsPure      = isPure;
            _timeout    = timeout;
            _timer      = 0f;
            _activated  = true;
            _canvasGroup.alpha = 0f;

            if (bottleImage != null)
                bottleImage.sprite = isPure ? purifiedSprite : contaminatedSprite;
            if (dirtyWaterLayer != null)
                dirtyWaterLayer.SetActive(!isPure);

            transform.SetParent(_homeParent, false);
            _rectTransform.anchoredPosition = _homeAnchoredPos;
            transform.SetParent(_canvas.transform, true);

            gameObject.SetActive(true);
            DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 1f, 0.2f);
        }

        public void Deactivate()
        {
            _activated = false;
            DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, 0f, 0.2f)
                   .OnComplete(() =>
                   {
                       transform.SetParent(_homeParent, false);
                       _rectTransform.anchoredPosition = _homeAnchoredPos;
                       gameObject.SetActive(false);
                   });
        }

        private void Update()
        {
            if (!_activated) return;
            _timer += Time.deltaTime;
            if (_timer < _timeout) return;
            _activated = false;
            _panel.OnBottleTimedOut(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;

            Camera  cam     = _canvas.worldCamera;
            Vector2 dropPos = eventData.position;

            if (_recycleBin != null)
            {
                Vector2 binScreen = RectTransformUtility.WorldToScreenPoint(cam, _recycleBin.position);
                if (Vector2.Distance(dropPos, binScreen) <= snapRadius)
                {
                    _panel.OnBottleRecycled(this);
                    return;
                }
            }

            int   closest     = -1;
            float closestDist = snapRadius;
            for (int i = 0; i < _customers.Length; i++)
            {
                Vector2 custScreen = RectTransformUtility.WorldToScreenPoint(cam, _customers[i].transform.position);
                float d = Vector2.Distance(dropPos, custScreen);
                if (d < closestDist) { closestDist = d; closest = i; }
            }

            if (closest >= 0)
            {
                _panel.OnBottleDropped(this, closest);
            }
            else
            {
                transform.SetParent(_homeParent, false);
                _rectTransform.anchoredPosition = _homeAnchoredPos;
                transform.SetParent(_canvas.transform, true);
            }
        }
    }
}
