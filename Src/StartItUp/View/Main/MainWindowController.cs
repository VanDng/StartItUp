using StartItUp.Extensions;
using StartItUp.Profiles;
using StartItUp.View.ExtensionSelection;
using System;
using System.Collections.Generic;
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

        public MainWindowController(ProfileManager profileManager, ExtensionManager extensionManager)
        {
            Initialization(profileManager, extensionManager);
        }

        private void Initialization(ProfileManager profileManager, ExtensionManager extensionManager)
        {
            _viewModel = new MainWindowViewModel(profileManager, extensionManager);

            Binding startupProfileListBinding = new Binding(nameof(_viewModel.StartupProfiles));
            startupProfileListBinding.Source = _viewModel;
            BindingOperations.SetBinding(ltvStartupProfiles, ListView.ItemsSourceProperty, startupProfileListBinding);

            btnNew.Command = _viewModel.NewStartupProfile;
            btnEdit.Command = _viewModel.EditStartupProfile;
            btnDelete.Command = _viewModel.DeleteStartupProfile;

            _viewModel.OnAskProfileSelection += _viewModel_OnAskProfileSelection;
            _viewModel.OnEditStartupProfile += _viewModel_OnEditStartupProfile;
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

    }
}
