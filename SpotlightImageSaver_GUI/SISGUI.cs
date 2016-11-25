using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotlightImageSaver
{
    public partial class SISGUI : Form
    {
        private bool reallyClose = false;
        public bool startMinimized = false;
        public bool processInBackground = false;
        private bool startup = true;
        private string dirToSaveTo;
        private string typeToSave;

        public SISGUI()
        {
            InitializeComponent();
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }
            startMinimized = Properties.Settings.Default.startMinimized;
            processInBackground = Properties.Settings.Default.runInBackground;
            dirToSaveTo = Properties.Settings.Default.saveToPath;
            if (dirToSaveTo.Equals("%default%"))
            {
                dirToSaveTo = Path.Combine(Environment.GetEnvironmentVariable("userprofile"), "Pictures\\Wallpapers");
            }
            typeToSave = Properties.Settings.Default.typeToSave;
        }

        private void SISGUI_Load(object sender, EventArgs e)
        {
            runInBG.Checked = processInBackground;
            //fileSystemWatcher1.Path = Path.Combine(Environment.GetEnvironmentVariable("userprofile"), "AppData\\Local\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");
            if (!startMinimized)
            {
                timer1.Enabled = false;
            }
            else
            {
                this.Hide();
                if (processInBackground)
                {
                    timer1.Enabled = true;
                }
            }
            startMinCB.Checked = startMinimized;
            whichToSelect.Text = typeToSave;
            doCheck();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reallyClose = true;
            this.Close();
        }

        private void SISGUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!reallyClose)
            {
                this.Hide();
                e.Cancel = true;
                if (processInBackground)
                {
                    timer1.Enabled = true;
                }
                showNotification("Spotlight Image Saver is running in the background.");
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                showNotification("Spotlight Image Saver is running in the background.");
                this.Hide();
            }
            else
            {
                this.Show();
                this.Activate();
            }
        }

        private void runInBG_CheckedChanged(object sender, EventArgs e)
        {
            if (!startup)
            {
                processInBackground = runInBG.Checked;
                Properties.Settings.Default.runInBackground = processInBackground;
                Properties.Settings.Default.Save();
                if (processInBackground)
                {
                    timer1.Enabled = true;
                }
                else
                {
                    timer1.Enabled = false;
                }
            }
        }

        private void doCheck()
        {
            if (!Directory.Exists(dirToSaveTo))
            {
                if (startup)
                {
                    showNotification("Save Directory Does not exist");
                    return;
                }
                if (MessageBox.Show("The Save To directory does not exist, create it?", "Spotlight Image Saver", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        Directory.CreateDirectory(dirToSaveTo);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Could not create save directory.");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            string user_profile = System.Environment.GetEnvironmentVariable("userprofile");
            string path_part2 = "AppData\\Local\\Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets";
            string spotlight_path = Path.Combine(user_profile, path_part2);

            var texistingImages = new List<string>(Directory.GetFiles(dirToSaveTo));
            if (texistingImages == null) return;
            var tnewImages = new List<string>(Directory.GetFiles(spotlight_path));
            if (tnewImages == null) return;

            var tempExisting = new List<string>(texistingImages);
            if (tempExisting == null) return;
            foreach (string item in tempExisting)
            {
                if (!item.EndsWith(".spotlight.jpg"))
                {
                    texistingImages.Remove(item);
                }
            }

            var existingImages = FileProcess.GetJpgInfo(texistingImages);
            if (existingImages == null) return;
            var newImages = FileProcess.GetJpgInfo(tnewImages);
            if (newImages == null) return;

            bool landscapeOnly = false;
            bool portraitOnly = false;

            if (typeToSave.Equals("Landscape Only"))
            {
                landscapeOnly = true;
            }
            if (typeToSave.Equals("Portrait Only"))
            {
                portraitOnly = true;
            }

            newImages = FileProcess.FilterOutInvalid(newImages, portraitOnly, landscapeOnly);
            if (newImages == null) return;
            List<JpgInfo> filesToCopy = FileProcess.FilesToCopy(existingImages, newImages);

            if (filesToCopy != null && filesToCopy.Count > 0)
            {
                foreach (JpgInfo item in filesToCopy)
                {
                    string destFileName = Path.Combine(dirToSaveTo, item.newFileName);
                    File.Copy(item.filepath, destFileName);
                    File.SetLastWriteTime(destFileName, DateTime.Now);
                }
                showNotification(string.Format("You have {0} new backgrounds.", filesToCopy.Count));
            }
        }

        private void showNotification(string msg)
        {
            var x = new NotificationArea();
            x.text = msg;
            x.timeout = 5000;
            x.Show();
        }

        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            doCheck();
        }

        private void saveToButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = dirToSaveTo;
            var res = folderBrowserDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                Properties.Settings.Default.saveToPath = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.Save();
                dirToSaveTo = Properties.Settings.Default.saveToPath;
            }
        }

        private void SISGUI_Shown(object sender, EventArgs e)
        {
            if (startup && startMinimized)
            {
                this.Hide();
                showNotification("Spotlight Image Saver is running in the background.");
            }
            startup = false;
        }

        private void whichToSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!startup)
            {
                typeToSave = whichToSelect.Text;
                Properties.Settings.Default.typeToSave = typeToSave;
                Properties.Settings.Default.Save();
            }
        }

        private void startMinCB_CheckedChanged(object sender, EventArgs e)
        {
            if (!startup)
            {
                startMinimized = startMinCB.Checked;
                Properties.Settings.Default.startMinimized = startMinimized;
                Properties.Settings.Default.Save();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            doCheck();
        }
    }
}