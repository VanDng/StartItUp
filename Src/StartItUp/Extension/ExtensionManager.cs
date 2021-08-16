using ExtensionInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StartItUp.Extensions
{
    class ExtensionManager
    {
        private List<Extension> _extensions;

        public ReadOnlyCollection<Extension> Extensions
        {
            get
            {
                return _extensions.AsReadOnly();
            }

            private set
            {
            }
        }

        public ExtensionManager()
        {
            _extensions = new List<Extension>();
        }

        public void Load(string rootDir)
        {
            string extensionRootDir = Path.Combine(rootDir, "Extensions");
            var dirs = Directory.EnumerateDirectories(extensionRootDir);

            foreach(var extensionDirectoryPath in dirs)
            {
                Extension extension = new Extension(extensionDirectoryPath);
                extension.Load();

                _extensions.Add(extension);
            }
        }
    }
}
