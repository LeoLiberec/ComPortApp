using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Timers;
using System.Threading;

namespace ComPortApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GetNameComPorts();
            BaudRateComPort();
            DataBitsComPort();
            StopBitsComPort();
            StartTimer();
        }

        string[] ports = SerialPort.GetPortNames();
        public SerialPort serialPort = new SerialPort();
        public string nameComPort;
        public int baudRateComPort;
        public int dataBitsComPort;
        public StopBits stopBitsComPort;
        public int rate = 1200;
        private System.Timers.Timer aTimer;
        private string getStringASCII;
        private bool testButtonMouse;

        public void GetNameComPorts()
        {
            try
            {
                foreach (string nameCompPortExist in ports)
                {
                    nameComPortCombo.Items.Add(nameCompPortExist);
                    nameComPortCombo.Text = nameCompPortExist;
                } 
            }
            catch
            {
            }
        }

        public void BaudRateComPort()
        {
            for (int i = 0; i < 6; ++i)
            {
                if (i == 0)
                    i = 1;
                rate = rate * 2;
                baudRateComboBox.Items.Add(rate);
            }
            baudRateComboBox.Text = "19200";
        }
        public void DataBitsComPort()
        {
            for (int i = 5; i < 10; i++)
            {
                dataBitsComboBox.Items.Add(i);
            }
            dataBitsComboBox.Text = "8";
        }
        public void StopBitsComPort()
        {
            stopBitsComboBox.Items.Add("None");
            stopBitsComboBox.Items.Add("One");
            stopBitsComboBox.Items.Add("Two");
            stopBitsComboBox.Items.Add("OnePointFive");
            serialPort.StopBits = StopBits.One;
            stopBitsComboBox.Text = "One"; 
        }

        public void GetComPortParam()
        {
            nameComPort = nameComPortCombo.Text;
            baudRateComPort = int.Parse(baudRateComboBox.Text);
            dataBitsComPort = int.Parse(dataBitsComboBox.Text);
            stopBitsComPort = (StopBits)Enum.Parse(typeof(StopBits), stopBitsComboBox.Text);
        }

        public void OpenComPort()
        {
            try
            {
                if (!serialPort.IsOpen)
                {
                    GetComPortParam();
                    serialPort.PortName = nameComPort;
                    serialPort.BaudRate = baudRateComPort;
                    serialPort.DataBits = dataBitsComPort;
                    serialPort.StopBits = stopBitsComPort;
                }
                else
                {
                    pictureBox1.Visible = true;
                    pictureBox1.Image = Properties.Resources.Ok;
                    //Console.WriteLine("Com port: " + nameComPort + " is open \n" +
                    //    serialPort.BaudRate +
                    //    "\n" + serialPort.DataBits +
                    //    "\n" + serialPort.StopBits);
                }
            }
            catch (Exception)
            {
            } 
        }

        private void nameComPortCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nameComPort = nameComPortCombo.Text;
        }

        private void baudRateComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            baudRateComPort = int.Parse(baudRateComboBox.Text);
        }

        private void dataBitsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            dataBitsComPort = int.Parse(dataBitsComboBox.Text);
        }

        private void stopBitsComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            stopBitsComPort = (StopBits)Enum.Parse(typeof(StopBits), stopBitsComboBox.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenComPort();
            try 
            {
                serialPort.Open();
            }
            catch(Exception)
            {
            }
        }

        public void StartTimer()
        {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 100;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            OpenComPort();
            ReadASCII_ComPort();
            WriteASCII_ComPort();
            ReadComPort_r_s();
            TrackBarValue();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                textBox1.Text = null;
                getStringASCII = null;
            }
            pictureBox1.Visible = false;
            trackBar1.Value = 0;
            label5.Text = null;
        }

        public void ReadASCII_ComPort()
        {
            if (serialPort.IsOpen)
            {
                if (checkBox2.CheckState == CheckState.Checked)
                { 
                    try
                    {
                        if (serialPort.ReadExisting().Contains("k"))
                        {
                            textBox1.Invoke((ThreadStart)delegate
                            {
                                getStringASCII = serialPort.ReadTo("k");
                                textBox1.Text = getStringASCII;
                                serialPort.DiscardInBuffer();
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public string trackBarValue;
        public void TrackBarValue()
        {
            trackBarValue = trackBar1.Value.ToString();
        }

        public void WriteASCII_ComPort()
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                try
                {
                    //Invoke((MethodInvoker)delegate
                    //{
                        serialPort.Write(string.Concat(trackBarValue + "k"));
                    //});
                }
                catch(Exception)
                {
                }
            }
            else
            {
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label5.Text = trackBar1.Value.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
            {
                trackBar1.Visible = true;
                label5.Visible = true;
                checkBox3.CheckState = CheckState.Unchecked;
            }
            else
            {
                trackBar1.Visible = false;
                label5.Visible = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.CheckState == CheckState.Checked)
            {
                textBox1.Visible = true;
                checkBox4.CheckState = CheckState.Unchecked;
            }
            else
            {
                textBox1.Visible = false;
            }
        }

        public void ReadComPort_r_s()
        {
            if (serialPort.IsOpen)
            {
                try
                {
                        if (serialPort.ReadExisting().Contains("r"))
                        {
                            textBox2.Invoke((ThreadStart)delegate
                            {
                                textBox2.BackColor = Color.FromArgb(0, 192, 0);
                                textBox2.Text = "Connect";
                            });
                        }
                        if (serialPort.ReadExisting().Contains("s"))
                        {
                            textBox2.Invoke((ThreadStart)delegate
                            {
                                textBox2.BackColor = Color.White;
                                textBox2.Text = null;
                            });
                        }
                }
                catch (Exception)
                {
                }
            }
        }
        private void button3_MouseDown(object sender, MouseEventArgs e)
        {
                testButtonMouse = true;
                Thread thread = new Thread(UpdateMethod);
                thread.Start();
                button3.Invoke((ThreadStart)delegate
                {
                    button3.BackColor = Color.SteelBlue;
                });
        }

        private void button3_MouseUp(object sender, MouseEventArgs e)
        {
            testButtonMouse = false;
            button3.Invoke((ThreadStart)delegate
            {
                button3.BackColor = Color.Transparent;
            });
        }

        private void UpdateMethod()
        {
            if (InvokeRequired)
            {
                while (testButtonMouse == true)
                {
                    if (serialPort.IsOpen)
                    {
                        try
                        {
                            if (checkBox3.CheckState == CheckState.Checked && checkBox4.CheckState == CheckState.Checked)
                            {
                                serialPort.Write("r");
                            }
                        }
                        catch(Exception)
                        {
                        }
                    }
                    else
                    {
                        MessageBox.Show("ComPort is closed", "Info", 
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        testButtonMouse = false;
                    }
                }
                while (testButtonMouse == false)
                {
                    if (serialPort.IsOpen)
                    {
                        try
                        {
                            if (checkBox3.CheckState == CheckState.Checked && checkBox4.CheckState == CheckState.Checked)
                            {
                                serialPort.Write("s");
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.CheckState == CheckState.Checked)
            {
                button3.Visible = true;
                checkBox2.CheckState = CheckState.Unchecked;
            }
            else
            {
                button3.Visible = false;
            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.CheckState == CheckState.Checked)
            {
                textBox2.Visible = true;
                checkBox1.CheckState = CheckState.Unchecked;
            }
            else
            {
                textBox2.Visible = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }
}
