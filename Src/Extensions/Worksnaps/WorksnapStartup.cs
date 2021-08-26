using CommonImplementation.System;
using CommonImplementation.WindowsAPI;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Worksnaps
{
    class WorksnapStartup
    {
        private const string WorkSnapsProcessName = "WSClient";

        private const string ErrorWindow = "ErrorWindow";
        private const string LoginWindow = " Worksnaps Client - Login";
        private const string SelectProjectWindow = " Worksnaps Client - Select your project";
        private const string TaskInformationWindow = " Worksnaps Client - Enter Task Information";

        private KeyValuePair<string, string>[] ErrorWindows = new KeyValuePair<string, string>[] 
        {
            new KeyValuePair<string, string>("Login", "Cannot connect to the server. Please try again later."),
            new KeyValuePair<string, string>("Error", "Cannot retrieve the task list. This might be due to temporary issue on the server.\nPlease try again later.")
        };

        private Thread _thread;
        private bool _isStop;

        private const string ConfigFileName = "config.json";
        private Config _config;
        private Config _defaultConfig;

        public string ConfigDir { get; set; }

        public WorksnapStartup()
        {
            _config = null;

            _defaultConfig = new Config()
            {
                WorksnapsClientFilePath = @"C:\Program Files (x86)\Worksnaps\WSClient.exe",
                CheckingInterval = 5000,
                StartupDelay = 10000,
                UrlForConnectionChecking = "https://www.worksnaps.net/www/index.shtml"
            };
        }

        public void Start()
        {
            _isStop = false;

            if (_thread == null)
            {
                _thread = new Thread(_lauchingTimer_Tick);
                _thread.IsBackground = true;
                _thread.Start();
            }
        }

        public void Stop()
        {
            _isStop = true;
            _thread = null;
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
            while (!_isStop)
            {
                Debug.WriteLine("========== Checking ===========");

                if (!IsWorksnapsLaunched())
                {
                    if (!Network.IsInternetRecentlyConnected(_config.UrlForConnectionChecking))
                    {
                        Debug.WriteLine("Internet connection is not available at the moment.");

                        Thread.Sleep(_config.StartupDelay);
                        continue;
                    }
                    else
                    {
                        LaunchWorksnaps();
                    }
                }

                if (!IsWorksnapsRecording())
                {
                    if (IsWindowAvailable(ErrorWindow))
                    {
                        /*
                            * Currently, I can not manage to close the error window.
                            * I can only close the process instead, and start all over again.
                            * 
                            * TODO Find a better way to handle this situation.
                            */
                        CloseClient();
                    }
                    else
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

                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(_config.CheckingInterval);
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
            bool isAvailable = false;

            if (window == ErrorWindow)
            {
                foreach (var errorWindow in ErrorWindows)
                {
                    IntPtr hwnd = WindowsAPI.FindWindow("#32770", errorWindow.Key); // #32770 = Dialog
                    if (hwnd != IntPtr.Zero)
                    {
                        IntPtr controlHwnd = IntPtr.Zero;
                        do
                        {
                            controlHwnd = WindowsAPI.FindWindowEx(hwnd, controlHwnd, "Static", null); // Static = Text/Label
                            if (controlHwnd != IntPtr.Zero)
                            {
                                var text = WindowsAPI.GetWindowText(controlHwnd);
                                if (text == errorWindow.Value)
                                {
                                    isAvailable = true;
                                    break;
                                }
                            }
                        } while (controlHwnd != IntPtr.Zero);
                    }

                    if (isAvailable) break;
                }
            }
            else
            {
                IntPtr hWnd = WindowsAPI.FindWindow(null, window);
                isAvailable = hWnd != IntPtr.Zero;
            }

            Debug.WriteLine(window + " is available: " + isAvailable);

            return isAvailable;
        }

        private void ProceedWindow(string window)
        {
            Debug.WriteLine(window + " is proceeding.");

            string[] buttons = new string[] { };

            switch (window)
            {
                case LoginWindow:
                    buttons = new string[] { "Log In" };
                    break;

                case SelectProjectWindow:
                    buttons = new string[] { "OK" };
                    break;

                case TaskInformationWindow:
                    buttons = new string[] { "Start" };
                    break;
            }

            IntPtr hWnd = WindowsAPI.FindWindow(null, window);
            if (hWnd != IntPtr.Zero)
            {
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

        private void CloseClient()
        {
            var processes = Process.GetProcessesByName(WorkSnapsProcessName);
            foreach(var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch
                { }
            }
        }
    }
}
