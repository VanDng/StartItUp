using StartItUp.Extensions;
using StartItUp.Profiles;
using StartItUp.Startup;
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
            // TODO One instance of app is allowed.
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            /*
             * Extension manager
             */
            var appExecutionDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var extensionManager = new ExtensionManager();
            extensionManager.Load(appExecutionDir);

            /*
             * Profile manager
             */
            var appDataDir = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\StartItUp";
            if (!Directory.Exists(appDataDir))
            {
                Directory.CreateDirectory(appDataDir);
            }

            var profileManager = new ProfileManager(appDataDir);
            profileManager.Load();

            /*
             * Startup manager
             */

            var startupManager = new StartupManager(e.Args);

            /*
             * Main Window
             */
            MainWindowController main = new MainWindowController(profileManager, extensionManager, startupManager);
            main.Show();
        }
    }
}
