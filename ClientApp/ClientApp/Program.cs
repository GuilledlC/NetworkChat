/*Copyright (c) 2020, Guillermo de la Cal All rights reserved. Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
	1 - Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
	2 - Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
		in the documentation and/or other materials provided with the distribution. THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS
		AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
		MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
		LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
		PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
		THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
		OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/

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
        string address = "fe80::b1bb:d56c:b2d9:a5af%16"; //fe80::b1bb:d56c:b2d9:a5af%16
        int port = 11111; //11111

        private delegate void SafeCallDelegate(string text);
        private void WriteTextSafe(string text)
        {
            if (OutputBox.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteTextSafe);
                OutputBox.Invoke(d, new object[] { text });
            }
            else
            {
                OutputBox.AppendText(text);
            }
        }

        private void Print(string text)
        {
            //OutputBox.AppendText(text);
            WriteTextSafe(text);
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

        public static void SendMessage(Socket sender, string message)
        {
            byte[] messageSent = Encoding.ASCII.GetBytes(message + "\b");
            int byteSent = sender.Send(messageSent);
        }

        private void ReceiveMessage(Socket sender)
        {
            byte[] buffer = new byte[2048];
            int numByte = sender.Receive(buffer);
            Print(Encoding.ASCII.GetString(buffer, 0, numByte));
        }

        private void PrintMessage(string user, string message)
        {
            Font bold = new Font(OutputBox.Font, FontStyle.Bold);       //Self explanatory, the
            Font regular = new Font(OutputBox.Font, FontStyle.Regular); //bold and regular fonts

            user = user + ": ";
            int indent = TextRenderer.MeasureText(user, bold).Width - 6; //Indentation value

            if (MsgTimer.Enabled) //If it's within the time it can be intended
            {
                OutputBox.SelectionIndent = indent; //Indentation
                OutputBox.SelectionFont = regular;
                Print(message + "\n");
            }
            else
            {
                OutputBox.SelectionIndent = 0; //No indentation
                OutputBox.SelectionFont = bold;
                Print(user);
                OutputBox.SelectionFont = regular;
                Print(message + "\n");
            }
            MsgTimer.Stop();
            MsgTimer.Start(); //Starts the 5 second timer for indentation
        }

        public void ExecuteClient(string addr, int prt)
        {
            try
            {
                string address = addr;
                int port = prt;

                Print("Trying to connect to " + addr + ":" + port);

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
                    SendMessage(sender, "RECEIVED\bCLIENT\b2");

                    //Console.WriteLine("Socket connected to -> {0}", sender.RemoteEndPoint.ToString()); //We print the EndPoing info
                    Print("\nConnected to " + addr + "!\n");

                    Thread ReceiveThread = new Thread(() => ReceiveMessage  (sender));
                    ReceiveThread.Start();

                    while(false)
                    {
                        //Close Socket using the Close() method
                        sender.Shutdown(SocketShutdown.Both);
                        sender.Close();
                    }
                }
                //Manage the Socket's exceptions
                catch (ArgumentNullException ane)
                {
                    Print("ArgumentNullException: " + ane.ToString());
                }
                catch (SocketException se)
                {
                    Print("SocketException: " + se.ToString());
                }
                catch (Exception e)
                {
                    Print("Unexpected exception: " + e.ToString());
                }
            }
            catch (Exception e)
            {
                Print(e.ToString());
            }
            //Console.ReadLine();
        }

        public Form1()
        {
            InitializeComponent();
            Print("Initialized\n");
            //ExecuteClient(address, port);
        }

        private void MsgTimer_Tick(object sender, EventArgs e)
        {
            MsgTimer.Stop();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (HasText(MessageBox.Text))
                //SendMessage(MessageBox.Text);
                MessageBox.Text = ""; //Resets the message box
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && HasText(MessageBox.Text))
            {
                //SendMessage(MessageBox.Text);
                MessageBox.Text = ""; //Resets the message box
                e.SuppressKeyPress = true; //Stops the DING sound when pressing Enter
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            ExecuteClient(address, port);
        }
    }
}
