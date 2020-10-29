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
	class FOO
	{
		//Types of message:
		// 1: Joined the server
		// 0: Left the server <- default message if none is specified, user is kicked lol
		// 2: Any message
		struct Message
		{
			public string message;
			private int type;
			public Socket user;

			public Message(string theMessage, int theType, Socket theUser)
			{
				message = theMessage;
				Type = theType;
				user = theUser;
			}

			public int Type
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

		public static void ExecuteServer()
		{
			//Set everything up;

			//Create a list of sockets which will be the max. ammount of clients
			List<Socket> clients = new List<Socket>();

			try
			{
				//Wait for a connection
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
			catch (Exception error){Show error}
		}

		public static void Connection(Socket client)
		{
			Message current = Listen(client);
		}

		public static Message Listen(Socket client)
		{
			byte[] bytes = new byte[2048];
			string data = null;
			string user = null;
			string temp = null;
			int type = null;

			while(true) //Parsing the message
			{
				int numByte = client.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, numByte);

                if (data.IndexOf("\b") > -1) //<EOF>
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
                data += Encoding.ASCII.GetString(bytes, 0, numByte); FIX THIS LATER

                if (data.IndexOf("\b") > -1) //<EOF>
                    break;
			}

		}

		static void Main(string[] args)
		{
			ExecuteServer();
		}
	}
}		
