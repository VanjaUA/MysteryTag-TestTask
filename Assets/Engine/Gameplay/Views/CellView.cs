#region

using UnityEngine;

#endregion

namespace MergeGridGame.UI
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private RectTransform itemContainer;

        public Vector2Int Coordinates { get; private set; }
        public RectTransform ItemContainer => itemContainer;

        public void Initialize(Vector2Int coordinates)
        {
            Coordinates = coordinates;
            gameObject.name = $"Cell_{coordinates.x}_{coordinates.y}";
        }
    }
}