using System;
using System.Timers;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;


namespace NetMonitor
{
    public partial class Form1 : Form
    {        
        private NetworkInterface[] nicArr;      //网卡集合
        private System.Timers.Timer timer;      //计时器

        public Form1()
        {
            InitializeComponent();

            InitNetworkInterface();
            InitializeTimer();
        }

        /// <summary>
        /// 初始化网卡
        /// </summary>
        public void InitNetworkInterface()
        {
            nicArr = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < nicArr.Length; i++)
                toolStripComboBox1.Items.Add(nicArr[i].Name);
            toolStripComboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化计时器
        /// </summary>
        private void InitializeTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }
               
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                UpdateNetworkInterface();
            });
        }

        public void UpdateNetworkInterface()
        {
            NetworkInterface nic = nicArr[toolStripComboBox1.SelectedIndex];
            IPv4InterfaceStatistics interfaceStats = nic.GetIPv4Statistics();

            int bytesSent = (int)(interfaceStats.BytesSent - double.Parse(label4.Text));
            int bytesRecv = (int)(interfaceStats.BytesReceived - double.Parse(label3.Text));
            
            label3.Text = interfaceStats.BytesReceived.ToString();
            label4.Text = interfaceStats.BytesSent.ToString();

            

            string netRecvText = "";
            string netSendText = "";

            
            if (bytesRecv < 1024 * 1024)
            {
                netRecvText = ((double)bytesRecv / 1024).ToString("0.00") + "K/s";
            }
            else if (bytesRecv >= 1024 * 1024)
            {
                netRecvText = ((double)bytesRecv / (1024 * 1024)).ToString("0.00") + "M/s";
            }

            
            if (bytesSent < 1024 * 1024)
            {
                netSendText = ((double)bytesSent / 1024).ToString("0.00") + "K/s";
            }
            else if (bytesSent >= 1024 * 1024)
            {
                netSendText = ((double)bytesSent / (1024 * 1024)).ToString("0.00") + "M/s";
            }

            //更新控件

            label1.Text = netSendText;
            label2.Text = netRecvText;
        }


        /// <summary>
        /// 嵌入状态栏
        /// </summary>
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(String lpClassName, String lpWindowName);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, String lpsz1, String lpsz2);
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern IntPtr MoveWindow(IntPtr hWnd, IntPtr x, IntPtr y, int nWidth, int nHeight, int bRepaint);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowRect(IntPtr HWnd, out Rect lpRect);


        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        private IntPtr hMin;
        private Rect rcMin;
        
        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr hTaskbar = FindWindow("Shell_TrayWnd", null);
            IntPtr hBar = FindWindowEx(hTaskbar, (IntPtr)0, "ReBarWindow32", null);
            hMin = FindWindowEx(hBar, (IntPtr)0, "MSTaskSwWClass", null);

            Rect rcBar = new Rect();
            rcMin = new Rect();
            GetWindowRect(hMin, out rcMin);
            GetWindowRect(hBar, out rcBar);

            MoveWindow(hMin, (IntPtr)0, (IntPtr)0, rcMin.Right - rcMin.Left - this.Width, rcMin.Bottom - rcMin.Top, 1);

            SetParent(this.Handle, hBar);
            this.Height = 40;
            this.Width = this.Width;
            MoveWindow(this.Handle, (IntPtr)(rcMin.Right - rcMin.Left - this.Width + 2), (IntPtr)((rcBar.Bottom - rcBar.Top - this.Height) / 2), this.Width, this.Height, 1);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }    
    }
}