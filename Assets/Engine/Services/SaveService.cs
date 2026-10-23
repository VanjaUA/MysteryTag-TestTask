#region

using UnityEngine;

#endregion

namespace MergeGridGame.Services
{
    public class SaveService : ISaveService
    {
        public void SaveInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public int LoadInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public void SaveAll()
        {
            PlayerPrefs.Save();
        }
    }
}
