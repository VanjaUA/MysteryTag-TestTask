#region

using UnityEngine;

#endregion

namespace MergeGridGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig")]
    public class LevelConfigSO : ScriptableObject
    {
        [Header("Grid Settings")]
        [SerializeField] private int columns = 5;
        [SerializeField] private int rows = 7;

        [Header("Progression")]
        [SerializeField] private ItemsChainConfigSO itemsChain;
        [SerializeField] private int targetLevel = 6;

        public int Columns => columns;
        public int Rows => rows;
        public ItemsChainConfigSO ItemsChain => itemsChain;
        public int TargetLevel => Mathf.Min(targetLevel, itemsChain != null ? itemsChain.MaxLevel : targetLevel);
    }
}