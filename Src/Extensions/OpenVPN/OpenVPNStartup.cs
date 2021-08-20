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
    class OpenVPNStartup
    {
        private string _ovpnProfileName;

        private Timer _lauchingTimer;
        private int _launchingInterval;

        private const string ConfigFileName = "config.json";
        private Config _config;
        private Config _defaultConfig;

        public string ConfigDir { get; set; }

        public OpenVPNStartup()
        {
            _ovpnProfileName = string.Empty;

            _launchingInterval = 1000;
            _lauchingTimer = new Timer(_lauchingTimer_Tick, null, Timeout.Infinite, _launchingInterval);

            _config = null;

            _defaultConfig = new Config()
            {
                ProcessName = "openvpn-gui",
                ClientFilePath = @"C\Program Files\OpenVPN\bin\openvpn-gui.exe",
                ConfigDir = string.Format(@"C:\Users\{0}\OpenVPN\{1}", Environment.UserName, "config"),
                LogDir = string.Format(@"C:\Users\{0}\OpenVPN\{1}", Environment.UserName, "log"),
                OVPNKeyword = "office-vpn.websparks.sg",
                RegistryConfigDir = @"HKEY_CURRENT_USER\SOFTWARE\OpenVPN-GUI"
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

            if (string.IsNullOrEmpty(_ovpnProfileName))
            {
                _ovpnProfileName = FindOVPNProfileName();
            }

            if (string.IsNullOrEmpty(_ovpnProfileName))
            {
                return;
            }

            if (!IsSilentConnectionSet())
            {
                SetSilentConnectionMode();
            }

            if (!IsConnecting())
            {
                if (IsLaunched())
                {
                    StopProcess();
                }
                else
                {
                    Connect();
                }
            }
        }

        //private void LaunchWithSilentModeEnabled()
        //{
        //    Debug.WriteLine("Launching...");

        //    var clientFilePath = _config.ClientFilePath;

        //    if (File.Exists(clientFilePath))
        //    {
        //        Process.Start(_config.ClientFilePath, "--slient_connection 1");
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Cound not found client.");
        //    }
        //}

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

        private void StopProcess()
        {
            var process = Process.GetProcessesByName(_config.ProcessName).FirstOrDefault();

            if (process != null)
            {
                try
                {
                    process.Kill();
                }
                catch
                { }
            }
        }

        private bool IsConnecting()
        {
            bool isConnecting = false;

            var logFilePath = $@"{_config.LogDir}\{_ovpnProfileName}.log";

            if (File.Exists(logFilePath))
            {
                var newLogFileName = Guid.NewGuid().ToString();
                var newlogFilePath = $@"{_config.LogDir}\{newLogFileName}.log";

                try
                {
                    File.Copy(logFilePath, newlogFilePath);
                }
                catch
                { }

                var logLines = File.ReadLines(newlogFilePath).Reverse().Take(5);
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

            var ovpnFiles = Directory.GetFiles(_config.ConfigDir, "*.ovpn");

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
    }
}
