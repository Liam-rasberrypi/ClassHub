// LocalStorageHelper.cs
// 本地存储辅助类：WinForms 侧统一读写入口（JSON/文本/二进制）
namespace ClassHub.Helpers
{
    public static class LocalStorageHelper
    {
        public static T LoadJson<T>(string path) where T : new()
        {
            // TODO: 实现 JSON 读取与反序列化，增加文件不存在容错
            return new T();
        }

        public static void SaveJson<T>(string path, T data)
        {
            // TODO: 实现 JSON 序列化与安全写入（临时文件 + 替换）
        }

        public static bool Exists(string path)
        {
            // TODO: 封装文件存在检查与日志记录
            return false;
        }
    }
}

