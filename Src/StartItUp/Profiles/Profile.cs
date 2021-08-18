using StartItUp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StartItUp.Profiles
{
    class Profile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // TODO Upgrade to a higher .NET Framework version
        // then use Fody.PropertyChanged NuGet to reduce implementation.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string Description { get; set; }

        public string ExtensionName { get; set; }

        public string ConfigDirectory { get; set; }
    }
}