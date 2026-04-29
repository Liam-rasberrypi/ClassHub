// DataService.cs
// 数据服务：统一管理本地数据存取、备份与恢复（离线优先）
namespace ClassHub.Services
{
    public interface IDataService
    {
        T Load<T>(string key) where T : new();
        void Save<T>(string key, T data);
        void Backup(string backupName);
        void Restore(string backupName);

        // TODO: 增加本地数据体积监控（目标 <= 100MB）
        long GetStorageUsageBytes();
    }

    public class DataService : IDataService
    {
        public T Load<T>(string key) where T : new()
        {
            // TODO: 本地 JSON/SQLite 读取
            return new T();
        }

        public void Save<T>(string key, T data)
        {
            // TODO: 本地数据保存（安全写入 + 失败回滚）
        }

        public void Backup(string backupName)
        {
            // TODO: 备份班级核心数据
        }

        public void Restore(string backupName)
        {
            // TODO: 恢复备份并校验数据完整性
        }

        public long GetStorageUsageBytes()
        {
            // TODO: 统计本地数据目录占用
            return 0;
        }
    }
}


