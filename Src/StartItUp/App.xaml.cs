using StartItUp.Extensions;
using StartItUp.Profiles;
using StartItUp.View.Main;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace StartItUp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var profileManager = new ProfileManager(rootDir);
            profileManager.Load();

            var extensionManager = new ExtensionManager();
            extensionManager.Load(rootDir);

            MainWindowController main = new MainWindowController(profileManager, extensionManager);
            main.Show();
        }
    }
}
