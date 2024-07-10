using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace GMF.Saving
{
    public class BinaryDataSerializer : IDataSerializer
    {
        public async Task SerializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData)
        {
            data.OnBeforeSave();
            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metaData.Path, kvp.Key);
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    await Task.Run(() => formatter.Serialize(ms, kvp.Value));
                    await storageProvider.SaveAsync(fullPath, ms.ToArray());
                }
            }
        }

        public async Task DeserializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData)
        {
            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metaData.Path, kvp.Key);
                byte[] fileData = await storageProvider.LoadAsync(fullPath);
                using (MemoryStream ms = new MemoryStream(fileData))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    var deserializedData = await Task.Run(() => formatter.Deserialize(ms));
                    data.SubData[kvp.Key] = deserializedData;
                }
            }
            data.OnAfterLoad();
        }
    }


}