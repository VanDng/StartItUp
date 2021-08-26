using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CommonImplementation.WindowsAPI
{

    public static partial class WindowsAPI
    {
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wparam, int lparam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern long GetWindowLong64(IntPtr hWnd, int nIndex);

        public static long GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLong64(hWnd, nIndex);
            else
                return GetWindowLong32(hWnd, nIndex);
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowText(IntPtr hWnd)
        {
            Int32 titleSize = SendMessage(hWnd, (int)WM_GETTEXTLENGTH, 0, 0).ToInt32();

            StringBuilder title = null;

            if (titleSize != 0)
            {
                title = new StringBuilder(titleSize + 1);

                SendMessage(hWnd, WM_GETTEXT, title.Capacity, title);
            }

            return title == null ? string.Empty : title.ToString();
        }

        public static void LeftClick(IntPtr hwnd)
        {
            WindowsAPI.PostMessage(hwnd, WindowsAPI.WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            WindowsAPI.PostMessage(hwnd, WindowsAPI.WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
