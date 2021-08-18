using ExtensionInterface;
using StartItUp.Extensions;
using StartItUp.Profiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StartItUp.View.Model
{
    class StartupProfile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Profile Profile { get; private set; }

        public IExtensionInstance Executor { get; private set; }

        public StartupProfile(Profile profile, IExtensionInstance extensionInstance)
        {
            Profile = profile;
            Executor = extensionInstance;
        }
    }
}
