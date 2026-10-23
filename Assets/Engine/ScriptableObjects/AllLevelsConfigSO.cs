#region
using System.Collections.Generic;
using UnityEngine;
#endregion

namespace MergeGridGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AllLevelsConfig", menuName = "ScriptableObjects/AllLevelsConfig")]
    public class AllLevelsConfigSO : ScriptableObject
    {
        [SerializeField] private List<LevelConfigSO> levels = new();

        public int LevelsCount => levels.Count;

        public LevelConfigSO GetLevel(int index)
        {
            if (levels == null || levels.Count == 0)
            {
                Debug.LogError("AllLevelsConfigSO has no levels assigned!");
                return null;
            }

            int safeIndex = Mathf.Clamp(index, 0, levels.Count - 1);
            return levels[safeIndex];
        }
    }
}