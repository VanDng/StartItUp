using CommonImplementation.Extensions;
using StartItUp.Extensions;
using StartItUp.Profiles;
using StartItUp.View.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StartItUp.View.Main
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
#pragma warning disable 67
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67

        public delegate void OnAskProfileSelectionEventHandler(ExtensionManager profileManager);

        public event OnAskProfileSelectionEventHandler OnAskProfileSelection;
        public event EventHandler OnEditStartupProfile;
        public event EventHandler OnDeleteStartupProfile;

        public ICommand NewStartupProfile { get; private set; }
        public ICommand EditStartupProfile { get; private set; }
        public ICommand DeleteStartupProfile { get; private set; }

        public ICommand CreateNewProfile { get; private set; }

        public ObservableCollection<Profile> StartupProfiles { get; set; }

        private ProfileManager _profileManager;
        private ExtensionManager _extensionManager;

        public MainWindowViewModel(ProfileManager profileManager, ExtensionManager extensionManager)
        {
            PropertyChanged += MainWindowViewModel_PropertyChanged;

            NewStartupProfile = new RelayCommand<object>(ExecuteNewStartupProfileCommand);
            EditStartupProfile = new RelayCommand<object>(ExecuteEditStartupProfileCommand);
            DeleteStartupProfile = new RelayCommand<object>(ExecuteDeleteStartupProfileCommand);

            CreateNewProfile = new RelayCommand<object>(ExecuteCreateNewProfileCommand);
           
            StartupProfiles = new ObservableCollection<Profile>();

            _profileManager = profileManager;
            ReloadProfiles();

            StartupProfiles.Add(new Profile() { IsEnabled = true, Description = "A" });
            StartupProfiles.Add(new Profile() { IsEnabled = false, Description = "B" });

            _extensionManager = extensionManager;
        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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
            string selectedProfile = (string)o;
            _profileManager.New(selectedProfile);

            ReloadProfiles();
        }

        private void ReloadProfiles()
        {
            _profileManager.Save();
            _profileManager.Load();

            StartupProfiles.Clear();
            StartupProfiles.AddRange(_profileManager.GetProfiles());
        }
    }
}
