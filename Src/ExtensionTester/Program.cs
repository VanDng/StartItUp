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
            IntPtr hwnd = WindowsAPI.FindWindow(null, "Login");
            IntPtr textHwnd = WindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, "Static", "Cannot connect to the server.Please try again later.");
            IntPtr textHwnd2 = WindowsAPI.FindWindowEx(hwnd, textHwnd, "Static", null);
            string text = WindowsAPI.GetWindowText(textHwnd2);

            // Error
            //    Cannot retrieve the task list. This might be due to temporary issue on the server.\nPlease try again later.
            // Login
            //Cannot connect to the server.Please try again later.

            //IntPtr hwnd = WindowsAPI.FindWindow("#32770", "Login");

            Console.ReadKey();
        }

        static private void ProceedErrorWindow()
        {



            //foreach (var window in windows)
            //{
            //    List<IntPtr> hwndTrackings = new List<IntPtr>();
            //    bool isStop = false;

            //    while (!isStop)
            //    {
            //        IntPtr hWnd = IntPtr.Zero;
            //        hWnd = WindowsAPI.FindWindow(null, window.Key);
            //        if (hWnd != IntPtr.Zero)
            //        {
            //            Debug.WriteLine("Found window");
            //            //hwndTrackings.Add(hWnd);

            //            //WindowsAPI.PostMessage(hWnd, WindowsAPI.WM_SETFOCUS, IntPtr.Zero, IntPtr.Zero);
            //            //WindowsAPI.PostMessage(hWndDesktop, WindowsAPI.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            //            WindowsAPI.SendMessage(hWnd, WindowsAPI.WM_COMMAND, 1, 0);

            //            IntPtr button = WindowsAPI.FindWindowEx(hWnd, IntPtr.Zero, "Button", null);
            //            if (button != IntPtr.Zero)
            //            {
            //                //WindowsAPI.LeftClick(button);
            //            }
            //        }
            //        else
            //        {
            //            isStop = true;
            //        }
            //    }
            //}
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
