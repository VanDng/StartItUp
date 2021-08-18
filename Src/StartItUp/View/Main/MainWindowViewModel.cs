using CommonImplementation.Extensions;
using CommonImplementation.System;
using StartItUp.Extensions;
using StartItUp.Profiles;
using StartItUp.Startup;
using StartItUp.View.Common;
using StartItUp.View.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StartItUp.View.Main
{
    class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public delegate void OnAskProfileSelectionEventHandler(ExtensionManager profileManager);

        public event OnAskProfileSelectionEventHandler OnAskProfileSelection;
        public event EventHandler OnEditStartupProfile;
        public event EventHandler OnDeleteStartupProfile;

        public ICommand NewStartupProfile { get; private set; }
        public ICommand EditStartupProfile { get; private set; }
        public ICommand DeleteStartupProfile { get; private set; }

        public ICommand CreateNewProfile { get; private set; }

        public ObservableCollection<StartupProfile> StartupProfiles { get; set; }

        private bool _AutoStartApplicationWithSystem;
        public bool AutoStartApplicationWithSystem
        {
            get
            {
                return _AutoStartApplicationWithSystem;
            }
            set
            {
                _AutoStartApplicationWithSystem = value;
                NotifyPropertyChanged();
            }
        }

        private ProfileManager _profileManager;
        private ExtensionManager _extensionManager;
        private StartupManager _startupManager;

        public MainWindowViewModel(ProfileManager profileManager,
                                   ExtensionManager extensionManager,
                                   StartupManager startupManager)
        {
            _profileManager = profileManager;
            _extensionManager = extensionManager;
            _startupManager = startupManager;

            NewStartupProfile = new RelayCommand<object>(ExecuteNewStartupProfileCommand);
            EditStartupProfile = new RelayCommand<object>(ExecuteEditStartupProfileCommand);
            DeleteStartupProfile = new RelayCommand<object>(ExecuteDeleteStartupProfileCommand);

            CreateNewProfile = new RelayCommand<object>(ExecuteCreateNewProfileCommand);
           
            AutoStartApplicationWithSystem = _startupManager.IsAutoLaunchEnabled;

            InitializeStartupProfiles();

            PropertyChanged += MainWindowViewModel_PropertyChanged;
        }

        private void StartupProfiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;

            foreach (var newProfile in e.NewItems.Cast<StartupProfile>())
            {
                /*
                 * When the application starts up,
                 * all profiles are executed once.
                 */
                ExecuteProfile(newProfile);

                /*
                 * Whenever a users disable/enable a profile on GUI,
                 * that profile will be started/stopped working correspondingly.
                 */
                newProfile.Profile.PropertyChanged += (s, pe) =>
                {
                    ExecuteProfile(newProfile);
                };
            }
        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AutoStartApplicationWithSystem))
            {
                SetAutoStartApplicationWithSystem(AutoStartApplicationWithSystem);
            }
        }

        private void ExecuteNewStartupProfileCommand(object o)
        {
            OnAskProfileSelection?.Invoke(_extensionManager);
        }

        private void ExecuteEditStartupProfileCommand(object o)
        {
            OnEditStartupProfile?.Invoke(null, null);
        }

        private void ExecuteDeleteStartupProfileCommand(object o)
        {
            OnDeleteStartupProfile?.Invoke(null, null);
        }

        private void ExecuteCreateNewProfileCommand(object o)
        {
            string selectedProfile = o as string;
            if (string.IsNullOrEmpty(selectedProfile)) return;

            var profile = _profileManager.New(selectedProfile);

            var startupProfile = MapProfileWithExtension(profile, _extensionManager);
            if (startupProfile == null)
            {
                _profileManager.Delete(profile);
            }
            else
            {
                StartupProfiles.Add(startupProfile);
                _profileManager.Save();
            }
        }

        private void InitializeStartupProfiles()
        {
            StartupProfiles = new ObservableCollection<StartupProfile>();
            StartupProfiles.CollectionChanged += StartupProfiles_CollectionChanged;

            var profiles = _profileManager.Profiles;
            foreach (var profile in profiles)
            {
                var startupProfile = MapProfileWithExtension(profile, _extensionManager);
                StartupProfiles.Add(startupProfile);
            }
        }

        private StartupProfile MapProfileWithExtension(Profile profile, ExtensionManager extensionManager)
        {
            StartupProfile startupProfile = null;

            var extension = extensionManager.Extensions
                                                .Where(e => e.Name.ToLower() == profile.ExtensionName.ToLower())
                                                .FirstOrDefault();

            if (extension == null)
            {
                // TODO Log an error of not found extension.
            }
            else
            {
                var extensionInstance = extension.CreateInstance();

                startupProfile = new StartupProfile(profile, extensionInstance);
            }

            return startupProfile;
        }

        private void ExecuteProfile(StartupProfile profile)
        {
            if (profile.Profile.IsEnabled)
            {
                profile.Executor.Start();
            }
            else
            {
                profile.Executor.Stop();
            }
        }

        private void SetAutoStartApplicationWithSystem(bool isEnabled)
        {
            _startupManager.SetAutoLaunch(isEnabled);
        }

        public void Dispose()
        {
            var runningProfiles = StartupProfiles.Where(p => p.Profile.IsEnabled);
            foreach (var profile in runningProfiles)
            {
                profile.Executor.Stop();
            }
        }
    }
}
