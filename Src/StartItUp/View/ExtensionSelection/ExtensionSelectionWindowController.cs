using StartItUp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace StartItUp.View.ExtensionSelection
{
    class ExtensionSelectionWindowController : ExtensionSelectionWindow
    {
        public ExtensionSelectionViewModel ViewModel;

        public ExtensionSelectionWindowController(ExtensionManager extensionManager)
        {
            ViewModel = new ExtensionSelectionViewModel(extensionManager);

            Binding availableExtensionBinding = new Binding(nameof(ViewModel.AvailabelExtensions));
            availableExtensionBinding.Source = ViewModel;
            BindingOperations.SetBinding(ltvExtensions, ListView.ItemsSourceProperty, availableExtensionBinding);

            Binding selectedProfileBinding = new Binding(nameof(ViewModel.SelectedProfileName));
            selectedProfileBinding.Source = ViewModel;
            BindingOperations.SetBinding(ltvExtensions, ListView.SelectedItemProperty, selectedProfileBinding);

            btnAccept.Command = ViewModel.AcceptProfile;

            ViewModel.OnAcceptProfile += _viewModel_OnAcceptProfile;
        }

        private void _viewModel_OnAcceptProfile(string extensionName)
        {
            Close();
        }
    }
}
