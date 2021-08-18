using ExtensionInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StartItUp.Extensions
{
    class Extension
    {
        private string _dllPath;

        private Assembly _dll;
        private Type _type;

        public string Name { get; private set; }

        [JsonProperty("extensionDir")]
        public string Dir { get; private set; }

        public Extension(string extensionDir)
        {
            Name = Path.GetFileNameWithoutExtension(extensionDir);

            Dir = extensionDir;
            _dllPath = Path.Combine(Dir, $"{Name}.dll");

            _dll = Assembly.LoadFrom(_dllPath);
            _type = _dll.GetType(string.Format("{0}.{1}", Name, "Main"));
        }

        public IExtensionInstance CreateInstance()
        {
            var instance = Activator.CreateInstance(_type);
            var extension = new ExtensionInstance(this, _type, instance);
            return extension;
        }
    }
}
