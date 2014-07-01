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
                    connector.Insert("Zel", "In");
                    
                    WriteInText(DateTime.Now.DayOfWeek.ToString() + " In " + DateTime.Now.ToString() + " " + cmbName.Text);                    
                    
                    MessageBox.Show("Welcome " + cmbName.Text + "!");                     
                    if (_FinalVideoSource.IsRunning)
                        _FinalVideoSource.Stop();
                    Application.Exit();
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
                    connector.Insert("Zel", "Out");

                    WriteInText(DateTime.Now.DayOfWeek.ToString() + " Out " + DateTime.Now.ToString() + " " + cmbName.Text);
                    
                    MessageBox.Show("Goodbye " + cmbName.Text + "!");
                    
                    if (_FinalVideoSource.IsRunning)
                        _FinalVideoSource.Stop();
                    Application.Exit();
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

        private void WriteInText(string x)
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
                        sw.WriteLine(x);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(x);
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
                        sw.WriteLine(x);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(x);
                    }
                }
            }
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
