using StartItUp.Profiles;
using StartItUp.View.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StartItUp.View.Main
{
    /// <summary>
    /// Interaction logic for ProfileUC.xaml
    /// </summary>
    public partial class ProfileUC : UserControl
    {
        public ProfileUC()
        {
            InitializeComponent();

            DataContextChanged += ProfileUC_DataContextChanged;
        }

        private void ProfileUC_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null) return;

            StartupProfile startUpProfile = DataContext as StartupProfile;
            if (startUpProfile == null) return;

            Profile profile = startUpProfile.Profile;

            string x = $"{nameof(startUpProfile.Profile)}.{nameof(profile.IsEnabled)}";
            Binding enableProfileBinding = new Binding($"{nameof(startUpProfile.Profile)}.{nameof(profile.IsEnabled)}");
            enableProfileBinding.Source = DataContext;
            enableProfileBinding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(chkIsEnabled, CheckBox.IsCheckedProperty, enableProfileBinding);

            Binding profileDescriptionBinding = new Binding($"{nameof(startUpProfile.Profile)}.{nameof(profile.Description)}");
            profileDescriptionBinding.Source = DataContext;
            BindingOperations.SetBinding(lblProfileDescription, Label.ContentProperty, profileDescriptionBinding);
        }
    }
}
