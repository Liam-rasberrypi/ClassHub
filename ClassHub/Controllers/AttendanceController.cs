// Controllers/AttendanceController.cs
// 点名册模块控制器
using ClassHub.Services;

namespace ClassHub.Controllers
{
    public class AttendanceController : IModuleController
    {
        private readonly IDataService _dataService;

        public AttendanceController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void Load() { /* TODO: 加载学生列表 */ }
        public void Activate() { /* TODO: 刷新排序和过滤 */ }
        public void Deactivate() { /* TODO: 提交未保存变更 */ }
    }
}
