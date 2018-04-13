using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Xml;

namespace mkConnectNAS
{
    public partial class mkConnectNAS : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        [DllImport("mpr.dll")]
        static extern UInt32 WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, uint dwFlags);

        const uint RESOURCETYPE_DISK = 1;

        Dictionary<string, string> drives = new Dictionary<string, string>();
        Dictionary<string, PictureBox> pictureboxes = new Dictionary<string, PictureBox>();

        string username;
        string password;

        public mkConnectNAS()
        {
            InitializeComponent();
            this.init();
            this.connectAll();
        }

        private void buttonConnectAll_Click(object sender, EventArgs e)
        {
            this.connectAll();
        }

        public void init()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node;
            PictureBox pictureBox;
            Label label;
            int preTop = 0;
            Boolean firstRun= true;
            int i = 0;
            int lastBottom = 0;

            doc.Load(Application.StartupPath + @"\config.xml");

            node = doc.DocumentElement.SelectSingleNode("/config/credentials/username");
            username = node.InnerText;

            node = doc.DocumentElement.SelectSingleNode("/config/credentials/password");
            password = node.InnerText;

            foreach (XmlNode drive in doc.DocumentElement.SelectSingleNode("/config/drives").ChildNodes)
            {
                drives.Add(drive.Attributes["letter"]?.InnerText, drive.Attributes["path"]?.InnerText);
            }
            
            foreach (KeyValuePair<string, string> drive in drives)
            {
                if (firstRun)
                {
                    preTop = buttonConnectAll.Bottom - 25;
                    firstRun = false;
                }

                pictureBox = new PictureBox();

                if (i % 2 == 0)
                {
                    pictureBox.Top = preTop + 50;
                    pictureBox.Left = buttonConnectAll.Left;
                }
                else
                {
                    pictureBox.Top = preTop;
                    pictureBox.Left = buttonConnectAll.Left + 100;
                }
                
                pictureBox.Image    = Properties.Resources.Delete_Database_96;
                pictureBox.Height   = 32;
                pictureBox.Width    = 32;
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

                label = new Label();
                label.Left      = pictureBox.Right + 5;
                label.Top       = pictureBox.Top + 4;
                label.Text      = drive.Key + ":";
                label.Width     = 32;
                label.Height    = 32;
                label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                label.Font      = new Font("Consolas", 16);

                this.Controls.Add(pictureBox);
                this.Controls.Add(label);
                
                pictureboxes.Add(drive.Key, pictureBox);

                preTop = pictureBox.Top;
                lastBottom = pictureBox.Bottom;
                i++;
            }

            this.Height = lastBottom + 60;
        }

        public void connectAll()
        {
            NETRESOURCE networkResource;
            PictureBox pictureBox;

            foreach (KeyValuePair<string, string> drive in drives)
            {
                pictureboxes.TryGetValue(drive.Key, out pictureBox);

                networkResource = new NETRESOURCE();
                networkResource.dwType = RESOURCETYPE_DISK;
                networkResource.lpLocalName = drive.Key + ":";
                networkResource.lpRemoteName = drive.Value;
                networkResource.lpProvider = null;

                uint result = WNetAddConnection2(ref networkResource, password, username, 0);

                switch (result)
                {
                    case 0:  // ERROR_SUCCESS 0(0x0) The operation completed successfully.
                    case 85: // ERROR_ALREADY_ASSIGNED 85(0x55) The local device name is already in use.
                        pictureBox.Image = Properties.Resources.Accept_Database_96;
                        break;

                    default:
                        //checkbox.Checked = false;
                        pictureBox.Image = Properties.Resources.Delete_Database_96;
                        break;
                }
            }
        }

        private void mkConnectNAS_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Visible = true;
                this.ShowInTaskbar = false;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }
    }
}
