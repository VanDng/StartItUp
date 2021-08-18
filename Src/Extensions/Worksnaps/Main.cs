using ExtensionInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Worksnaps
{
    public class Main : IExtensionInstance
    {
        ConfigWindow _configWindow;

        static int counter = 0;

        public Main()
        {
            _configWindow = null;
        }

        public void Start()
        {
            //throw new NotImplementedException();
            Debug.WriteLine("Worksnaps start.");
        }

        public void Stop()
        {
            //throw new NotImplementedException();
            Debug.WriteLine("Worksnaps stop.");
        }

        public void Config()
        {
        }
    }
}
