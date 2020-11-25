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
	class Message
	{
		public string text;
		private MessageTypes _type;
		public string user;

		public Message(string constructor)
		{
			int count = 0, theType;
			string theMessage = null, theUser = null, typestr = null;
			foreach (char c in constructor)
			{
				if (c == '\b')
					count++;
				else if (count == 0)
					theMessage += c;
				else if (count == 1)
					theUser += c;
				else if (count == 2)
					typestr += c;
			}

			try
			{
				theType = Convert.ToInt32(typestr);
			}
			catch //If it's not an integer
			{
				theType = -1; //Convert to error
			}
			
			text = theMessage;
			user = theUser;
			MsgType = (MessageTypes)theType;
			Console.WriteLine(theUser + ": " + theMessage + "/" + MsgType);
		}

		public MessageTypes MsgType
		{
			get { return _type; }
			set
			{
				if (value == MessageTypes.Disconnected || value == MessageTypes.Joined || value == MessageTypes.Any)
					_type = value;
				else
					_type = (MessageTypes)(-1);
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

					string texto = Console.ReadLine();
					Send(texto, "Guille");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void Join(Socket sock, string user) //Adds user to the list and announces the join
        {
			users.Add(user);
			Send(user.ToString() + " has just joined!", "SERVER");
		}

		public static void Connection(Socket sock)
		{
			socks.Add(sock);
			while (true)
			{
				Message current = Listen(sock);
				if (current.MsgType == MessageTypes.Disconnected)
					Disconnect(sock, current.user);
				else if (current.MsgType == MessageTypes.Joined)
					Join(sock, current.user);
				else if (current.MsgType == MessageTypes.Any)
					Send(current.text, current.user.ToString());
				//else if (current.MsgType == MessageTypes.NameChange)

			}
		}

		public static void Disconnect(Socket sock, string user) //Announces the disconnection and closes the socket
		{
			Send(user.ToString() + " has just left!", "SERVER");
			sock.Shutdown(SocketShutdown.Both);
			sock.Close();
		}

		public static void Send(string msg, string user) //Sends a message to all sockets
		{
			foreach (Socket sock in socks)
			{
				byte[] messageSent = Encoding.ASCII.GetBytes(user + ": " + msg);
				sock.Send(messageSent);
			}
		}

		public static Message Listen(Socket sock)
		{
			byte[] buffer = new byte[2048];
			int numByte = sock.Receive(buffer);
			string temp = Encoding.ASCII.GetString(buffer, 0, numByte);

			Message message = new Message(temp);
			return message;
		}

		static void Main(string[] args)
		{
			ExecuteServer();
			Console.ReadLine();
		}
	}
}
