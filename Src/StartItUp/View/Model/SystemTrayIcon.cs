using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartItUp.View.Model
{
    class SystemTrayIcon
    {
        public event EventHandler OnDoubleClickOnSystemTrayIcon;
        public event EventHandler OnExitMenuClicked;

        private NotifyIcon _notifyIcon;

        public SystemTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Visible = true;
            _notifyIcon.Icon = Resource1.app;
            _notifyIcon.DoubleClick += _notifyIcon_DoubleClick;

            _notifyIcon.BalloonTipTitle = "StartItUp";
            _notifyIcon.BalloonTipText = "StartItUp is running in background!";

            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(new MenuItem("Exit", clickExitMenu));
            _notifyIcon.ContextMenu = contextMenu;
        }

        public void ShowToolTip()
        {
            _notifyIcon.ShowBalloonTip(3000);
        }

        private void _notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            OnDoubleClickOnSystemTrayIcon?.Invoke(sender, e);
        }

        private void clickExitMenu(object sender, EventArgs e)
        {
            OnExitMenuClicked?.Invoke(sender, e);
        }
    }
}
