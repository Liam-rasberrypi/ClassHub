// Modules/ModuleControlBase.cs
// 所有模块 UserControl 的基类，约定初始化流程与依赖注入入口
using System.Windows.Forms;
using ClassHub.Infrastructure;

namespace ClassHub.Modules
{
    public abstract class ModuleControlBase : UserControl
    {
        protected ClassHubAppContext? AppContext { get; private set; }

        public void Initialize(ClassHubAppContext appContext)
        {
            AppContext = appContext;

            // TODO: 模块统一初始化入口（事件绑定、数据加载、权限检查）
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            // TODO: 子模块覆写实现具体初始化逻辑
        }
    }
}
