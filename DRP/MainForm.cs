﻿using System;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using DiscordRPC;
using Timer = System.Windows.Forms.Timer;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DRP
{
    public partial class mainForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

		DiscordRpcClient client;
		RichPresence presence;

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();

        public mainForm()
        {
            InitializeComponent();
        }

        public void initClient()
        {
            if (DetailsTB.Text != "" && StateLbl.Text != "" && clientIDTB.Text != "")
            {

				client = new DiscordRpcClient(clientIDTB.Text);
				if (client.Initialize())
				{

					if (!isEndCB.Checked)
					{
						presence = new RichPresence()
						{
							Details = DetailsTB.Text,
							State = StateTB.Text,
							Timestamps = new Timestamps()
							{
								Start = DateTime.UtcNow
							},
							Assets = new Assets()
							{
								LargeImageKey = largeImageKeyTB.Text,
								SmallImageKey = smallImageKeyTB.Text,
								LargeImageText = largeImageTextTB.Text,
								SmallImageText = smallImageTextTB.Text
							}
						};

					}
					else
					{
						presence = new RichPresence()
						{
							Details = DetailsTB.Text,
							State = StateTB.Text,
							Timestamps = new Timestamps()
							{
								Start = DateTime.UtcNow,
								End = DateTime.UtcNow + TimeSpan.FromSeconds(Convert.ToDouble(endTimeTB.Text))
							},
							Assets = new Assets()
							{
								LargeImageKey = largeImageKeyTB.Text,
								SmallImageKey = smallImageKeyTB.Text,
								LargeImageText = largeImageTextTB.Text,
								SmallImageText = smallImageTextTB.Text
							}
						};
					}

					client.SetPresence(presence);
				} else
					MessageBox.Show("Could not init discrod rpc");

			}
            else
            {
               MessageBox.Show("Please Field Text Boxes:\n[Details] [State] [ClientID]", "Warning", MessageBoxButtons.OK);
                Application.Restart();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			LoadConfig();
		}

        private void timer_Tick(object sender, EventArgs e)
        {
			userdispname.Text = client.CurrentUser.Username;
		}

        private void button1_Click(object sender, EventArgs e)
        {
            initClient();
            if (isEndCB.Checked)
            {
                Timer timer = new Timer();
                timer.Interval = (1000);
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
            initBtn.Enabled = false;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
			//this.WindowState = FormWindowState.Minimized;

			notifyIcon1.Visible = true;
			notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
			notifyIcon1.BalloonTipText = "Работаю в фоновом режиме.";
			notifyIcon1.BalloonTipTitle = "Discord Rich Presence";
			notifyIcon1.ShowBalloonTip(10);
			Visible = false;
        }

        private void isEndCB_CheckedChanged(object sender, EventArgs e)
        {
            if (isEndCB.Checked)
            {
                endTimeTB.Enabled = true;
            }
            else
            {
                endTimeTB.Enabled = false;
                endTimeTB.Text = "";
            }
        }

        private void restartBtn_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        public void saveCfgBtn_Click(object sender, EventArgs e)
        {
            UserData data = new UserData()
            {
                Details = DetailsTB.Text,
                State = StateTB.Text,
                LargeImageKey = largeImageKeyTB.Text,
                SmallImageKey = smallImageKeyTB.Text,
                LargeImageText = largeImageTextTB.Text,
                SmallImageText = smallImageTextTB.Text,
                ClientID = clientIDTB.Text,
                EndTime = endTimeTB.Text,
                isEnd = isEndCB.Checked
            };
            File.WriteAllText("Config.json", JsonConvert.SerializeObject(data, Formatting.Indented));
        }

        public void loadCfgBtn_Click(object sender, EventArgs e)
        {
			LoadConfig();
        }

        private void endTimeTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void clientIDTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void mainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

		void LoadConfig()
		{
			UserData data = new UserData();
			StreamReader file = File.OpenText("Config.json");
			JsonSerializer serializer = new JsonSerializer();
			data = (UserData)serializer.Deserialize(file, typeof(UserData));
			file.Close();
			DetailsTB.Text = data.Details;
			StateTB.Text = data.State;
			largeImageKeyTB.Text = data.LargeImageKey;
			smallImageKeyTB.Text = data.SmallImageKey;
			largeImageTextTB.Text = data.LargeImageText;
			smallImageTextTB.Text = data.SmallImageText;
			clientIDTB.Text = data.ClientID;
			endTimeTB.Text = data.EndTime;
			isEndCB.Checked = data.isEnd;
		}

		private void button1_Click_2(object sender, EventArgs e)
		{
			Process.Start("https://discord.com/developers/applications");
		}

		private void notifyIcon1_Click(object sender, EventArgs e)
		{
			Visible = true;
		}

		private void launchToolStripMenuItem_Click(object sender, EventArgs e)
		{
			initClient();
			if (isEndCB.Checked)
			{
				Timer timer = new Timer();
				timer.Interval = (1000);
				timer.Tick += new EventHandler(timer_Tick);
				timer.Start();
			}
			initBtn.Enabled = false;
		}

		private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					Visible = true;
					break;
				case MouseButtons.Right:
					contextMenuStrip1.Show();
					break;

			}
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Visible = true;
		}
	}
}
