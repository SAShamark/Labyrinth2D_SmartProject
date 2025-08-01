using System.IO;
using Newtonsoft.Json;
using Services.DataProtection;
using UnityEngine;


namespace Services.Storage
{
    public static class StorageService
    {
        public static void SaveData<T>(string key, T data)
        {
            string path = BuildPath(key);
            string jsonWithoutProtection = JsonConvert.SerializeObject(data, Formatting.Indented);
            string json = DataProtectionManager.Encode(jsonWithoutProtection, key);
            File.WriteAllText(path, json);
        }

        public static T LoadData<T>(string key, T defaultValue)
        {
            string path = BuildPath(key);

            if (File.Exists(path))
            {
                string jsonWithProtection = File.ReadAllText(path);
                string json = DataProtectionManager.Decode(jsonWithProtection, key);
                return JsonConvert.DeserializeObject<T>(json);
            }

            Debug.Log($"Returned default data by key [{key}] by value [{defaultValue}]");
            return defaultValue;
        }

        public static void DeleteAllData()
        {
            string directory = Application.persistentDataPath;

            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
                Directory.CreateDirectory(directory);
            }
        }

        private static string BuildPath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key);
        }
    }
}