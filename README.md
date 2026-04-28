# 班级全能管理助手（WinUI 3 C# 版）

## 项目简介
本项目基于 WinUI 3 框架，主语言为 C#，专为班级教室 Windows10 旧电脑打造，集班级日常管理、教学辅助、电脑系统维护于一体。

## 目录结构说明
- `ClassHub/App.xaml`、`App.xaml.cs`：应用入口及全局资源。
- `ClassHub/MainWindow.xaml`、`MainWindow.xaml.cs`：主界面，卡片式布局，四大功能模块入口。
- `ClassHub/Views/`：各功能模块的界面（XAML）。
- `ClassHub/ViewModels/`：各功能模块的视图模型（MVVM 逻辑）。
- `ClassHub/Models/`：数据模型（如学生、作业、账户等）。
- `ClassHub/Services/`：本地存储、系统操作等服务类。

## 功能模块
- 班级日常管理（点名册、值日生、作业板、公告板）
- 教学辅助工具（简易画板、班级资料夹）
- 电脑一键优化（系统优化、账户管理等）
- 软件基础设置（权限、备份、界面、开机自启等）

## 开发建议
- 各 XAML 文件内已用中文注释标明功能区域，便于后续开发。
- 推荐采用 MVVM 架构，逻辑与界面分离。
- 兼容 Windows10 32/64 位，适配低配旧电脑。

## 备注
如需添加具体功能，请在对应 ViewModel、Model、Service 目录下新建 C# 文件，并参考注释实现。
