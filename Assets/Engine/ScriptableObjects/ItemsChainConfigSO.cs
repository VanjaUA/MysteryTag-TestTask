#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace MergeGridGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemsChainConfig", menuName = "ScriptableObjects/ItemsChainConfig")]
    public class ItemsChainConfigSO : ScriptableObject
    {
        [SerializeField] private List<Sprite> itemSprites = new();

        public int MaxLevel => itemSprites.Count;

        public Sprite GetSpriteForLevel(int level)
        {
            if (itemSprites.Count == 0)
            {
                Debug.LogError("Item sprites list count is 0!");
                return null;
            }
            int index = Mathf.Clamp(level - 1, 0, itemSprites.Count - 1);
            return itemSprites[index];
        }
    }
}