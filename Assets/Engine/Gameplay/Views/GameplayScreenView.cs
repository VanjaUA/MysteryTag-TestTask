#region

using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace MergeGridGame.UI
{
    public class GameplayScreenView : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private RectTransform gridContainer;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Controls")]
        [SerializeField] private Button spawnButton;

        public RectTransform GridContainer => gridContainer;
        public event Action OnSpawnClicked;

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

        public void ConfigureGrid(int columns, int rows)
        {
            Canvas.ForceUpdateCanvases();

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;

            Rect rect = gridContainer.rect;
            float availableWidth = rect.width - gridLayout.padding.left - gridLayout.padding.right - gridLayout.spacing.x * (columns - 1);
            float availableHeight = rect.height - gridLayout.padding.top - gridLayout.padding.bottom - gridLayout.spacing.y * (rows - 1);

            float cellSize = Mathf.Floor(Mathf.Min(availableWidth / columns, availableHeight / rows));
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
        }

        public void PlayGeneratorShakeAnimation()
        {
            _ = spawnButton.transform.DOKill(complete: true);
            _ = spawnButton.transform.DOShakePosition(0.2f, strength: 15f, vibrato: 20);
        }
    }
}