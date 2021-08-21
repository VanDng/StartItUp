using CommonImplementation.WindowsAPI;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace OpenVPN
{
    class OpenVPNStartup : IDisposable
    {
        private const string OpenVPNServiceProcessName = "openvpn";

        private string _ovpnProfileName;

        private Thread _thread;
        CancellationTokenSource _threadCancelTokenSrc;
        private ManualResetEvent _startEvent;

        private const string ConfigFileName = "config.json";
        private Config _config;
        private Config _defaultConfig;

        public string ConfigDir { get; set; }

        public OpenVPNStartup()
        {
            _ovpnProfileName = string.Empty;

            _startEvent = new ManualResetEvent(false);

            _threadCancelTokenSrc = new CancellationTokenSource();

            _thread = new Thread(MainProc);
            _thread.Start(_threadCancelTokenSrc.Token);

            _config = null;

            _defaultConfig = new Config()
            {
                ProcessName = "openvpn-gui",
                ClientFilePath = @"C:\Program Files\OpenVPN\bin\openvpn-gui.exe",
                ConfigDir = string.Format(@"C:\Users\{0}\OpenVPN\{1}", Environment.UserName, "config"),
                LogDir = string.Format(@"C:\Users\{0}\OpenVPN\{1}", Environment.UserName, "log"),
                OVPNKeyword = "office-vpn.websparks.sg",
                RegistryConfigDir = @"HKEY_CURRENT_USER\SOFTWARE\OpenVPN-GUI"
            };
        }

        public void Start()
        {
            _startEvent.Set();
        }

        public void Stop()
        {
            _startEvent.Reset();
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

        private void MainProc(object token)
        {
            CancellationToken cancellationToken = (CancellationToken)token;

            while (true)
            {
                _startEvent.WaitOne();

                if (cancellationToken.IsCancellationRequested) break;

                Debug.WriteLine("========== Checking ===========");

                if (string.IsNullOrEmpty(_ovpnProfileName))
                {
                    _ovpnProfileName = FindOVPNProfileName();
                }

                if (string.IsNullOrEmpty(_ovpnProfileName))
                {
                    return;
                }

                if (CloseAnyDialog())
                {
                    /*
                     * If there is any dialog titled "OpenVPN GUI", OpenVPN might be working improperly.
                     * Therefore, I will stop the OpenVPN service and start all over again.
                     */

                    StopOpenVPNService();
                }

                if (!IsSilentConnectionSet())
                {
                    SetSilentConnectionMode();
                }

                if (IsLaunched())
                {
                    if (!LogSaysConnecting())
                    {
                        Connect();
                    }
                }
                else
                {
                    Connect();
                }

                Thread.Sleep(1000);
            }
        }

        private bool IsLaunched()
        {
            bool isLaunched = false;

            Process[] processCollection = Process.GetProcesses();

            foreach (var process in processCollection)
            {
                if (process.ProcessName.ToLower() == _config.ProcessName.ToLower())
                {
                    isLaunched = true;
                    break;
                }
            }

            Debug.WriteLine("OpenVPN GUI is running: " + isLaunched);

            return isLaunched;
        }

        private void StopOpenVPNService()
        {
            var serviceProcesses = Process.GetProcessesByName(OpenVPNServiceProcessName);
            var guiProcesses = Process.GetProcessesByName(_config.ProcessName);

            var processes = serviceProcesses.Union(guiProcesses);

            if (processes != null)
            {
                foreach (var p in processes)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    { }
                }
            }
        }

        private bool LogSaysConnecting()
        {
            bool isConnecting = false;

            var logFilePath = $@"{_config.LogDir}\{_ovpnProfileName}.log";

            if (File.Exists(logFilePath))
            {
                var newLogFileName = Guid.NewGuid().ToString();
                var newlogFilePath = $@"{_config.LogDir}\{newLogFileName}.log";

                string[] logLines = { };
                try
                {
                    File.Copy(logFilePath, newlogFilePath);

                    logLines = File.ReadLines(newlogFilePath).Reverse().Take(5).ToArray();

                    File.Delete(newlogFilePath);
                }
                catch
                { }

                foreach (var log in logLines)
                {
                    if (log.Contains("Initialization Sequence Completed"))
                    {
                        isConnecting = true;
                        break;
                    }
                }
            }
            else
            {
                Debug.WriteLine("Could not find log file.");
            }

            return isConnecting;
        }

        private string FindOVPNProfileName()
        {
            string ovpnFileProfileName = string.Empty;

            var ovpnFiles = Directory.GetFiles(_config.ConfigDir, "*.ovpn", SearchOption.AllDirectories);

            foreach (var ovpnFile in ovpnFiles)
            {
                string fileContent = File.ReadAllText(ovpnFile);

                if (fileContent.Contains(_config.OVPNKeyword))
                {
                    ovpnFileProfileName = Path.GetFileNameWithoutExtension(ovpnFile);
                    break;
                }
            }

            if (string.IsNullOrEmpty(ovpnFileProfileName))
            {
                Debug.WriteLine("Could not find any ovpn file containing the defined key word.");
            }
            else
            {
                Debug.WriteLine("OVPN Profile: " + ovpnFileProfileName);
            }

            return ovpnFileProfileName;
        }

        private void Connect()
        {
            Debug.WriteLine("Connecting...");

            var clientFilePath = _config.ClientFilePath;

            if (File.Exists(clientFilePath))
            {
                Process.Start(_config.ClientFilePath, $"--connect {_ovpnProfileName}");
            }
            else
            {
                Debug.WriteLine("Cound not found client.");
            }
        }

        private bool IsSilentConnectionSet()
        {
            bool isSet = false;

            var isSilent = (int)Registry.GetValue(_config.RegistryConfigDir, "silent_connection", string.Empty);
            isSet = isSilent != 0;

            return isSet;
        }

        private void SetSilentConnectionMode()
        {
            Registry.SetValue(_config.RegistryConfigDir, "silent_connection", 1);
        }

        private bool CloseAnyDialog()
        {
            bool isDialogClosed = false;

            IntPtr hwnd = WindowsAPI.FindWindow(null, "OpenVPN GUI");
            if (hwnd != IntPtr.Zero)
            {
                IntPtr button = WindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, "Button", null);
                if (button  != IntPtr.Zero)
                {
                    WindowsAPI.PostMessage(button, WindowsAPI.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                    WindowsAPI.PostMessage(button, WindowsAPI.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

                    isDialogClosed = true;
                }
            }

            return isDialogClosed;
        }

        public void Dispose()
        {
            _threadCancelTokenSrc.Cancel();
            _startEvent.Set();
        }
    }
}
