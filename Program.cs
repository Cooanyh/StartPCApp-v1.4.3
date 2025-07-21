using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace StartPCApp
{
    public partial class Form1 : Form
    {
        private WebView2 webView;
        private bool isFullScreen = false;
        private FormWindowState lastWindowState;
        private FormBorderStyle lastBorderStyle;
        private Rectangle lastBounds;

        public Form1()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                // 设置窗体属性
                this.Text = "StartPC应用";
                this.Size = new Size(1200, 800);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.WindowState = FormWindowState.Maximized;

                // 保存当前窗体状态
                lastWindowState = this.WindowState;
                lastBorderStyle = this.FormBorderStyle;
                lastBounds = this.Bounds;

                // 创建WebView2控件
                webView = new WebView2()
                {
                    Dock = DockStyle.Fill
                };
                this.Controls.Add(webView);

                // 注册键盘事件
                this.KeyPreview = true;
                this.KeyDown += Form1_KeyDown;

                // 初始化WebView2
                await InitializeWebView2();
            }
            catch (Exception ex)
            {
                HandleWebView2Error(ex);
            }
        }

        private async Task InitializeWebView2()
        {
            try
            {
                // 首先尝试默认初始化
                await webView.EnsureCoreWebView2Async();
            }
            catch (Exception ex)
            {
                try
                {
                    // 如果默认初始化失败，使用自定义配置
                    string userDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StartPCApp", "WebView2Data");
                    
                    // 确保目录存在
                    if (!Directory.Exists(userDataFolder))
                    {
                        Directory.CreateDirectory(userDataFolder);
                    }

                    var options = CoreWebView2Environment.CreateCoreWebView2EnvironmentOptions();
                    options.AdditionalBrowserArguments = "--no-sandbox --disable-gpu-sandbox --disable-web-security";
                    
                    var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, options);
                    await webView.EnsureCoreWebView2Async(environment);
                }
                catch (Exception fallbackEx)
                {
                    throw new Exception($"WebView2初始化失败: {ex.Message} | 降级尝试也失败: {fallbackEx.Message}", ex);
                }
            }

            // 配置WebView2设置
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.ZoomFactor = 1.5;

            // 注册页面加载完成事件
            webView.CoreWebView2.DOMContentLoaded += async (sender, e) =>
            {
                try
                {
                    // 注入基础全屏优化脚本
                    await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
                        // 基础全屏优化
                        document.addEventListener('DOMContentLoaded', function() {
                            const style = document.createElement('style');
                            style.textContent = `
                                body { margin: 0; padding: 0; overflow: hidden; }
                                video { width: 100vw; height: 100vh; object-fit: cover; }
                            `;
                            document.head.appendChild(style);
                        });
                    ");
                }
                catch (Exception scriptEx)
                {
                    // 脚本注入失败不影响主要功能
                    Console.WriteLine($"脚本注入失败: {scriptEx.Message}");
                }
            };

            // 导航到目标URL
            webView.CoreWebView2.Navigate("https://www.startpc.cn/");
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                ToggleFullScreen();
            }
            else if (e.KeyCode == Keys.Escape && isFullScreen)
            {
                ExitFullScreen();
            }
        }

        private void ToggleFullScreen()
        {
            if (isFullScreen)
            {
                ExitFullScreen();
            }
            else
            {
                EnterFullScreen();
            }
        }

        private void EnterFullScreen()
        {
            // 保存当前状态
            lastWindowState = this.WindowState;
            lastBorderStyle = this.FormBorderStyle;
            lastBounds = this.Bounds;

            // 设置全屏
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            this.TopMost = true;

            isFullScreen = true;
        }

        private void ExitFullScreen()
        {
            // 恢复之前的状态
            this.TopMost = false;
            this.FormBorderStyle = lastBorderStyle;
            this.WindowState = lastWindowState;
            if (lastWindowState == FormWindowState.Normal)
            {
                this.Bounds = lastBounds;
            }

            isFullScreen = false;
        }

        private void HandleWebView2Error(Exception ex)
        {
            string errorMessage = "WebView2初始化失败";
            string solution = "";

            // 根据错误类型提供具体解决方案
            if (ex.HResult == unchecked((int)0x80070005) || ex.Message.Contains("E_ACCESSDENIED"))
            {
                solution = "解决方案:\n1. 以管理员身份运行程序\n2. 检查WebView2 Runtime是否正确安装\n3. 确保用户数据目录有写入权限";
            }
            else if (ex.Message.Contains("Expecting object to be local"))
            {
                solution = "解决方案:\n1. 重新安装Microsoft Edge WebView2 Runtime\n2. 清理临时文件后重试\n3. 检查系统完整性";
            }
            else if (ex.Message.Contains("WebView2"))
            {
                solution = "解决方案:\n1. 安装最新版Microsoft Edge WebView2 Runtime\n2. 重启计算机后重试\n3. 检查系统兼容性";
            }
            else
            {
                solution = "解决方案:\n1. 重启程序\n2. 以管理员身份运行\n3. 联系技术支持";
            }

            // 记录详细错误信息到日志文件
            try
            {
                string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StartPCApp");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                
                string logFile = Path.Combine(logDir, "error.log");
                string logContent = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 错误详情:\n" +
                                  $"异常类型: {ex.GetType().Name}\n" +
                                  $"错误代码: {ex.HResult:X8}\n" +
                                  $"错误消息: {ex.Message}\n" +
                                  $"堆栈跟踪: {ex.StackTrace}\n" +
                                  $"解决方案: {solution}\n" +
                                  new string('-', 80) + "\n";
                
                File.AppendAllText(logFile, logContent);
            }
            catch
            {
                // 日志记录失败不影响错误显示
            }

            // 显示用户友好的错误信息
            string displayMessage = $"{errorMessage}\n\n{solution}\n\n详细错误: {ex.Message}\n\n错误日志已保存到: %APPDATA%\\StartPCApp\\error.log";
            MessageBox.Show(displayMessage, "启动错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // 显示简化的备用界面
            ShowFallbackUI();
        }

        private void ShowFallbackUI()
        {
            // 清除现有控件
            this.Controls.Clear();

            // 创建备用界面
            Label fallbackLabel = new Label
            {
                Text = "WebView2组件初始化失败\n\n请检查:\n1. Microsoft Edge WebView2 Runtime是否已安装\n2. 是否以管理员身份运行\n3. 查看错误日志获取详细信息\n\n按F5刷新重试",
                Font = new Font("Microsoft YaHei", 12),
                ForeColor = Color.DarkRed,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            this.Controls.Add(fallbackLabel);

            // 添加F5刷新功能
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    Application.Restart();
                }
            };
        }
    }
}