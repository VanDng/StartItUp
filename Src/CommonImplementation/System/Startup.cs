using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace CommonImplementation.System
{
    static public class Startup
    {
        static private readonly string StartupPrefix = "__";
        static private readonly string StartupSuffix = "__";

        static private string GetStartupFolderPath()
        {
            /*
             * Startup folder paths:
             *    - Windows 10: C:\Users\[User]\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup
             *    - Other verions: Not tested yet.
             */

            return string.Format(@"C:\Users\{0}\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup",
                                Environment.UserName);
        }

        static private string CreateLinkPath(string linkName)
        {
            return string.Format(@"{0}\{1}{2}{3}.lnk",
                                        GetStartupFolderPath(),
                                        StartupPrefix,
                                        linkName,
                                        StartupSuffix);
        }

        static public bool Exists(string executableFilePath)
        {
            var appName = Path.GetFileNameWithoutExtension(executableFilePath);
            var workingDirectory = Path.GetDirectoryName(executableFilePath);
            var link = CreateLinkPath(appName);

            return File.Exists(link);
        }

        static public void Set(string executableFilePath, string argument = "")
        {
            var appName = Path.GetFileNameWithoutExtension(executableFilePath);
            var workingDirectory = Path.GetDirectoryName(executableFilePath);
            var link = CreateLinkPath(appName);

            if (File.Exists(link))
            {
                File.Delete(link);
            }

            var shell = new WshShell();
            var shortcut = shell.CreateShortcut(link) as IWshShortcut;
            shortcut.TargetPath = executableFilePath;
            shortcut.WorkingDirectory = workingDirectory;
            shortcut.Arguments = argument;
            shortcut.Save();
        }

        static public void UnSet(string executableFilePath)
        {
            var appName = Path.GetFileNameWithoutExtension(executableFilePath);
            var workingDirectory = Path.GetDirectoryName(executableFilePath);
            var link = CreateLinkPath(appName);

            if (File.Exists(link))
            {
                File.Delete(link);
            }
        }
    }
}
