using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteClient();
        }

        public static void SendMessage(Socket sender)
        {
            Console.Write("CLIENT: ");
            string message = Console.ReadLine();
            byte[] messageSent = Encoding.ASCII.GetBytes(message + "\b");
            int byteSent = sender.Send(messageSent);
        }

        public static void ReceiveMessage(Socket sender)
        {
            //Data buffer
            byte[] messageReceived = new byte[1024];

            //We receive the message using the method Receive(). This method returns the number of bytes received, that we'll use to convert them to string
            int byteRecv = sender.Receive(messageReceived);
            Console.WriteLine("SERVER: " + Encoding.ASCII.GetString(messageReceived, 0, byteRecv));
        }

        public static void ExecuteClient()
        {
            try
            {
                string address = "fe80::b1bb:d56c:b2d9:a5af%15"; //fe80::711c:5c55:292f:c656%21
                int port = 11111; //11111
                /*Console.Write("Please write the IPv6 address:");
                address = Console.ReadLine();
                Console.Write("Please write the port number:");
                port = Convert.ToInt32(Console.ReadLine());*/

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

                    //Console.WriteLine("Socket connected to -> {0}", sender.RemoteEndPoint.ToString()); //We print the EndPoing info
                    Console.WriteLine("Connected!");

                    while (true)
                    {
                        
                        Thread ReceiveThread = new Thread(() => SendMessage(sender));
                        ReceiveThread.Start();

                        ReceiveMessage(sender);
                    }
                    //Close Socket using the Close() method
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                //Manage the Socket's exceptions
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException: {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException: {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception: {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadLine();
        }
    }
}
