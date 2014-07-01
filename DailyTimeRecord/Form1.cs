using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;
using AForge.Video;
using AForge.Video.DirectShow;

namespace DailyTimeRecord
{
    public partial class Form1 : Form
    {
        
        private FilterInfoCollection _VideoCaptureDevices; //this stores all the video devices available
        private VideoCaptureDevice _FinalVideoSource; //this will store video device to be used ie usb cam
        private DBConnector connector = new DBConnector();

        public Form1()
        {
            InitializeComponent();            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString();
        }

        private void btnIn_Click(object sender, EventArgs e)
        {            
            try
            {
                if (cmbName.Text != "")
                {
                    WriteInText(DateTime.Now.DayOfWeek.ToString() + " In " + DateTime.Now.ToString() + " " + cmbName.Text);
                    WriteFileForToday("In," + cmbName.Text + "," + DateTime.Now.ToString("HH:mm:ss") + "," + DateTime.Now.ToString("MM/dd/yyyy"));
                    InsertToDB("In", cmbName.Text);
                }
                else
                {
                    MessageBox.Show("Please select your name.");
                }
            }
            catch
            {
                MessageBox.Show("Login failed. Please try again.");
            }
        }

        private void btnOut_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbName.Text != "")
                {
                    WriteInText(DateTime.Now.DayOfWeek.ToString() + " Out " + DateTime.Now.ToString() + " " + cmbName.Text);
                    WriteFileForToday("Out," + cmbName.Text + "," + DateTime.Now.ToString("HH:mm:ss") + "," + DateTime.Now.ToString("MM/dd/yyyy"));
                    InsertToDB("Out", cmbName.Text);
                }
                else
                {
                    MessageBox.Show("Please select your name.");
                }
            }
            catch
            {
                MessageBox.Show("Logout failed. Please try again.");
            }
        }

        private void WriteInText(string line)
        {
            try
            {
                if (!Directory.Exists(connector.GetDirectory()))
                {
                    Directory.CreateDirectory(connector.GetDirectory());
                }
                string path = connector.GetDirectory() + "GarapataDTR.txt";
                string pic = connector.GetDirectory() + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Year.ToString() + " " + DateTime.Now.TimeOfDay.Hours.ToString() + "." + DateTime.Now.TimeOfDay.Minutes.ToString() + " " + cmbName.Text + ".jpeg";
                
                pictureBox1.Image.Save(pic, System.Drawing.Imaging.ImageFormat.Jpeg);
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(line);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(line);
                    }
                }
            }
            catch
            {
                if (!Directory.Exists(connector.GetDirectory()))
                {
                    Directory.CreateDirectory(connector.GetDirectory());
                }
                string path = connector.GetDirectory() + "GarapataDTR.txt";
                string pic = connector.GetDirectory() + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Year.ToString() + " " + DateTime.Now.TimeOfDay.Hours.ToString() + "." + DateTime.Now.TimeOfDay.Minutes.ToString() + " " + cmbName.Text + ".jpeg";

                pictureBox1.Image.Save(pic, System.Drawing.Imaging.ImageFormat.Jpeg);                
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(line);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(line);
                    }
                }
            }
        }

        private void WriteFileForToday(string line)
        {
        }

        private void InsertToDB(string logType, string name)
        {
            connector.Insert(name, logType);

            if(logType.Equals("In"))
                MessageBox.Show("Welcome " + name + "!");
            else if(logType.Equals("Out"))
                MessageBox.Show("Goodbye " + name + "!");

            if (_FinalVideoSource.IsRunning)
                _FinalVideoSource.Stop();
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (int.Parse(DateTime.Now.TimeOfDay.Hours.ToString()) > 17)
                btnIn.Enabled = false;

            _VideoCaptureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo _VideoCam in _VideoCaptureDevices)
            {
                comboBox1.Items.Add(_VideoCam.Name);
            }
            comboBox1.SelectedIndex = 0;
            
            _FinalVideoSource = new VideoCaptureDevice(_VideoCaptureDevices[comboBox1.SelectedIndex].MonikerString);
            _FinalVideoSource.NewFrame += new NewFrameEventHandler(_FinalVideoSource_NewFrame);
            _FinalVideoSource.Start();
        }

        void _FinalVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_FinalVideoSource.IsRunning)
                _FinalVideoSource.Stop();
        }

        private void btnComment_Click(object sender, EventArgs e)
        {
            if (txtComment.Visible == false) txtComment.Visible = true;
            else txtComment.Visible = false;
        }
    }
}
