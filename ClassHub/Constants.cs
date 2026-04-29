// Constants.cs
// 全局常量定义（WinForms 架构）
namespace ClassHub
{
    public static class Constants
    {
        public const string AppName = "班级全能管理助手";

        // 架构约束：本地数据总量控制目标（单位：MB）
        public const int LocalDataSizeLimitMb = 100;

        // TODO: 增加低配模式参数（刷新间隔、缓存上限、模块懒加载开关）
        // TODO: 拆分为 UIConstants / StorageConstants / SecurityConstants
    }
}


