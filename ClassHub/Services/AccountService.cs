// AccountService.cs
// 账户服务：账户生命周期、认证、授权、日志
using System.Collections.Generic;
using ClassHub.Models;

namespace ClassHub.Services
{
    public interface IAccountService
    {
        IEnumerable<Account> GetAll();
        Account? GetByUserName(string userName);

        // TODO: 密码验证
        bool ValidateCredentials(string userName, string password);

        // TODO: U 盘验证
        bool ValidateUsbBinding(string userName, string usbDeviceId);

        // TODO: 组合验证（依据账户 AuthMode 决定）
        bool ValidateLogin(string userName, string? password, string? usbDeviceId);

        void Create(Account account);
        void Update(Account account);
        void Delete(string userName);

        // TODO: 登录日志、角色授权、密码重置、U 盘重新绑定
    }

    public class AccountService : IAccountService
    {
        public IEnumerable<Account> GetAll()
        {
            // TODO: 从本地存储加载账户列表
            return new List<Account>();
        }

        public Account? GetByUserName(string userName)
        {
            // TODO: 按用户名查询账户详情
            return null;
        }

        public bool ValidateCredentials(string userName, string password)
        {
            // TODO: 密码哈希比对
            return false;
        }

        public bool ValidateUsbBinding(string userName, string usbDeviceId)
        {
            // TODO: 校验账户绑定 U 盘设备 ID
            return false;
        }

        public bool ValidateLogin(string userName, string? password, string? usbDeviceId)
        {
            // TODO: 根据账户验证策略执行密码/U盘/组合验证
            return false;
        }

        public void Create(Account account)
        {
            // TODO: 创建账户并持久化
        }

        public void Update(Account account)
        {
            // TODO: 更新账户信息并记录审计日志
        }

        public void Delete(string userName)
        {
            // TODO: 删除账户并处理关联数据
        }
    }
}


