// FileService.cs
// 文件服务：班级资料文件的上传、分类、打开、检索
using System.Collections.Generic;
using ClassHub.Models;

namespace ClassHub.Services
{
    public interface IFileService
    {
        IEnumerable<ClassFileItem> GetAll();
        void Import(string sourcePath, string category);
        void Open(ClassFileItem item);
        void Delete(ClassFileItem item);

        // TODO: 增加标签检索、批量导入、云同步等能力
    }

    public class FileService : IFileService
    {
        public IEnumerable<ClassFileItem> GetAll()
        {
            // TODO: 从索引仓库加载资料列表
            return new List<ClassFileItem>();
        }

        public void Import(string sourcePath, string category)
        {
            // TODO: 执行文件复制、命名规范校验、分类入库
        }

        public void Open(ClassFileItem item)
        {
            // TODO: 使用系统默认程序打开文件或打开指定预览器
        }

        public void Delete(ClassFileItem item)
        {
            // TODO: 删除文件并更新索引
        }
    }
}

