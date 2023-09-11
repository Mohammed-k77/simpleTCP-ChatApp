using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace chat_APP
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;
        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            
            foreach(IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    txtServerIP.Text = address.ToString();
                }
            }

        }



        private void btnStart_Click(object sender, EventArgs e)
        {
            

            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(txtServerPort.Text));
            listener.Start();
            client = listener.AcceptTcpClient();
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
            STR = new StreamReader(client.GetStream());
            STW = new StreamWriter(client.GetStream());
            STW.AutoFlush = true;
 
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IpEnd = new IPEndPoint(IPAddress.Parse(txClientIP.Text), int.Parse(txtClientPort.Text));
            client.Connect(IpEnd);

            try
            {
                
                txtChatScreen.AppendText("Connect to server" + "\r\n");
                STW = new StreamWriter(client.GetStream());
                STR = new StreamReader(client.GetStream());                
                STW.AutoFlush = true;
                backgroundWorker3.DoWork += BackgroundWorker3_DoWork;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BackgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.txtChatScreen.Invoke(new MethodInvoker(delegate ()
                        {
                            txtChatScreen.AppendText("You: " + recieve + "\r\n");
                        }));
                    recieve = "";
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                this.txtChatScreen.Invoke(new MethodInvoker(delegate ()     
                {
                    txtChatScreen.AppendText("Me: " + TextToSend + "\r\n");
                }));

            }
            else
            {
                MessageBox.Show("Sending Failed");
            }

            //backgroundWorker2.CancelAsync();                                   
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtMessage.Text != "")
            {
                TextToSend = txtMessage.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            txtMessage.Text = "";
        }

       
    }
}
