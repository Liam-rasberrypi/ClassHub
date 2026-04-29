# ClassHub WinForms 二期架构蓝图（按《班级全能管理助手》大纲）

> 当前阶段：**仅完成架构二次调整与文件骨架**，不写业务细节实现。  
> 技术栈：**WinForms + C# + 本地存储（离线优先）**。

## 1. 产品定位与约束

- 目标设备：Windows10 教室旧电脑（32/64 位，低内存低性能）
- 核心方向：班级管理 + 教学辅助 + 系统优化一体化
- 约束原则：
  - 轻量化（低占用、低耦合、低后台负担）
  - 一站式（四大模块统一入口）
  - 易操作（大按钮、少步骤、可远距离识别）
  - 本地存储（无网可用，保护班级数据）
  - 可修改（参数配置可编辑）
  - 多账户权限（密码/U盘/组合验证）

---

## 2. 分层架构

### 2.1 启动层
- `Program.cs`
  - 启动 WinForms
  - 构建应用上下文（`Bootstrapper.Build()`）
  - 打开主窗体 `MainForm`

### 2.2 组合根与上下文
- `Infrastructure/Bootstrapper.cs`
  - 注册服务
  - 注册模块导航（一级模块 + 二级功能）
- `Infrastructure/AppContext.cs` (`ClassHubAppContext`)
  - 汇总系统服务、权限服务、策略服务、导航服务

### 2.3 界面层（WinForms）
- `Forms/MainForm.cs`
  - 一级导航：四大模块
  - 内容区：模块控件承载
  - 后续支持全屏/窗口切换、低分辨率适配
- `Forms/LoginForm.cs`
  - 多账户登录入口（密码/U盘）
- `Forms/SettingsForm.cs`
  - 设置编辑入口（可保留为独立弹窗）

### 2.4 模块层（UserControl）
- 基类：`Modules/ModuleControlBase.cs`
- 一级模块：
  - `ClassManagementCenterModuleControl`
  - `TeachingAssistCenterModuleControl`
  - `SystemOptimizationCenterModuleControl`
  - `SettingsCenterModuleControl`
- 二级模块（已建骨架）：
  - 班级管理：`Attendance / Duty / Homework / Notice`
  - 教学辅助：`Paintboard / ClassFiles`
  - 系统优化：`Optimizer / Account`
  - 软件设置：`AppSettings / DataBackup`

### 2.5 服务层
- 已有服务：
  - `IAccountService`
  - `IDataService`
  - `IFileService`
  - `ISettingsService`
  - `ISystemOptimizerService`
  - `INavigationService`
- 二期新增：
  - `ISecurityService`（账户验证、功能密码、权限校验）
  - `IClassroomPolicyService`（系统限制策略）

### 2.6 控制层（可选增强）
- `Controllers/*Controller.cs`
  - 保留模块流程编排入口
  - 后续逐步承接 UI 事件逻辑与业务协调

---

## 3. 导航结构（与大纲一致）

### 一级模块（主界面卡片）
1. 班级日常管理
2. 教学辅助工具
3. 电脑一键优化
4. 软件基础设置

### 二级功能映射（ModuleKeys）
- 班级日常管理：点名册、值日生安排、班级作业板、班级公告板
- 教学辅助工具：简易画板、班级资料夹
- 电脑一键优化：系统优化、账户与权限
- 软件基础设置：界面/行为设置、数据备份恢复

---

## 4. 权限与安全架构（框架约束）

- 账户体系：管理员 / 普通学生 / 可扩展副班委
- 验证方式：
  - 密码验证
  - U盘设备验证
  - 组合验证（密码 + U盘）
- 功能级保护：点名册、值日、作业、公告、画板支持独立密码门禁
- 审计方向：登录日志、关键操作日志（后续落地）

---

## 5. 性能与部署约束（架构级）

- 本地优先：全部核心数据本地读写
- 控件懒加载：模块按需创建，切换时释放非必要资源
- 低配策略：减少常驻后台任务，避免高频 IO/高频刷新
- 部署目标：单机可运行，简化依赖

---

## 6. 开发阶段规划（对应你给的大纲）

### 第一阶段（核心必做）
- 点名册管理
- 值日生安排
- 班级作业板
- 简易画板

### 第二阶段（次要开发）
- 系统一键优化
- 基础系统设置修改
- 快捷操作/账户权限基础能力

### 第三阶段（完善优化）
- 班级公告板
- 班级资料夹
- 高级设置
- 数据备份与恢复
- 权限精细化

---

## 7. WinUI 脱钩状态

- `ClassHub.csproj` 已使用 WinForms 目标
- `*.xaml` 已从项目处理中移除
- `App.xaml.cs` / `MainWindow.xaml.cs` 仅保留迁移说明，不再作为运行入口
