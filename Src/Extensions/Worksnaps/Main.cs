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
        WorksnapStartup _worksnapsStartup;

        public Main()
        {
            _worksnapsStartup = new WorksnapStartup();
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
