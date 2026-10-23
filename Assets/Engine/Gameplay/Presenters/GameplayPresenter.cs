#region
using DG.Tweening;
using MergeGridGame.Models;
using MergeGridGame.ScriptableObjects;
using MergeGridGame.Services;
using MergeGridGame.UI;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
#endregion

namespace MergeGridGame.Presenters
{
    public class GameplayPresenter
    {
        private const string CurrentLevelSaveKey = "CurrentLevelIndex";

        private readonly GridModel _gridModel;
        private readonly GameplayScreenView _screenView;
        private readonly LevelConfigSO _levelConfig;
        private readonly int _currentLevelIndex;
        private readonly int _totalLevelsCount;
        private readonly PrefabRegistry _prefabRegistry;
        private readonly UIRoot _uiRoot;
        private readonly ISaveService _saveService;

        private CellView[,] _cellViews;
        private ItemView[,] _itemViews;

        private bool _isTimerRunning;
        private bool _isLevelCompleted;
        private float _elapsedTime;
        private int _lastDisplayedSecond = -1;
        private Tween _victoryDelayTween;

        public event Action<string, string> OnLevelCompleted;

        private int DisplayLevelNumber => _currentLevelIndex + 1;
        private string BestTimeSaveKey => $"BestTime_LevelIndex_{_currentLevelIndex}";

        public GameplayPresenter(
            GridModel gridModel,
            GameplayScreenView screenView,
            LevelConfigSO levelConfig,
            int currentLevelIndex,
            int totalLevelsCount,
            PrefabRegistry prefabRegistry,
            UIRoot uiRoot,
            ISaveService saveService)
        {
            _gridModel = gridModel;
            _screenView = screenView;
            _levelConfig = levelConfig;
            _currentLevelIndex = currentLevelIndex;
            _totalLevelsCount = totalLevelsCount;
            _prefabRegistry = prefabRegistry;
            _uiRoot = uiRoot;
            _saveService = saveService;
        }

        public void Initialize()
        {
            InitializeHeader();
            _screenView.ConfigureGrid(_gridModel.Columns, _gridModel.Rows);
            BuildGridCells();

            _screenView.OnSpawnClicked += TrySpawnLevelOneItem;
        }

        public void Update(float deltaTime)
        {
            if (!_isTimerRunning || _isLevelCompleted)
            {
                return;
            }

            _elapsedTime += deltaTime;
            int currentSecond = Mathf.FloorToInt(_elapsedTime);

            if (currentSecond != _lastDisplayedSecond)
            {
                _lastDisplayedSecond = currentSecond;
                _screenView.SetTimerText(FormatTime(_elapsedTime));
            }
        }

        public void Dispose()
        {
            _victoryDelayTween?.Kill();
            _screenView.OnSpawnClicked -= TrySpawnLevelOneItem;

            if (_itemViews == null)
            {
                return;
            }

            foreach (ItemView item in _itemViews)
            {
                if (item != null)
                {
                    item.OnDragEnded -= HandleItemDragEnded;
                }
            }
        }

        private void InitializeHeader()
        {
            Sprite targetSprite = _levelConfig.ItemsChain.GetSpriteForLevel(_levelConfig.TargetLevel);
            bool hasBestTime = _saveService.HasKey(BestTimeSaveKey);
            string bestTimeFormatted = hasBestTime
                ? FormatTime(_saveService.LoadFloat(BestTimeSaveKey))
                : "--:--";

            _screenView.SetupHeader(DisplayLevelNumber, targetSprite, hasBestTime, bestTimeFormatted);
        }

        private void BuildGridCells()
        {
            _cellViews = new CellView[_gridModel.Columns, _gridModel.Rows];
            _itemViews = new ItemView[_gridModel.Columns, _gridModel.Rows];

            GameObject cellPrefab = _prefabRegistry.Get("CellView");

            for (int y = 0; y < _gridModel.Rows; y++)
            {
                for (int x = 0; x < _gridModel.Columns; x++)
                {
                    GameObject cellObj = Object.Instantiate(cellPrefab, _screenView.GridContainer, false);
                    CellView cell = cellObj.GetComponent<CellView>();

                    Vector2Int coords = new(x, y);
                    cell.Initialize(coords);
                    _cellViews[x, y] = cell;
                }
            }
        }

        private void TrySpawnLevelOneItem()
        {
            if (_isLevelCompleted)
            {
                return;
            }

            if (!_gridModel.TryGetRandomFreeCell(out Vector2Int freeCoords))
            {
                _screenView.PlayGeneratorShakeAnimation();
                return;
            }

            const int startItemLevel = 1;
            _gridModel.SetItemLevel(freeCoords, startItemLevel);

            CellView targetCell = _cellViews[freeCoords.x, freeCoords.y];
            GameObject itemPrefab = _prefabRegistry.Get("ItemView");

            GameObject itemObj = Object.Instantiate(itemPrefab, targetCell.ItemContainer, false);
            ItemView newItem = itemObj.GetComponent<ItemView>();

            newItem.Initialize(_uiRoot.DragLayer, _uiRoot.UICamera);
            Sprite levelOneSprite = _levelConfig.ItemsChain.GetSpriteForLevel(startItemLevel);
            newItem.SetData(levelOneSprite, freeCoords);
            newItem.OnDragEnded += HandleItemDragEnded;
            newItem.PlaySpawnAnimation();

            _itemViews[freeCoords.x, freeCoords.y] = newItem;
        }

        private void HandleItemDragEnded(ItemView draggedItem, CellView targetCell)
        {
            Vector2Int sourceCoords = draggedItem.Coordinates;
            CellView sourceCell = _cellViews[sourceCoords.x, sourceCoords.y];

            if (_isLevelCompleted || targetCell == null || targetCell.Coordinates == sourceCoords)
            {
                draggedItem.MoveToCell(sourceCell, animate: true);
                return;
            }

            Vector2Int targetCoords = targetCell.Coordinates;

            if (_gridModel.IsEmpty(targetCoords))
            {
                int itemLevel = _gridModel.GetItemLevel(sourceCoords);
                _gridModel.SetItemLevel(sourceCoords, 0);
                _gridModel.SetItemLevel(targetCoords, itemLevel);

                _itemViews[sourceCoords.x, sourceCoords.y] = null;
                _itemViews[targetCoords.x, targetCoords.y] = draggedItem;

                draggedItem.MoveToCell(targetCell, animate: true);
                return;
            }

            ItemView targetItem = _itemViews[targetCoords.x, targetCoords.y];

            if (targetItem != null && targetItem.IsLocked)
            {
                draggedItem.MoveToCell(sourceCell, animate: true);
                return;
            }

            int sourceLevel = _gridModel.GetItemLevel(sourceCoords);
            int targetLevel = _gridModel.GetItemLevel(targetCoords);

            if (sourceLevel == targetLevel && targetLevel < _levelConfig.ItemsChain.MaxLevel)
            {
                PerformMerge(draggedItem, targetItem, sourceCoords, targetCoords, targetLevel + 1);
                return;
            }

            PerformSwap(draggedItem, targetItem, sourceCell, targetCell, sourceLevel, targetLevel);
        }

        private void PerformMerge(
            ItemView draggedItem,
            ItemView targetItem,
            Vector2Int sourceCoords,
            Vector2Int targetCoords,
            int newLevel)
        {
            if (!_isTimerRunning && !_isLevelCompleted)
            {
                _isTimerRunning = true;
            }

            _gridModel.SetItemLevel(sourceCoords, 0);
            _gridModel.SetItemLevel(targetCoords, newLevel);
            _itemViews[sourceCoords.x, sourceCoords.y] = null;

            draggedItem.OnDragEnded -= HandleItemDragEnded;
            Object.Destroy(draggedItem.gameObject);

            Sprite nextLevelSprite = _levelConfig.ItemsChain.GetSpriteForLevel(newLevel);
            targetItem.SetData(nextLevelSprite, targetCoords);
            targetItem.PlayMergeAnimation();

            if (newLevel >= _levelConfig.TargetLevel)
            {
                CompleteLevel();
            }
        }

        private void PerformSwap(
            ItemView draggedItem,
            ItemView targetItem,
            CellView sourceCell,
            CellView targetCell,
            int sourceLevel,
            int targetLevel)
        {
            Vector2Int sourceCoords = sourceCell.Coordinates;
            Vector2Int targetCoords = targetCell.Coordinates;

            _gridModel.SetItemLevel(sourceCoords, targetLevel);
            _gridModel.SetItemLevel(targetCoords, sourceLevel);

            _itemViews[sourceCoords.x, sourceCoords.y] = targetItem;
            _itemViews[targetCoords.x, targetCoords.y] = draggedItem;

            draggedItem.MoveToCell(targetCell, animate: true);
            targetItem.MoveToCell(sourceCell, animate: true);
        }

        private void CompleteLevel()
        {
            _isTimerRunning = false;
            _isLevelCompleted = true;

            string currentTimeFormatted = FormatTime(_elapsedTime);
            _screenView.SetTimerText(currentTimeFormatted);

            bool hasPreviousBest = _saveService.HasKey(BestTimeSaveKey);
            float bestTime = _saveService.LoadFloat(BestTimeSaveKey, float.MaxValue);

            if (!hasPreviousBest || _elapsedTime < bestTime)
            {
                bestTime = _elapsedTime;
                _saveService.SaveFloat(BestTimeSaveKey, bestTime);
                _screenView.UpdateBestTimeDisplay(FormatTime(bestTime));
            }

            int nextLevelIndex = (_currentLevelIndex + 1) % _totalLevelsCount;
            _saveService.SaveInt(CurrentLevelSaveKey, nextLevelIndex);
            _saveService.SaveAll();

            string bestTimeFormatted = FormatTime(bestTime);

            _victoryDelayTween = DOVirtual.DelayedCall(0.35f, () =>
            {
                OnLevelCompleted?.Invoke(currentTimeFormatted, bestTimeFormatted);
            });
        }

        private static string FormatTime(float timeInSeconds)
        {
            int totalSeconds = Mathf.FloorToInt(timeInSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes:00}:{seconds:00}";
        }
    }
}