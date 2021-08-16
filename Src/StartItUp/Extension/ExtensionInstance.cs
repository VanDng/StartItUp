using ExtensionInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartItUp.Extensions
{
    class ExtensionInstance : IExtensionInstance
    {
        private object _instance;
        private Type _type;

        public Extension Extension { get; private set; }

        public ExtensionInstance(Extension extension, Type type, object instance)
        {
            Extension = extension;

            _type = type;
            _instance = instance;
        }

        public void Config()
        {
            var method = _type.GetMethod("Config");
            method.Invoke(_instance, null);
        }

        public void Start()
        {
            var method = _type.GetMethod("Start");
            method.Invoke(_instance, null);
        }

        public void Stop()
        {
            var method = _type.GetMethod("Stop");
            method.Invoke(_instance, null);
        }
    }
}
