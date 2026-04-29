// NavigationService.cs
// WinForms 导航服务：管理“模块 Key -> UserControl 实例”的创建逻辑
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ClassHub.Services
{
    public interface INavigationService
    {
        void Register(string key, Func<UserControl> factory);
        UserControl Create(string key);
    }

    public class NavigationService : INavigationService
    {
        private readonly Dictionary<string, Func<UserControl>> _registry = new();

        public void Register(string key, Func<UserControl> factory)
        {
            // TODO: 增加参数校验、重复注册策略、日志记录
            _registry[key] = factory;
        }

        public UserControl Create(string key)
        {
            // TODO: 可扩展为权限校验 + 懒加载缓存 + 生命周期管理
            if (_registry.TryGetValue(key, out var factory))
            {
                return factory();
            }

            // TODO: 后续可替换为统一异常处理或“模块不存在”占位控件
            throw new KeyNotFoundException($"未注册模块: {key}");
        }
    }
}

