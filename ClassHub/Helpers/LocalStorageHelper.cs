// LocalStorageHelper.cs
// 本地存储辅助类：WinForms 侧统一读写入口（JSON/文本/二进制）
using System;
using System.IO;
using System.Text.Json;

namespace ClassHub.Helpers
{
    public static class LocalStorageHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public static T LoadJson<T>(string path) where T : new()
        {
            try
            {
                if (!File.Exists(path))
                {
                    return new T();
                }

                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new T();
                }

                return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? new T();
            }
            catch
            {
                // 读取失败时回退默认对象，避免影响主流程
                return new T();
            }
        }

        public static void SaveJson<T>(string path, T data)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(path, json);
        }

        public static bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}


