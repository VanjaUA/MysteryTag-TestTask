namespace MergeGridGame.Services
{
    public interface ISaveService
    {
        void SaveInt(string key, int value);
        int LoadInt(string key, int defaultValue = 0);
        void SaveFloat(string key, float value);
        float LoadFloat(string key, float defaultValue = 0f);
        bool HasKey(string key);
        void DeleteKey(string key);
        void SaveAll();
        void ClearAll();
    }
}