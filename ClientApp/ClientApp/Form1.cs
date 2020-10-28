using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class Form1 : Form
    {
        string name = "Guille"; //Guille
        string address = "fe80::b1bb:d56c:b2d9:a5af%15"; //fe80::b1bb:d56c:b2d9:a5af%15
        int port = 11111; //11111

        public static void SendMessage(Socket sender, string message)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(message + "\b");
            int byteSent = sender.Send(messageSent);
        }

        private void Show(string text)
        {
            OutputBox.AppendText(text);
        }

        private void Send(string message)
        {
            Font bold = new Font(OutputBox.Font, FontStyle.Bold);       //Self explanatory, the
            Font regular = new Font(OutputBox.Font, FontStyle.Regular); //bold and regular fonts

            string user = name + ": ";
            int indent = TextRenderer.MeasureText(user, bold).Width - 6; //Indentation value

            if (MsgTimer.Enabled) //If it's within the time it can be intended
            {
                OutputBox.SelectionIndent = indent; //Indentation
                OutputBox.SelectionFont = regular;
                OutputBox.AppendText(message + "\n");
            }
            else
            {
                OutputBox.SelectionIndent = 0; //No indentation
                OutputBox.SelectionFont = bold;
                OutputBox.AppendText(user);
                OutputBox.SelectionFont = regular;
                OutputBox.AppendText(message + "\n");
            }
            MessageBox.Text = ""; //Resets the message box
            MsgTimer.Stop();
            MsgTimer.Start(); //Starts the 5 second timer for indentation
        }

        private bool HasText(string text)
        {
            foreach (char c in text)
            {
                if (c > 32)
                {
                    return true;
                }
            }
            return false;
        }

        public Form1()
        {
            InitializeComponent();
            Show("Initialized\n");
            //ExecuteClient(address, port);
        }

        private void MsgTimer_Tick(object sender, EventArgs e)
        {
            MsgTimer.Stop();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (HasText(MessageBox.Text))
                Send(MessageBox.Text);
            ExecuteClient(address, port);
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && HasText(MessageBox.Text))
            {
                Send(MessageBox.Text);
                e.SuppressKeyPress = true; //Stops the DING sound when pressing Enter
            }
        }

        public void ExecuteClient(string addr, int prt)
        {
            try
            {
                string address = addr;
                int port = prt;

                //Establish the remote endpoint for the socket.
                IPHostEntry iPHost = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddr = iPHost.AddressList[0]; //This is for local IP Address
                IPAddress ipAddr = IPAddress.Parse(address);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);
                //Creation of the TCP/IP socket using Socket Class Constructor
                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(localEndPoint); //Connect Socket to the remote endpoint using Connect() method
                    SendMessage(sender, "RECEIVED");

                    //Console.WriteLine("Socket connected to -> {0}", sender.RemoteEndPoint.ToString()); //We print the EndPoing info
                    Show("Connected to " + addr + "!\n");

                    while (true)
                    {

                        //Thread ReceiveThread = new Thread(() => SendMessage(sender));
                        //ReceiveThread.Start();

                        //ReceiveMessage(sender);
                    }
                    //Close Socket using the Close() method
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                //Manage the Socket's exceptions
                catch (ArgumentNullException ane)
                {
                    Show("ArgumentNullException: " + ane.ToString());
                }
                catch (SocketException se)
                {
                    Show("SocketException: " + se.ToString());
                }
                catch (Exception e)
                {
                    Show("Unexpected exception: " + e.ToString());
                }
            }
            catch (Exception e)
            {
                Show(e.ToString());
            }
            //Console.ReadLine();
        }
    }
}
