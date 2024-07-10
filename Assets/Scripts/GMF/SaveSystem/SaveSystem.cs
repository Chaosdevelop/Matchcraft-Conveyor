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
    }

    public interface ISaveMetaData
    {
        string Name { get; }
        string Path { get; }
        DateTime Date { get; }
        string Version { get; }

    }

    // Интерфейс для обеспечения сериализации данных
    public interface IDataSerializer
    {
        Task SerializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData);
        Task DeserializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData);
    }

    // Интерфейс для хранения данных
    public interface ISaveData
    {
        public Dictionary<string, object> SubData { get; }
        void OnBeforeSave();
        void OnAfterLoad();
    }


    public interface IManualSaveLoadManager : ISaveLoadManager
    {
        Task DeleteSaveSlotAsync(ISaveMetaData saveSlot);
    }

    public interface IAutoSaveLoadManager : ISaveLoadManager
    {
        Task AutoSaveAsync();
        Task AutoLoadAsync();
    }


    [Serializable]
    public class ExampleSaveData : ISaveData
    {

        public Dictionary<string, object> SubData => new Dictionary<string, object>() { { "Main.txt", 123 } };
        public void OnBeforeSave() { }
        public void OnAfterLoad() { }


    }

    [Serializable]
    public struct ExampleSaveMetaData : ISaveMetaData
    {
        public string Name { get; }
        public string Path { get; set; }
        public DateTime Date { get; }
        public string Version { get; }

    }
    public static class SaveExtension
    {
        public static async Task<byte[]> SerializeMetaDataAsync(ISaveMetaData metaData)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(ms, metaData);
                return ms.ToArray();
            }
        }
    }

    public class SaveLoadManager : IAutoSaveLoadManager
    {
        private readonly IDataSerializer serializer;
        private readonly IStorageProvider storageProvider;
        private readonly List<ISaveMetaData> saves = new List<ISaveMetaData>();
        private readonly ISaveData saveData;

        public SaveLoadManager(IDataSerializer serializer, IStorageProvider storageProvider)
        {
            this.serializer = serializer;
            this.storageProvider = storageProvider;
        }

        public async Task SaveAsync(ISaveMetaData saveSlot)
        {
            var state = new ExampleSaveData();
            await serializer.SerializeAsync(state, storageProvider, saveSlot);
            saves.Add(saveSlot);
        }

        public async Task LoadAsync(ISaveMetaData saveSlot)
        {
            var state = new ExampleSaveData();
            await serializer.DeserializeAsync(state, storageProvider, saveSlot);
            // Обработка состояния игры после загрузки
        }

        public Task<IEnumerable<ISaveMetaData>> GetAvailableSaves()
        {
            return Task.FromResult<IEnumerable<ISaveMetaData>>(saves);
        }

        public async Task AutoSaveAsync()
        {
            await SaveAsync(new ExampleSaveMetaData { Path = "" });

        }

        public async Task AutoLoadAsync()
        {
            await LoadAsync(new ExampleSaveMetaData { Path = "" });
        }
    }
}