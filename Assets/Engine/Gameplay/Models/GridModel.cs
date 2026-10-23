#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace MergeGridGame.Models
{
    public class GridModel
    {
        public int Columns { get; }
        public int Rows { get; }

        private readonly int[,] _cellsLevel; // 0 level means empty
        private readonly List<Vector2Int> _freeCellsBuffer = new();

        public GridModel(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;
            _cellsLevel = new int[columns, rows];
        }

        public bool IsEmpty(Vector2Int coords) => _cellsLevel[coords.x, coords.y] == 0;

        public int GetItemLevel(Vector2Int coords) => _cellsLevel[coords.x, coords.y];

        public void SetItemLevel(Vector2Int coords, int level)
        {
            _cellsLevel[coords.x, coords.y] = level;
        }

        public bool TryGetRandomFreeCell(out Vector2Int freeCellCoords)
        {
            _freeCellsBuffer.Clear();

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (_cellsLevel[x, y] == 0)
                    {
                        _freeCellsBuffer.Add(new Vector2Int(x, y));
                    }
                }
            }

            if (_freeCellsBuffer.Count == 0)
            {
                freeCellCoords = default;
                return false;
            }

            int randomIndex = Random.Range(0, _freeCellsBuffer.Count);
            freeCellCoords = _freeCellsBuffer[randomIndex];
            return true;
        }
    }
}