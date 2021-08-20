using ExtensionInterface;
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
