// Controllers/ClassFilesController.cs
// 资料模块控制器
using ClassHub.Services;

namespace ClassHub.Controllers
{
    public class ClassFilesController : IModuleController
    {
        private readonly IFileService _fileService;

        public ClassFilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void Load() { /* TODO: 加载文件索引 */ }
        public void Activate() { /* TODO: 刷新目录状态 */ }
        public void Deactivate() { /* TODO: 停止后台扫描任务 */ }
    }
}
