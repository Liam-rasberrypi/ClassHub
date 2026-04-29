# 班级全能管理助手（WinForms + C# 版）

## 项目简介
本项目基于 **WinForms + C#**，面向教室 **Windows 10 旧电脑** 场景，聚焦“低配置可用、离线可用、班级管理一体化”。

目标是通过一个轻量软件完成：
- 班级日常管理
- 教学辅助
- 旧电脑系统优化
- 软件基础设置与权限管控

## 技术与架构
- UI：WinForms（不再使用 WinUI/XAML 作为运行架构）
- 语言：C#
- 数据：本地存储（离线优先）
- 结构：Modules + Services + Controllers + Infrastructure

> 当前仓库处于“**框架优先**”阶段：已搭好目录和模块骨架，业务细节以 `TODO` 注释标记。

## 目录结构（当前主干）
- `ClassHub/Program.cs`：WinForms 启动入口
- `ClassHub/Forms/`：窗体层（如 `MainForm`、`LoginForm`、`SettingsForm`）
- `ClassHub/Modules/`：模块层（四大一级模块 + 各二级功能模块）
- `ClassHub/Services/`：服务层（账户、数据、设置、系统优化、安全、策略等）
- `ClassHub/Controllers/`：控制层（模块流程编排骨架）
- `ClassHub/Infrastructure/`：应用上下文与启动装配
- `ClassHub/Models/`：数据模型与权限标识
- `ClassHub/ARCHITECTURE.md`：详细架构说明
- `ClassHub/DEVELOPMENT_PHASES.md`：阶段开发清单

## 功能模块规划（四大模块）
1. 班级日常管理
   - 点名册管理
   - 值日生安排
   - 班级作业板
   - 班级公告板
2. 教学辅助工具
   - 简易画板
   - 班级资料夹
3. 电脑一键优化
   - 系统一键优化
   - 系统设置修改
   - 账户与权限管理
4. 软件基础设置
   - 用户/权限设置
   - 数据备份与恢复
   - 界面设置
   - 开机自启

## 运行与适配目标
- 兼容 Windows 10（32/64 位）
- 适配低配旧电脑，强调低内存占用与流畅度
- 本地数据可控，便于班级环境长期使用

## 说明
- 仓库已切换 WinForms 方向，WinUI 历史文件仅作为迁移参考。
- 若需继续开发，建议按 `DEVELOPMENT_PHASES.md` 分阶段推进。

