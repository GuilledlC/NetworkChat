using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteServer();
            Console.Read();
        }

        public static void ReceiveMessage(Socket clientSocket)
        {
            //Data buffer
            byte[] bytes = new byte[1024];
            string data = null;

            while (true)
            {
                int numByte = clientSocket.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numByte);

                if (data.IndexOf("\b") > -1) //<EOF>
                    break;
            }

            Console.WriteLine("CLIENT: " + data);
        }

        public static void SendMessage(Socket clientSocket)
        {
            Console.Write("SERVER: ");
            string message = Console.ReadLine();
            byte[] messageSent = Encoding.ASCII.GetBytes(message + "\b");
            clientSocket.Send(messageSent);
        }

        public static void ExecuteServer()
        {
            //Establish the remote endpoint for the socket. Dns.GetHostName returns the name of the host running the application
            IPHostEntry iPHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = iPHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            //Creation of the TCP/IP socket using Socket Class Constructor
            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Using Bind() method we associate a network adress to the Server Socket. All clients that connect to this Server Socket must know this network adress
                listener.Bind(localEndPoint);

                //Using Listen() method we create the list of Clients that want to connect to the server
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for connection...");

                    //Suspend while waiting for incoming connection. Using the Accept() method the server will accept the connection of the client
                    Socket clientSocket = listener.Accept();
                    Console.WriteLine("Connected!");

                    while (true)
                    {

                        Thread ReceiveThread = new Thread(() => ReceiveMessage(clientSocket));
                        ReceiveThread.Start();

                        SendMessage(clientSocket);
                    }
                    //Close client Socket using the Close() method. After
                    //closing we can used the closed Socket for a new
                    //Client connection
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
