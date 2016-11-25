using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotlightImageSaver
{
    public partial class NotificationArea : Form
    {
        public int timeout;
        public string text;
        private static NotificationArea showing = null;

        public NotificationArea()
        {
            InitializeComponent();
            if (showing != null)
            {
                showing.Close();
                showing = null;
            }
            showing = this;
        }

        private void NotificationArea_Load(object sender, EventArgs e)
        {
            textBox1.Text = text;

            timer1.Interval = timeout;
            timer1.Start();
            timer1.Enabled = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.Hide();
        }
    }
}