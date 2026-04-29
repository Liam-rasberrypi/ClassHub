// Controllers/DutyController.cs
// 值日模块控制器
using ClassHub.Services;

namespace ClassHub.Controllers
{
    public class DutyController : IModuleController
    {
        private readonly IDataService _dataService;

        public DutyController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void Load() { /* TODO: 加载值日安排 */ }
        public void Activate() { /* TODO: 刷新当天安排 */ }
        public void Deactivate() { /* TODO: 保存轮值规则 */ }
    }
}
