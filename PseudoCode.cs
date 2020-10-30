//A bunch of pseudocode that I'm slowly turning into real code
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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FinalServer
{
	enum MessageTypes
	{
		Disconnected = 0,
		Joined = 1,
		Any = 2,
	}

	//Types of message:
	// 1: Joined the server
	// 0: Left the server <- default message if none is specified, user is kicked lol
	// 2: Any message
	class Message
	{
		public string text;
		private MessageTypes _type;
        public string user;
		public Socket sock;

		public Message(string theMessage, MessageTypes theType, string theUser, Socket theSock)
		{
			text = theMessage;
			MsgType = theType;
            user = theUser;
			sock = theSock;
		}

		public MessageTypes MsgType
		{
			get { return _type; }
			set
			{
				if (value == MessageTypes.Disconnected || value == MessageTypes.Joined || value == MessageTypes.Any)
					_type = value;
				else
					_type = 0;
			}
		}
	}

	class FOO
	{
		//Create a list of sockets
		static List<Socket> socks = new List<Socket>();
        static List<string> users = new List<string>();

		public static void ExecuteServer()
		{
			//Set everything up;
			IPHostEntry iPHost = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddr = iPHost.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
			Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				//Wait for a connection
				listener.Bind(localEndPoint);
				listener.Listen(10);
				while (true)
				{
					Socket clientSocket = listener.Accept();

					Thread ConnectionThread = new Thread(() => Connection(clientSocket));
					ConnectionThread.Start();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void Connection(Socket sock)
		{
			while (true)
			{
				Message current = Listen(sock);
				if (current.MsgType == MessageTypes.Disconnected)
					Disconnect(sock);
				else if (current.MsgType == MessageTypes.Joined)
					Send(current.user.ToString() + " has just joined!", "Server");
				else if (current.MsgType == MessageTypes.Any)
					Send(current.text, current.user.ToString());
			}
		}

		public static void Disconnect(Socket sock)
		{
			sock.Shutdown(SocketShutdown.Both);
			sock.Close();
		}

		public static void Send(string msg, string user)
		{
			foreach (Socket sock in socks)
			{
				byte[] messageSent = Encoding.ASCII.GetBytes(user + ": " + msg);
				sock.Send(messageSent);
			}
		}

		public static Message Listen(Socket sock)
		{
			byte[] bytes = new byte[2048];
			string text = null;
            string user = null;
			Socket user = null;
			string temp = null;
			int type;

			while (true) //Parsing the message
			{
				int numByte = sock.Receive(bytes);
				text += Encoding.ASCII.GetString(bytes, 0, numByte);

				if (text.IndexOf("\b") > -1) //<EOF>
					break;
			}
			while (true) //Parsing the user
			{
				int numByte = sock.Receive(bytes);
				user += Encoding.ASCII.GetString(bytes, 0, numByte);

				if (user.IndexOf("\b") > -1) //<EOF>
					break;
			}
			while (true) //Parsing the type
			{
				int numByte = sock.Receive(bytes);
				temp += Encoding.ASCII.GetString(bytes, 0, numByte);

				if (temp.IndexOf("\b") > -1) //<EOF>
					break;
			}
			temp.Trim('\b');
			type = Convert.ToInt32(temp);

			Message message = new Message(text, (MessageTypes)type, user, sock);
			return message;
		}

		static void Main(string[] args)
		{
			ExecuteServer();
		}
	}
}
