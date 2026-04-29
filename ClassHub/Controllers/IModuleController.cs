// Controllers/IModuleController.cs
// 模块控制器接口（WinForms）
namespace ClassHub.Controllers
{
    public interface IModuleController
    {
        // TODO: 约定模块生命周期：加载、激活、离开、释放
        void Load();
        void Activate();
        void Deactivate();
    }
}
