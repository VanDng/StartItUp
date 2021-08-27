using StartItUp.Extensions;
using StartItUp.Profiles;
using StartItUp.Startup;
using StartItUp.View.ExtensionSelection;
using StartItUp.View.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace StartItUp.View.Main
{
    class MainWindowController : MainWindow
    {
        private MainWindowViewModel _viewModel;
        private StartupManager _startupManager;

        private SystemTrayIcon _systemTrayIcon;
        private bool _isAppForcedToClose;

        public MainWindowController(ProfileManager profileManager,
                                    ExtensionManager extensionManager,
                                    StartupManager startupManager)
        {
            _startupManager = startupManager;

            Initialization(profileManager, extensionManager, startupManager);
        }

        private void Initialization(ProfileManager profileManager,
                                    ExtensionManager extensionManager,
                                    StartupManager startupManager)
        {
            _viewModel = new MainWindowViewModel(profileManager, extensionManager, startupManager);

            _isAppForcedToClose = false;

            _systemTrayIcon = new SystemTrayIcon();
            _systemTrayIcon.OnDoubleClickOnSystemTrayIcon += (s, e) =>
            {
                ShowWindow(true);
            };
            _systemTrayIcon.OnExitMenuClicked += (s, e) =>
            {
                _isAppForcedToClose = true;
                Close();
            };

            Binding autoStartAppWithSystemBinding = new Binding(nameof(_viewModel.AutoStartApplicationWithSystem));
            autoStartAppWithSystemBinding.Source = _viewModel;
            BindingOperations.SetBinding(cbAutoStartupWithSystem, CheckBox.IsCheckedProperty, autoStartAppWithSystemBinding);

            Binding startupProfileListBinding = new Binding(nameof(_viewModel.StartupProfiles));
            startupProfileListBinding.Source = _viewModel;
            BindingOperations.SetBinding(ltvStartupProfiles, ListView.ItemsSourceProperty, startupProfileListBinding);

            Binding selectedStartupProfile = new Binding(nameof(_viewModel.SelectedStartupProfile));
            selectedStartupProfile.Source = _viewModel;
            BindingOperations.SetBinding(ltvStartupProfiles, ListView.SelectedItemProperty, selectedStartupProfile);

            btnNew.Command = _viewModel.NewStartupProfile;
            btnEdit.Command = _viewModel.EditStartupProfile;
            btnDelete.Command = _viewModel.DeleteStartupProfile;

            Initialized += MainWindowController_Initialized;
            Loaded += MainWindowController_Loaded;
            Closing += MainWindowController_Closing;

            _viewModel.OnAskProfileSelection += _viewModel_OnAskProfileSelection;
            _viewModel.OnEditStartupProfile += _viewModel_OnEditStartupProfile;
            _viewModel.OnDeleteStartupProfile += _viewModel_OnDeleteStartupProfile;
        }

        public new void Show()
        {
            if (_startupManager.IsLaunchedManuallyByEndUser)
            {
                ShowWindow();
            }
            else
            {
                HideWindow();
            }
        }

        private void MainWindowController_Initialized(object sender, EventArgs e)
        {
            
        }

        private void MainWindowController_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MainWindowController_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isAppForcedToClose)
            {
                e.Cancel = true;
                HideWindow();
            }
        }

        private void HideWindow()
        {
            Visibility = Visibility.Hidden;
        }

        private void ShowWindow(bool isActive = false)
        {
            Visibility = Visibility.Visible;

            if (isActive)
            {
                Activate();
                Focus();
            }
        }

        private void _viewModel_OnAskProfileSelection(ExtensionManager extensionManager)
        {
            ExtensionSelectionWindowController extensionSelectionWindow = new ExtensionSelectionWindowController(extensionManager);
            extensionSelectionWindow.Owner = this;
            extensionSelectionWindow.ShowDialog();

            var selectedProfile = extensionSelectionWindow.ViewModel.SelectedProfileName;
            _viewModel.CreateNewProfile.Execute(selectedProfile);
        }

        private void _viewModel_OnEditStartupProfile(object sender, EventArgs e)
        {
            MessageBox.Show("Not Implemented.");
        }

        private void _viewModel_OnDeleteStartupProfile(object sender, EventArgs e)
        {
        }
    }
}
