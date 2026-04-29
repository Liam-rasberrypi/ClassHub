// Controllers/NoticeController.cs
// 公告模块控制器
using ClassHub.Services;

namespace ClassHub.Controllers
{
    public class NoticeController : IModuleController
    {
        private readonly IDataService _dataService;

        public NoticeController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void Load() { /* TODO: 加载公告列表 */ }
        public void Activate() { /* TODO: 进入模块时刷新数据 */ }
        public void Deactivate() { /* TODO: 离开模块前保存草稿 */ }
    }
}
