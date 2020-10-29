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

namespace Server
{
	//Types of message:
	// 1: Joined the server
	// 0: Left the server <- default message if none is specified, user is kicked lol
	// 2: Any message
	class Message
	{
		public string text;
		private int type;
		public Socket user;

		public Message(string theMessage, int theType, Socket theUser)
		{
			message = theMessage;
			Msgtype = theType;
			user = theUser;
		}

		public int Msgtype
		{
			get { return type; }
			set{
				if(value == 0 || value == 1 || value == 2)
					type = value;
				else
					type = 0;
			}
		}
	}

	class FOO
	{
		public static void ExecuteServer()
		{
			//Set everything up;
			IPHostEntry iPHost = Dns.GetHostEntry(Dns.GetHostName());
			IPAddress ipAddr = iPHost.AddressList[0];
			IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
			Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			//Create a list of sockets
			List<Socket> clients = new List<Socket>();

			try
			{
				//Wait for a connection
				listener.Bind(localEndPoint);
				listener.Listen(10);
				while(true)
				{
					Socket clientSocket = listener.Accept();
					//Everyhting that went here I am going to put inside the Connection() method
					//and then create a new thread with that method for each client
					//Is that what you meant?
					Thread ConnectionThread = new Thread(() => Connection(clientSocket));
					ConnectionThread.Start();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void Connection(Socket client)
		{
			while(true)
			{
				Message current = Listen(client);
				if(message.Msgtype == 0)
					Disconnect(client);
				else if(message.Msgtype == 1)
					Send(message.user" has just joined!", Server); //NEED TO MAKE THE SERVER A USER
				else if(message.Msgtype == 2)
					Send(message.text, message.user);
			}
		}

		public static void Disconnect(Socket client)
		{
			client.Shutdown(SocketShutdown.Both);
			client.Close();
		}

		public static Message Listen(Socket client)
		{
			byte[] bytes = new byte[2048];
			string text = null;
			string user = null;
			string temp = null;
			int type = null;

			while(true) //Parsing the message
			{
				int numByte = client.Receive(bytes);
				text += Encoding.ASCII.GetString(bytes, 0, numByte);

				if (text.IndexOf("\b") > -1) //<EOF>
					break;
			}
			while(true) //Parsing the user
			{
				int numByte = client.Receive(bytes);
				user += Encoding.ASCII.GetString(bytes, 0, numByte);

				if (user.IndexOf("\b") > -1) //<EOF>
					break;
			}
			while(true) //Parsing the type
			{
				int numByte = client.Receive(bytes);
				temp += Encoding.ASCII.GetString(bytes, 0, numByte);

				if (temp.IndexOf("\b") > -1) //<EOF>
				break;
			}
			temp.Trim("\b");
			type = Convert.ToInt32(temp);

			Message message = new Message(text, user, type);
			return message;
		}

		static void Main(string[] args)
		{
			ExecuteServer();
		}
	}
}
