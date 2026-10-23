#region

using MergeGridGame.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace MergeGridGame.ScriptableObjects
{
    [System.Serializable]
    public class PrefabEntry
    {
        public string Key;
        public GameObject Prefab;
    }

    [CreateAssetMenu(fileName = "PrefabRegistry", menuName = "ScriptableObjects/PrefabRegistry")]
    public class PrefabRegistry : ScriptableObject
    {
        [Header("Core Infrastructure")]
        public Camera MainCameraPrefab;
        public EventSystem EventSystemPrefab;
        public UIRoot UIRootPrefab;

        [Header("Configs")]
        public AllLevelsConfigSO AllLevelsConfig;

        public List<PrefabEntry> Entries;

        private Dictionary<string, GameObject> _map;

        public GameObject Get(string key)
        {
            _map ??= Entries.ToDictionary(e => e.Key, e => e.Prefab);
            if (!_map.ContainsKey(key))
            {
                Debug.LogError($"Prefab registry does not contain key: {key}");
                return null;
            }
            return _map[key];
        }
    }
}
