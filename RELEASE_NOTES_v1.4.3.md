# StartPCApp v1.4.3 发布说明

## 🔧 重要修复

### WebView2 初始化优化
- **简化初始化逻辑**: 实现了更稳定的WebView2初始化机制
- **降级策略**: 采用先尝试默认初始化，失败后使用自定义配置的降级策略
- **减少配置冲突**: 移除了可能导致冲突的WebView2设置项
- **优化脚本注入**: 简化页面加载后的脚本，减少潜在错误

### 技术改进
- 移除了 `AreDefaultContextMenusEnabled`、`IsGeneralAutofillEnabled`、`IsPasswordAutosaveEnabled` 等可能引起问题的配置
- 简化了CSS优化脚本，只保留核心全屏显示功能
- 优化了错误处理机制，提供更准确的错误诊断

## 📦 发布文件

### 便携版 (推荐)
- **文件名**: `StartPCApp-Portable-v1.4.3-Fixed.zip`
- **说明**: 解压即用，无需安装
- **适用**: 临时使用、测试环境

### 安装版
- **文件名**: `StartPCApp_Setup.exe` (位于Output目录)
- **说明**: 完整安装程序，自动配置系统集成
- **适用**: 长期使用、生产环境

## 🚀 快速开始

### 便携版使用
1. 下载 `StartPCApp-Portable-v1.4.3-Fixed.zip`
2. 解压到任意目录
3. 双击 `StartPCApp.exe` 运行

### 安装版使用
1. 下载 `StartPCApp_Setup.exe`
2. 以管理员身份运行安装程序
3. 按照向导完成安装
4. 从开始菜单或桌面快捷方式启动

## 🔍 核心功能

- **智能启动**: 自动检测并启动PC应用
- **全屏优化**: 提供最佳的全屏显示体验
- **WebView2集成**: 基于最新的Microsoft WebView2技术
- **错误诊断**: 完善的错误日志和诊断功能

## 💻 系统要求

- **操作系统**: Windows 10/11 (64位)
- **运行时**: .NET 6.0 Runtime (安装版自动包含)
- **浏览器**: Microsoft Edge WebView2 Runtime
- **权限**: 建议以管理员身份运行

## 🛠️ 故障排除

### 如果程序无法启动
1. 确保已安装 Microsoft Edge WebView2 Runtime
2. 尝试以管理员身份运行程序
3. 检查 `%APPDATA%\StartPCApp\error.log` 错误日志
4. 确保系统满足最低要求

### 如果遇到WebView2错误
- 程序已自动优化WebView2初始化逻辑
- 如仍有问题，请查看错误日志获取详细信息

## 📋 版本历史

- **v1.4.3** (2025-01-21): WebView2初始化优化，简化配置，提升稳定性
- **v1.4.2** (2025-01-21): WebView2数据目录权限修复
- **v1.4.1**: 基础功能完善

## 🆘 技术支持

如遇到问题，请提供以下信息：
- 操作系统版本
- 错误日志内容 (`%APPDATA%\StartPCApp\error.log`)
- 具体错误现象描述

---

**注意**: 本版本专门针对WebView2初始化问题进行了深度优化，建议所有用户升级到此版本。