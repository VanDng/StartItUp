using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Worksnaps
{
    public partial class ConfigWindow : Form
    {
        public ConfigWindow(int counter)
        {
            InitializeComponent();

            label1.Text = counter.ToString();
        }
    }
}
