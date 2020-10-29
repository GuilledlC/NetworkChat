/*SERVER:
-1 Initialize Winsock
-2 Create a socket
-3 Bind the socket
-4 Listen on the socket for a client
-5 Accept a connection from a client
-6 Send and receive data
-7 Disconnect

CLIENT:
-1 Initialize Winsock
-2 Create a socket
-3 Connect to the server
-4 Send and receive data
-5 Disconnect*/


namespace Server
{
	class FOO
	{
		struct Message
		{
			public string message;
			public int type;
			public Socket user;

			public Message(string theMessage, int theType, Socket theUser)
			{
				message = theMessage;
				type = theType;
				user = theUser;
			}
		}

		public static void ExecuteServer()
		{
			Set everything up

			Create a list of sockets which will be the max. ammount of clients

			try
			{

			}
			catch (every error){Show it;}
		}
	}
}
