using ExtensionInterface;
using System.IO;

namespace Worksnaps
{
    public class Main : IExtensionInstance
    {
        WorksnapStartup _worksnapsStartup;

        public Main()
        {
            _worksnapsStartup = new WorksnapStartup();
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
