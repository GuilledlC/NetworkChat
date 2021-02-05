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
				if (value == MessageTypes.Disconnected || value == MessageTypes.Joined || value == MessageTypes.Any
				|| value == MessageTypes.NameChange || value == MessageTypes.Test)
					_type = value;
				else
					_type = (MessageTypes)(-1);
			}
		}
	}
}
