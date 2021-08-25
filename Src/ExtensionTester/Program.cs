using CommonImplementation.WindowsAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ExtensionTester
{
    class Program
    {
        private const string LoginWindow = " Worksnaps Client - Login";

        

        static void Main(string[] args)
        {
            Timer t1 = new Timer((o) =>
            {
                ProceedWindow(LoginWindow);
            }, null, 0, 1000);

            Timer t2 = new Timer((o) =>
            {
                ProceedErrorWindow();
            }, null, 0, 1000);


            Timer t3 = null;
            t3 = new Timer((o) =>
            {
                t1.Change(Timeout.Infinite, 0);
                t3.Change(Timeout.Infinite, 0);
            }, null, 5000, 1000);
            

            Console.WriteLine("stop proceed login");

            Console.ReadKey();
        }

        static private void ProceedErrorWindow()
        {
            List<KeyValuePair<string, string>> windows = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Login", "Cannot connect to the server. Please try again later")
            };

            IntPtr hWndDesktop = WindowsAPI.FindWindow(null, LoginWindow);
            if (hWndDesktop != IntPtr.Zero)
            {
                foreach (var window in windows)
                {
                    List<IntPtr> hwndTrackings = new List<IntPtr>();
                    bool isStop = false;

                    while (!isStop)
                    {
                        IntPtr hWnd = IntPtr.Zero;
                        hWnd = WindowsAPI.FindWindow(null, window.Key);
                        if (hWnd != IntPtr.Zero)
                        {
                            Debug.WriteLine("Found window");
                            //hwndTrackings.Add(hWnd);

                            //WindowsAPI.PostMessage(hWnd, WindowsAPI.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
                            //WindowsAPI.PostMessage(hWndDesktop, WindowsAPI.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                            WindowsAPI.SendMessage(hWnd, WindowsAPI.WM_COMMAND, 1, 0);

                            IntPtr button = WindowsAPI.FindWindowEx(hWnd, IntPtr.Zero, "Button", null);
                            if (button != IntPtr.Zero)
                            {
                                //WindowsAPI.LeftClick(button);
                            }
                        }
                        else
                        {
                            isStop = true;
                        }
                    }
                }

            }
        }

        static private void ProceedWindow(string window)
        {
            Debug.WriteLine(window + " is proceeding.");

            string[] buttons = new string[] { };

            switch (window)
            {
                case LoginWindow:
                    buttons = new string[] { "Log In" };
                    break;

                //case SelectProjectWindow:
                //    buttons = new string[] { "OK" };
                //    break;

                //case TaskInformationWindow:
                //    buttons = new string[] { "Start" };
                //    break;
            }

            IntPtr hWnd = WindowsAPI.FindWindow(null, window);
            if (hWnd != IntPtr.Zero)
            {
                foreach (var button in buttons)
                {
                    IntPtr hWndButton = WindowsAPI.FindWindowEx(hWnd, IntPtr.Zero, null, button);
                    if (hWndButton != IntPtr.Zero)
                    {
                        WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                        WindowsAPI.PostMessage(hWndButton, WindowsAPI.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }
    }
}
