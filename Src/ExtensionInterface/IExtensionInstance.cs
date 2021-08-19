using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionInterface
{
    public interface IExtensionInstance
    {
        void Start();
        void Stop();

        void SetConfigDir(string configDir);
        void Config();
    }
}
