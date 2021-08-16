using StartItUp.Extensions;
using StartItUp.Profiles;
using StartItUp.View.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StartItUp.View.ExtensionSelection
{
    class ExtensionSelectionViewModel
    {
        public ObservableCollection<string> AvailabelExtensions { get; set; }

        public string SelectedProfileName { get; set; }

        public delegate void OnAcceptProfileEventHandler(string extensionName);

        public event OnAcceptProfileEventHandler OnAcceptProfile;

        private ExtensionManager _extensionManager;

        public ICommand AcceptProfile { get; private set; }

        public ExtensionSelectionViewModel(ExtensionManager extensionManager)
        {
            _extensionManager = extensionManager;

            AvailabelExtensions = new ObservableCollection<string>();
            
            foreach(var extension in _extensionManager.Extensions)
            {
                AvailabelExtensions.Add(extension.Name);
            }

            AcceptProfile = new RelayCommand<object>(ExecuteAcceptProfileCommand, CanAcceptProfileCommandExecuted);
        }

        private void ExecuteAcceptProfileCommand(object o)
        {
            OnAcceptProfile?.Invoke(SelectedProfileName);
        }

        private bool CanAcceptProfileCommandExecuted(object o)
        {
            return !string.IsNullOrEmpty(SelectedProfileName);
        }
    }
}
