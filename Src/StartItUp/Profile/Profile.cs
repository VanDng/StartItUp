using StartItUp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartItUp.Profiles
{
    class Profile
    {
        public bool IsEnabled { get; set; }

        public string Description { get; set; }

        public string ExtensionName { get; set; }

        public string ProfileDirectory { get; set; }
    }
}