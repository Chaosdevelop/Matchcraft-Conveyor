using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GMF.Saving
{
    public class JsonDataSerializer : IDataSerializer
    {
        public async Task SerializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData)
        {
            data.OnBeforeSave();
            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metaData.Path, kvp.Key);
                using (MemoryStream ms = new MemoryStream())
                {
                    await JsonSerializer.SerializeAsync(ms, kvp.Value);
                    await storageProvider.SaveAsync(fullPath, ms.ToArray());
                }
            }
            byte[] metaDataBytes = await SaveExtension.SerializeMetaDataAsync(metaData);
            string metaDataPath = Path.Combine(metaData.Path, "metadata.json");
            await storageProvider.SaveAsync(metaDataPath, metaDataBytes);
        }

        public async Task DeserializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData)
        {
            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metaData.Path, kvp.Key);
                byte[] fileData = await storageProvider.LoadAsync(fullPath);
                using (MemoryStream ms = new MemoryStream(fileData))
                {
                    var deserializedData = await JsonSerializer.DeserializeAsync<object>(ms);
                    data.SubData[kvp.Key] = deserializedData;
                }
            }
            data.OnAfterLoad();
        }

    }


}