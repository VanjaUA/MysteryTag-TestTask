#region
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion

namespace MergeGridGame.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private CanvasGroup canvasGroup;

        public Vector2Int Coordinates { get; private set; }
        public bool IsLocked { get; private set; }

        public event Action<ItemView, CellView> OnDragEnded;

        private RectTransform _rectTransform;
        private RectTransform _dragLayer;
        private Camera _uiCamera;
        private bool _isDragging;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void Initialize(RectTransform dragLayer, Camera uiCamera)
        {
            _dragLayer = dragLayer;
            _uiCamera = uiCamera;
        }

        public void SetData(Sprite sprite, Vector2Int coords)
        {
            iconImage.sprite = sprite;
            Coordinates = coords;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsLocked)
            {
                return;
            }

            _isDragging = true;
            canvasGroup.blocksRaycasts = false;

            _ = _rectTransform.DOKill();
            _rectTransform.SetParent(_dragLayer, worldPositionStays: true);
            _ = _rectTransform.DOScale(1.15f, 0.12f).SetEase(Ease.OutQuad);

            UpdateDragPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                return;
            }

            UpdateDragPosition(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                return;
            }

            _isDragging = false;

            CellView targetCell = null;
            GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
            if (hitObject != null)
            {
                targetCell = hitObject.GetComponentInParent<CellView>();
            }

            OnDragEnded?.Invoke(this, targetCell);
        }

        private void UpdateDragPosition(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                _dragLayer, eventData.position, _uiCamera, out Vector3 worldPoint))
            {
                _rectTransform.position = worldPoint;
            }
        }

        public void MoveToCell(CellView targetCell, bool animate = true, Action onComplete = null)
        {
            Coordinates = targetCell.Coordinates;
            _ = _rectTransform.DOKill();

            if (!animate)
            {
                AttachToCell(targetCell);
                onComplete?.Invoke();
                return;
            }

            IsLocked = true;
            _ = _rectTransform.DOMove(targetCell.ItemContainer.position, 0.3f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    AttachToCell(targetCell);
                    IsLocked = false;
                    onComplete?.Invoke();
                });

            _ = _rectTransform.DOScale(Vector3.one, 0.16f);
        }

        private void AttachToCell(CellView cellView)
        {
            _rectTransform.SetParent(cellView.ItemContainer, worldPositionStays: false);
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;
            _rectTransform.localScale = Vector3.one;
            canvasGroup.blocksRaycasts = true;
        }

        public void PlaySpawnAnimation()
        {
            _ = _rectTransform.DOKill();
            IsLocked = true;
            _rectTransform.localScale = Vector3.zero;
            _ = _rectTransform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => IsLocked = false);
        }

        public void PlayMergeAnimation()
        {
            _ = _rectTransform.DOKill();
            IsLocked = true;
            _rectTransform.localScale = Vector3.one;
            _ = _rectTransform.DOPunchScale(new Vector3(0.3f, 0.3f, 0f), 0.22f, vibrato: 8, elasticity: 0.8f)
                .OnComplete(() => IsLocked = false);
        }

        private void OnDestroy()
        {
            _ = _rectTransform.DOKill();
        }
    }
}