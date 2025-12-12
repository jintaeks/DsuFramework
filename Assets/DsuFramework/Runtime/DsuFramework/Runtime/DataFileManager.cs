using System;
using System.IO;
using UnityEngine;

namespace Dsu.Framework
{
    public static class DataFileManager
    {
        private static string GetFilePath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        public static void Save<T>(T data, string fileName)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetFilePath(fileName), json);
#if UNITY_EDITOR
            Debug.Log($"[DataFileManager.Save] Saved to: {GetFilePath(fileName)}\n{json}");
#endif
        }

        public static bool Load<T>(string fileName, out T data)
        {
            string path = GetFilePath(fileName);
            if (!File.Exists(path)) {
                data = default;
                return false;
            }

            string json = File.ReadAllText(path);
            data = JsonUtility.FromJson<T>(json);
#if UNITY_EDITOR
            Debug.Log($"[DataFileManager.Load] Loaded from: {path}\n{json}");
#endif
            return true;
        }

        public static void Delete(string fileName)
        {
            string path = GetFilePath(fileName);
            if (File.Exists(path)) {
                File.Delete(path);
#if UNITY_EDITOR
                Debug.Log($"[DataFileManager.Delete] Deleted file: {path}");
#endif
            }
        }
    }
}