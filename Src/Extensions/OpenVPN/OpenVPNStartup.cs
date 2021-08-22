using CommonImplementation.WindowsAPI;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace OpenVPN
{
    class OpenVPNStartup : IDisposable
    {
        private string _ovpnProfileName;

        private Thread _thread;
        private bool _stopWorking;

        private const string ConfigFileName = "config.json";
        private Config _config;
        private Config _defaultConfig;

        public string ConfigDir { get; set; }

        public OpenVPNStartup()
        {
            _ovpnProfileName = string.Empty;

            _config = null;

            _defaultConfig = new Config()
            {
                ServiceProcessName = "openvpn",
                GuiProcessName = "openvpn-gui",
                ClientFilePath = @"C:\Program Files\OpenVPN\bin\openvpn-gui.exe",
                ConfigDir = string.Format(@"C:\Users\{0}\OpenVPN\{1}", Environment.UserName, "config"),
                LogDir = string.Format(@"C:\Users\{0}\OpenVPN\{1}", Environment.UserName, "log"),
                OVPNKeyword = "office-vpn.websparks.sg",
                RegistryConfigDir = @"HKEY_CURRENT_USER\SOFTWARE\OpenVPN-GUI"
            };
        }

        public void Start()
        {
            _stopWorking = false;

            if (_thread == null ||
                !_thread.IsAlive)
            {
                _thread = new Thread(MainProc);
                _thread.Start();
            }
        }

        public void Stop()
        {
            _stopWorking = true;
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

        private void MainProc()
        {
            while (true)
            {
                if (_stopWorking) break;

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
                     * Then, OpenVPN service/GUI will be shutdown and start all over again.
                     */

                    StopOpenVPNService();
                }

                if (!IsSilentConnectionSet())
                {
                    SetSilentConnectionMode();
                }

                if (IsLaunched())
                {
                    if (!IsConnecting())
                    {
                        Connect();
                    }
                }
                else
                {
                    Connect();
                }

                if (_stopWorking) break;

                Thread.Sleep(1000);
            }
        }

        private bool IsLaunched()
        {
            bool isLaunched = false;

            Process[] processCollection = Process.GetProcesses();

            foreach (var process in processCollection)
            {
                if (process.ProcessName.ToLower() == _config.GuiProcessName.ToLower())
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
            var serviceProcesses = Process.GetProcessesByName(_config.ServiceProcessName);
            var guiProcesses = Process.GetProcessesByName(_config.GuiProcessName);

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

        private bool IsConnecting()
        {
            bool isConnecting = false;

            bool isConnectedStringLogged = false;
            DateTime? logDateTime = null;

            isConnectedStringLogged = IsConnectedStringLogged(out logDateTime);

            DateTime? processDateTime = ProcessStartTime();

            if (processDateTime == null ||
                logDateTime == null)
            {
                Debug.WriteLine("Can not get necessary information to detect whether it is connecting.");

                string logDateTimeString = logDateTime == null ? "N/A" : logDateTime.Value.ToString();
                Debug.WriteLine("Log date time: " + logDateTimeString);

                string processDateTimeString = processDateTime == null ? "N/A" : processDateTime.Value.ToString();
                Debug.WriteLine("Process date time: " + processDateTimeString);

                isConnecting = isConnectedStringLogged;
            }
            else
            {
                /*
                 * Somehome the OpenVPN GUI app is running and its connection status is not connected.
                 * but the old log content is remaining.
                 * 
                 * Then, the connection detecting will make a false positive conclusion.
                 * 
                 * That is why the process start time is involved.
                 * 
                 */

                if (isConnectedStringLogged)
                {
                    isConnecting = logDateTime.Value >= processDateTime.Value ? true : false;
                }
                else
                {
                    isConnecting = false;
                }
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

        private bool IsConnectedStringLogged(out DateTime? logDateTime)
        {
            bool isConnectedStringLogged = false;
            logDateTime = null;

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
                        isConnectedStringLogged = true;
                        logDateTime = ParseDateTime(log);

                        break;
                    }
                }
            }
            else
            {
                Debug.WriteLine("Could not find log file.");
            }

            return isConnectedStringLogged;
        }

        private DateTime? ParseDateTime(string log)
        {
            // Sample "Sun Aug 22 09:57:55 2021 Initialization Sequence Completed"

            DateTime? dateTimeLog = null;

            string[] logParts = log.Split(' ');
            string yearString = logParts[4];
            string monthString = logParts[1];
            string dayString = logParts[2];
            string timeString = logParts[3];

            int year = 0;
            int.TryParse(yearString, out year);

            int month = 0;
            Hashtable months = new Hashtable()
            {
                { "Jan", "1" },
                { "Feb", "2" },
                { "Mar", "3" },
                { "Apr", "4" },
                { "May", "5" },
                { "Jun", "6" },
                { "Jul", "7" },
                { "Aug", "8" },
                { "Sep", "9" },
                { "Oct", "10" },
                { "Nov", "11" },
                { "Dec", "12" }
            };
            if (months.ContainsKey(monthString))
            {
                month = int.Parse(months[monthString].ToString());
            }

            int day = 0;
            int.TryParse(dayString, out day);
            if (year > 0 && month > 0)
            {
                int daysInMonth = DateTime.DaysInMonth(year, month);
                day = daysInMonth < day ? daysInMonth : day;
            }

            DateTime? time = null;
            DateTime tryparseTime;
            if (DateTime.TryParse(timeString, out tryparseTime))
            {
                time = tryparseTime;
            }

            if (year > 0 && month > 0 && day > 0 && time != null)
            {
                dateTimeLog = new DateTime(year, month, day,
                                            time.Value.Hour, time.Value.Minute, time.Value.Second);
            }

            return dateTimeLog;
        }

        private DateTime? ProcessStartTime()
        {
            DateTime? dateTimeProcess = null;

            var guiProcess = Process.GetProcessesByName(_config.GuiProcessName).FirstOrDefault();
            if (guiProcess != null)
            {
                dateTimeProcess = guiProcess.StartTime;
            }

            return dateTimeProcess;
        }

        public void Dispose()
        {
        }
    }
}
