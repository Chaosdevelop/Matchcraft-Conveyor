using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GMF.Saving
{
    public interface IStorageProvider
    {
        Task SaveAsync(string path, byte[] data);
        Task<byte[]> LoadAsync(string path);
        Task DeleteAsync(string path);
        Task<IEnumerable<string>> GetFilesAsync(string directoryPath);
    }

    public interface ISaveLoadManager
    {
        Task SaveAsync(ISaveMetaData saveSlot);
        Task LoadAsync(ISaveMetaData saveSlot);
        Task<IEnumerable<ISaveMetaData>> GetAvailableSaves();
        Task DeleteSaveSlotAsync(ISaveMetaData saveSlot);
        Task AutoLoadAsync();
        Action<ISaveData, ISaveMetaData> OnSaveLoaded { get; set; }
        void Initialize();
    }

    public interface ISaveMetaData
    {
        string Name { get; }
        string Path { get; }
        DateTime Date { get; }
        string Version { get; }

        public async Task<byte[]> SerializeAsync()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(ms, this);
                return ms.ToArray();
            }
        }
    }

    // Интерфейс для обеспечения сериализации данных
    public interface IDataSerializer
    {
        Task SerializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData);
        Task<ISaveData> DeserializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData);
    }

    // Интерфейс для хранения данных
    public interface ISaveData
    {
        public Dictionary<string, object> SubData { get; }
        void OnBeforeSave();
        void OnAfterLoad();
    }



    [Serializable]
    public struct SimpleSaveMetaData : ISaveMetaData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime Date { get; set; }
        public string Version { get; set; }

    }



    public static class SaveSystem
    {

        static ISaveData CurrentSaveData { get; set; }
        public static ISaveMetaData CurrentSaveSlot { get; private set; }
        public static ISaveLoadManager SaveLoadManager { get; private set; }

        public static Action<ISaveData, ISaveMetaData> OnSaveLoaded { get; set; }

        public static void Initialize(ISaveLoadManager saveLoadManager)
        {
            SaveLoadManager = saveLoadManager;
            saveLoadManager.OnSaveLoaded += SaveLoaded;
        }

        static void SaveLoaded(ISaveData saveData, ISaveMetaData saveMetaData)
        {
            CurrentSaveData = saveData;
            CurrentSaveSlot = saveMetaData;
            OnSaveLoaded?.Invoke(saveData, saveMetaData);
        }
        public static T GetCurrentSave<T>() where T : class, ISaveData
        {
            return CurrentSaveData as T;
        }


        public static Task SaveCurrentAsync()
        {

            return SaveLoadManager.SaveAsync(CurrentSaveSlot);

        }

        public static void SaveCurrent()
        {

            //UnityEngine.Debug.Log($"SaveCurrent {status}");
            SaveCurrentAsync().GetAwaiter().GetResult();

        }

        public static Task TryAutoLoadAsync()
        {
            return SaveLoadManager.AutoLoadAsync();
        }
    }
}