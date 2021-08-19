using CommonImplementation.WindowsAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worksnaps
{
    class WorksnapStartup
    {
        private const string WorkSnapsProcessName = "WSClient";

        private const string LoginWindow = " Worksnaps Client - Login";
        private const string SelectProjectWindow = " Worksnaps Client - Select your project";
        private const string TaskInformationWindow = " Worksnaps Client - Enter Task Information";

        private Timer _lauchingTimer;
        private int _launchingInterval;

        private const string ConfigFileName = "config.json";
        private Config _config;
        private Config _defaultConfig;

        public string ConfigDir { get; set; }

        public WorksnapStartup()
        {
            _launchingInterval = 1000;
            _lauchingTimer = new Timer(_lauchingTimer_Tick, null, Timeout.Infinite, _launchingInterval);

            _config = null;

            _defaultConfig = new Config()
            {
                WorksnapsClientFilePath = @"C:\Program Files (x86)\Worksnaps\WSClient.exe"
            };
        }

        public void Start()
        {
            _lauchingTimer.Change(0, _launchingInterval);
        }

        public void Stop()
        {
            _lauchingTimer.Change(Timeout.Infinite, _launchingInterval);
        }

        public void LoadConfig()
        {
            var configFile = Path.Combine(ConfigDir, ConfigFileName);

            string configJson = string.Empty;
            
            if (File.Exists(configFile))
            {
                configJson = File.ReadAllText(configFile);
            }

            _config = null;

            if (!string.IsNullOrEmpty(configJson))
            {
                try
                {
                    _config = JsonConvert.DeserializeObject<Config>(configJson);
                }
                catch
                { }
            }

            if (_config == null)
            {
                _config = _defaultConfig;
            }
        }

        public void SaveConfig()
        {
            string configJson = JsonConvert.SerializeObject(_config, Formatting.Indented);

            var configFile = Path.Combine(ConfigDir, ConfigFileName);

            File.WriteAllText(configFile, configJson);
        }

        private void _lauchingTimer_Tick(object sender)
        {
            Debug.WriteLine("========== Checking ===========");

            if (!IsWorksnapsLaunched())
            {
                LaunchWorksnaps();
            }

            if (!IsWorksnapsRecording())
            {
                if (IsWindowAvailable(LoginWindow) && IsWindowVisibile(LoginWindow))
                {
                    ProceedWindow(LoginWindow);
                }

                if (IsWindowAvailable(SelectProjectWindow))
                {
                    ProceedWindow(SelectProjectWindow);
                }

                if (IsWindowAvailable(TaskInformationWindow))
                {
                    ProceedWindow(TaskInformationWindow);
                }
            }
        }

        private void LaunchWorksnaps()
        {
            Debug.WriteLine("Launching...");

            var wsclientPath = _config.WorksnapsClientFilePath;

            if (File.Exists(wsclientPath))
            {
                Process.Start(wsclientPath);
            }
        }

        private bool IsWorksnapsLaunched()
        {
            bool isLaunched = false;

            Process[] processCollection = Process.GetProcesses();

            foreach (var process in processCollection)
            {
                if (process.ProcessName.ToLower() == WorkSnapsProcessName.ToLower())
                {
                    isLaunched = true;
                    break;
                }
            }

            Debug.WriteLine("Worksnaps is running: " + isLaunched);

            return isLaunched;
        }

        private bool IsWorksnapsRecording()
        {
            bool isLoginWindowVisible               = IsWindowVisibile(LoginWindow); // Login window is always available, it's hidden after a user login successfully.
            bool isSelectProjectWindowAvailable     = IsWindowAvailable(SelectProjectWindow);
            bool isTaskInformationWindowAvailable   = IsWindowAvailable(TaskInformationWindow);

            bool isRecording = !isLoginWindowVisible &&
                               !isSelectProjectWindowAvailable &&
                               !isTaskInformationWindowAvailable;

            Debug.WriteLine("Worksnaps is recording: " + isRecording);

            return isRecording;
        }

        private bool IsWindowVisibile(string window)
        {
            bool isVisible = false;

            IntPtr hWnd = WindowsAPI.FindWindow(null, window);
            if (hWnd != IntPtr.Zero)
            {
                long style = WindowsAPI.GetWindowLongPtr(hWnd, WindowsAPI.GWL_STYLE);
                isVisible = (style & WindowsAPI.WS_VISIBLE) == WindowsAPI.WS_VISIBLE;
            }

            Debug.WriteLine(window + " is visibile: " + isVisible);

            return isVisible;
        }

        private bool IsWindowAvailable(string window)
        {
            IntPtr hWnd = WindowsAPI.FindWindow(null, window);
            bool isAvailable = hWnd != IntPtr.Zero;

            Debug.WriteLine(window + " is available: " + isAvailable);

            return isAvailable;
        }

        private void ProceedWindow(string window)
        {
            Debug.WriteLine(window + " is proceeding.");

            switch (window)
            {
                case LoginWindow:
                    ProceedLoginWindow();
                    break;

                case SelectProjectWindow:
                    ProceedSelectProjectWindow();
                    break;

                case TaskInformationWindow:
                    ProceedTaskInformationWindow();
                    break;
            }
        }

        private void ProceedLoginWindow()
        {
            IntPtr hWnd = WindowsAPI.FindWindow(null, LoginWindow);
            if (hWnd != IntPtr.Zero)
            {
                IntPtr hWndButton = WindowsAPI.FindWindowEx(hWnd, IntPtr.Zero, null, "Log In");

                WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void ProceedSelectProjectWindow()
        {
            IntPtr hWnd = WindowsAPI.FindWindow(null, SelectProjectWindow);
            if (hWnd != IntPtr.Zero)
            {
                IntPtr hWndButton = WindowsAPI.FindWindowEx(hWnd, IntPtr.Zero, null, "OK");

                WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private void ProceedTaskInformationWindow()
        {
            IntPtr hWnd = WindowsAPI.FindWindow(null, TaskInformationWindow);
            if (hWnd != IntPtr.Zero)
            {
                string[] buttons = new string[]
                {
                    "OK",
                    "Start"
                };
                foreach (var button in buttons)
                {
                    IntPtr hWndButton = WindowsAPI.FindWindowEx(hWnd, IntPtr.Zero, null, button);
                    if (hWndButton != IntPtr.Zero)
                    {
                        WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                        WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }
    }
}
