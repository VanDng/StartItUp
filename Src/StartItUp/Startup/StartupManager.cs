using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StartItUp.Startup
{
    class StartupManager
    {
        private const string AutoLaunchKey = "autolaunch";

        private string _appFilePath;
        private bool _isLaunchedManuallyByEndUser;

        public bool IsAutoLaunchEnabled
        {
            get
            {
                return CommonImplementation.System.Startup.Exists(_appFilePath);
            }
        }

        public bool IsLaunchedManuallyByEndUser
        {
            get
            {
                return _isLaunchedManuallyByEndUser;
            }
        }

        public StartupManager(string[] args)
        {
            _appFilePath = Assembly.GetExecutingAssembly().Location;

            _isLaunchedManuallyByEndUser = !args.Contains(AutoLaunchKey);
        }

        public void SetAutoLaunch(bool isEnabled)
        {
            if (isEnabled)
            {
                CommonImplementation.System.Startup.Set(_appFilePath, "autolaunch");
            }
            else
            {
                CommonImplementation.System.Startup.UnSet(_appFilePath);
            }
        }
    }
}
