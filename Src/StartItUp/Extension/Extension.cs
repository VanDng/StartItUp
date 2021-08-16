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
        }

        public void Load()
        {
            _dll = Assembly.LoadFrom(_dllPath);
            _type = _dll.GetType(string.Format("{0}.{1}", Name, "Main"));
        }

        public IExtensionInstance CreateInstance()
        {
            var instance = Activator.CreateInstance(_type);
            var extension = new ExtensionInstance(this, _type, instance);
            return extension;
        }

        //public  Profile[] GetStartupProfiles()
        //{
        //    List<Profile> startupProfiles = new List<Profile>();

        //    var profileDir = Path.Combine(Dir, "Profiles");

        //    if (Directory.Exists(profileDir))
        //    {
        //        var dirs = Directory.GetDirectories(profileDir);
        //        foreach(var dir in dirs)
        //        {
        //            string id = Path.GetFileNameWithoutExtension(dir);
        //            string description = string.Empty;

        //            try
        //            {
        //                var desFilePath = Path.Combine(dir, "Description.txt");
        //                description = File.ReadAllText(desFilePath);
        //            }
        //            catch
        //            { }

        //            Profile profile = new Profile();
        //            profile.Name = id;
        //            profile.Description = description;

        //            startupProfiles.Add(profile);
        //        }
        //    }

        //    return startupProfiles.ToArray();
        //}
    }
}
