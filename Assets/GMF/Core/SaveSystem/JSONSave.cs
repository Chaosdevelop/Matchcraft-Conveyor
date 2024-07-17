using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace GMF.Saving
{
    public class UnityJsonDataSerializer : IDataSerializer
    {

        public async Task SerializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData)
        {

            data.OnBeforeSave();
            {
                string fullPath = Path.Combine(metaData.Path, "Save");

                var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
                await storageProvider.SaveAsync(fullPath, bytes);
            }

            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metaData.Path, "Sub", kvp.Key);
                var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(kvp.Value));
                await storageProvider.SaveAsync(fullPath, bytes);
            }
            byte[] metaDataBytes = await metaData.SerializeAsync();
            string metaDataPath = Path.Combine(metaData.Path, "metadata.json");
            await storageProvider.SaveAsync(metaDataPath, metaDataBytes);
        }

        public async Task<ISaveData> DeserializeAsync(ISaveData data, IStorageProvider storageProvider, ISaveMetaData metaData)
        {

            ISaveData deserialized = data;
            {
                string fullPath = Path.Combine(metaData.Path, "Save");
                byte[] fileData = await storageProvider.LoadAsync(fullPath);
                if (fileData.Length > 0)
                {
                    var jsonString = Encoding.UTF8.GetString(fileData);
                    JsonUtility.FromJsonOverwrite(jsonString, data);
                    deserialized = data;

                }
            }

            foreach (var kvp in data.SubData)
            {
                string fullPath = Path.Combine(metaData.Path, "Sub", kvp.Key);
                byte[] fileData = await storageProvider.LoadAsync(fullPath);
                if (fileData.Length > 0)
                {
                    var jsonString = Encoding.UTF8.GetString(fileData);
                    JsonUtility.FromJsonOverwrite(jsonString, kvp.Value);
                }
                else
                {
                    data.SubData[kvp.Key] = null;
                }

            }
            data.OnAfterLoad();

            return deserialized;

        }

    }


}