#region
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace MergeGridGame.UI
{
    public class GameplayScreenView : MonoBehaviour
    {
        [Header("Header UI")]
        [SerializeField] private TMP_Text levelTitleText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject bestTimeContainer;
        [SerializeField] private TMP_Text bestTimeText;
        [SerializeField] private Image targetItemIcon;

        [Header("Grid")]
        [SerializeField] private RectTransform gridContainer;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Controls")]
        [SerializeField] private Button spawnButton;

        public RectTransform GridContainer => gridContainer;
        public event Action OnSpawnClicked;

        private int _columns;
        private int _rows;

        private void Awake()
        {
            spawnButton.onClick.AddListener(HandleSpawnButtonClicked);
        }

        private void OnDestroy()
        {
            spawnButton.onClick.RemoveListener(HandleSpawnButtonClicked);
            _ = spawnButton.transform.DOKill();
        }

        private void HandleSpawnButtonClicked()
        {
            OnSpawnClicked?.Invoke();
        }

        public void SetupHeader(int levelNumber, Sprite targetSprite, bool hasBestTime, string bestTimeFormatted)
        {
            levelTitleText.text = $"Level {levelNumber}";
            targetItemIcon.sprite = targetSprite;
            SetTimerText("00:00");

            if (bestTimeContainer != null)
            {
                bestTimeContainer.SetActive(hasBestTime);
            }

            if (hasBestTime && bestTimeText != null)
            {
                bestTimeText.text = $"Best: {bestTimeFormatted}";
            }
        }

        public void SetTimerText(string formattedTime)
        {
            timerText.text = formattedTime;
        }

        public void UpdateBestTimeDisplay(string bestTimeFormatted)
        {
            if (bestTimeContainer != null)
            {
                bestTimeContainer.SetActive(true);
            }

            if (bestTimeText != null)
            {
                bestTimeText.text = $"Best: {bestTimeFormatted}";
            }
        }

        public void ConfigureGrid(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;
            RecalculateCellSize();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (_columns > 0 && _rows > 0 && gameObject.activeInHierarchy)
            {
                RecalculateCellSize();
            }
        }

        private void RecalculateCellSize()
        {
            Canvas.ForceUpdateCanvases();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = _columns;

            Rect rect = gridContainer.rect;
            float availableWidth = rect.width - gridLayout.padding.left - gridLayout.padding.right - gridLayout.spacing.x * (_columns - 1);
            float availableHeight = rect.height - gridLayout.padding.top - gridLayout.padding.bottom - gridLayout.spacing.y * (_rows - 1);

            float cellSize = Mathf.Floor(Mathf.Min(availableWidth / _columns, availableHeight / _rows));
            if (cellSize > 0)
            {
                gridLayout.cellSize = new Vector2(cellSize, cellSize);
            }
        }

        public void PlayGeneratorShakeAnimation()
        {
            _ = spawnButton.transform.DOKill(complete: true);
            _ = spawnButton.transform.DOShakePosition(0.2f, strength: 15f, vibrato: 20);
        }
    }
}