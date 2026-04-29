// Controllers/HomeworkController.cs
// 作业模块控制器
using ClassHub.Services;

namespace ClassHub.Controllers
{
    public class HomeworkController : IModuleController
    {
        private readonly IDataService _dataService;

        public HomeworkController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void Load() { /* TODO: 初始化作业数据源 */ }
        public void Activate() { /* TODO: 刷新临期提醒 */ }
        public void Deactivate() { /* TODO: 保存筛选与编辑状态 */ }
    }
}
