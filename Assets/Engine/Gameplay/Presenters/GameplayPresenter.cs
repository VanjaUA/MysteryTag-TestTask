#region
using MergeGridGame.Models;
using MergeGridGame.ScriptableObjects;
using MergeGridGame.UI;
using UnityEngine;
#endregion

namespace MergeGridGame.Presenters
{
    public class GameplayPresenter
    {
        private readonly GridModel _gridModel;
        private readonly GameplayScreenView _screenView;
        private readonly LevelConfigSO _levelConfig;
        private readonly PrefabRegistry _prefabRegistry;
        private readonly UIRoot _uiRoot;

        private CellView[,] _cellViews;
        private ItemView[,] _itemViews;

        public GameplayPresenter(
            GridModel gridModel,
            GameplayScreenView screenView,
            LevelConfigSO levelConfig,
            PrefabRegistry prefabRegistry,
            UIRoot uiRoot)
        {
            _gridModel = gridModel;
            _screenView = screenView;
            _levelConfig = levelConfig;
            _prefabRegistry = prefabRegistry;
            _uiRoot = uiRoot;
        }

        public void Initialize()
        {
            _screenView.ConfigureGrid(_gridModel.Columns, _gridModel.Rows);
            BuildGridCells();

            _screenView.OnSpawnClicked += TrySpawnLevelOneItem;
        }

        public void Dispose()
        {
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

            if (targetCell == null || targetCell.Coordinates == sourceCoords)
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
                Debug.LogError("WIN");
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
    }
}