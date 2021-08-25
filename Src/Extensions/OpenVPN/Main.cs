using ExtensionInterface;
using Serilog;
using System.IO;

namespace OpenVPN
{
    public class Main : IExtensionInstance
    {
        OpenVPNStartup _worksnapsStartup;

        public Main()
        {


            _worksnapsStartup = new OpenVPNStartup();
        }

        public  void SetConfigDir(string configDir)
        {
            _worksnapsStartup.ConfigDir = configDir;

            _worksnapsStartup.LoadConfig();
            _worksnapsStartup.SaveConfig();


            var logDir = $@"{configDir}\Logs";
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            Log.Logger = new LoggerConfiguration()
                           .MinimumLevel.Debug()
                           .WriteTo.File($@"{logDir}\log.txt", rollingInterval: RollingInterval.Day)
                           .CreateLogger();

            Log.Information(":::: Login Begin ::::");
        }

        public void Start()
        {
            _worksnapsStartup.Start();
        }

        public void Stop()
        {
            _worksnapsStartup.Stop();
        }

        public void Config()
        {
            
        }
    }
}
