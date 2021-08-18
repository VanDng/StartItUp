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
        private string _appFilePath;

        public bool IsAutoLaunchEnabled
        {
            get
            {
                return CommonImplementation.System.Startup.Exists(_appFilePath);
            }
        }

        public StartupManager()
        {
            _appFilePath = Assembly.GetExecutingAssembly().Location;
        }

        public void SetAutoLaunch(bool isEnabled)
        {
            if (isEnabled)
            {
                CommonImplementation.System.Startup.Set(_appFilePath);
            }
            else
            {
                CommonImplementation.System.Startup.UnSet(_appFilePath);
            }
        }
    }
}
