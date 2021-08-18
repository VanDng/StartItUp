using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace StartItUp.Profiles
{
    class ProfileManager
    {
        private string _profilesDir;
        private string _profileFilePath;

        private List<Profile> _profiles { get; set; }

        public ReadOnlyCollection<Profile> Profiles
        {
            get
            {
                return _profiles.AsReadOnly();
            }
            private set
            {
            }
        }

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

            foreach(var profile in _profiles)
            {
                profile.ConfigDirectory = $"{_profilesDir}{profile.ConfigDirectory}";
            }
        }

        public void Save()
        {
            var saveProfiles = _profiles.Select(p => new Profile()
            {
                IsEnabled = p.IsEnabled,
                Description = p.Description,
                ExtensionName = p.ExtensionName,
                ConfigDirectory = p.ConfigDirectory.Replace(_profilesDir, string.Empty)
            });

            var profileJson = JsonConvert.SerializeObject(saveProfiles, Formatting.Indented);

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

            string configProfileDir = string.Empty;
            do
            {
                configProfileDir = Path.Combine(_profilesDir, Guid.NewGuid().ToString());
            } while (Directory.Exists(configProfileDir));

            Directory.CreateDirectory(configProfileDir);

            //
            // Initialize profile
            //

            profile.IsEnabled = false;
            profile.Description = extensionName; // Default description is the extension name.
                                                 // User can change it in the extension's configuration UI (if it's available) later.
            profile.ExtensionName = extensionName;
            profile.ConfigDirectory = configProfileDir;

            _profiles.Add(profile);

            return profile;
        }

        public void Delete(Profile profile)
        {
            _profiles.Remove(profile);
        }
    }
}
