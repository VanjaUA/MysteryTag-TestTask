#region
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#endregion

namespace MergeGridGame.Services
{
    public class SaveService : ISaveService
    {
        [Serializable]
        private struct IntEntry
        {
            public string Key;
            public int Value;
        }

        [Serializable]
        private struct FloatEntry
        {
            public string Key;
            public float Value;
        }

        [Serializable]
        private class SaveDataContainer
        {
            public List<IntEntry> IntEntries = new();
            public List<FloatEntry> FloatEntries = new();
        }

        private readonly string _filePath;
        private readonly Dictionary<string, int> _intData = new();
        private readonly Dictionary<string, float> _floatData = new();

        public SaveService()
        {
            _filePath = Path.Combine(Application.persistentDataPath, "save_data.json");
            LoadFromFile();
        }

        public void SaveInt(string key, int value)
        {
            _intData[key] = value;
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return _intData.TryGetValue(key, out int value) ? value : defaultValue;
        }

        public void SaveFloat(string key, float value)
        {
            _floatData[key] = value;
        }

        public float LoadFloat(string key, float defaultValue = 0f)
        {
            return _floatData.TryGetValue(key, out float value) ? value : defaultValue;
        }

        public bool HasKey(string key)
        {
            return _intData.ContainsKey(key) || _floatData.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            _ = _intData.Remove(key);
            _ = _floatData.Remove(key);
        }

        public void SaveAll()
        {
            SaveDataContainer container = new();

            foreach (KeyValuePair<string, int> kvp in _intData)
            {
                container.IntEntries.Add(new IntEntry { Key = kvp.Key, Value = kvp.Value });
            }

            foreach (KeyValuePair<string, float> kvp in _floatData)
            {
                container.FloatEntries.Add(new FloatEntry { Key = kvp.Key, Value = kvp.Value });
            }

            string json = JsonUtility.ToJson(container, prettyPrint: true);
            File.WriteAllText(_filePath, json);
        }

        public void ClearAll()
        {
            _intData.Clear();
            _floatData.Clear();

            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        private void LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                SaveDataContainer container = JsonUtility.FromJson<SaveDataContainer>(json);
                if (container == null)
                {
                    return;
                }

                _intData.Clear();
                foreach (IntEntry entry in container.IntEntries)
                {
                    _intData[entry.Key] = entry.Value;
                }

                _floatData.Clear();
                foreach (FloatEntry entry in container.FloatEntries)
                {
                    _floatData[entry.Key] = entry.Value;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load save file: {e.Message}");
            }
        }
    }
}