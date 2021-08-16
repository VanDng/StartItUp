using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StartItUp.Profiles
{
    class ProfileManager
    {
        private string _profilesDir;
        private string _profileFilePath;

        private List<Profile> _profiles { get; set; }

        public ProfileManager(string rootDir)
        {
            _profilesDir = Path.Combine(rootDir, "Profiles");
            if (!Directory.Exists(_profilesDir))
            {
                Directory.CreateDirectory(_profilesDir);
            }

            _profileFilePath = Path.Combine(rootDir, "Profiles.json");

            _profiles = new List<Profile>();
        }

        public void Load()
        {
            string profileJson = string.Empty;
            try
            {
                profileJson = File.ReadAllText(_profileFilePath);
            }
            catch
            { }

            _profiles = JsonConvert.DeserializeObject<List<Profile>>(profileJson);

            if (_profiles == null)
            {
                _profiles = new List<Profile>();
            }
        }

        public void Save()
        {
            var profileJson = JsonConvert.SerializeObject(_profiles, Formatting.Indented);

            try
            {
                File.WriteAllText(_profileFilePath, profileJson);
            }
            catch
            { }
        }

        public Profile New(string extensionName)
        {
            Profile profile = new Profile();

            //
            // Prepare profile directory
            //

            string profileDir = string.Empty;
            do
            {
                profileDir = Path.Combine(_profilesDir, Guid.NewGuid().ToString());
            } while (Directory.Exists(profileDir));

            Directory.CreateDirectory(profileDir);

            //
            // Initialize profile
            //

            profile.IsEnabled = false;
            profile.Description = string.Empty;
            profile.ExtensionName = extensionName;
            profile.ProfileDirectory = profileDir;

            _profiles.Add(profile);

            return profile;
        }

        public void Delete(Profile profile)
        {
            _profiles.Remove(profile);
        }

        public Profile[] GetProfiles()
        {
            return _profiles.ToArray();
        }
    }
}
