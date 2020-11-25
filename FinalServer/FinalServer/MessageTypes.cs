namespace FinalServer
{
	enum MessageTypes
	{
		Error = -1,
		Disconnected = 0,
		Joined = 1,
		Any = 2,
		NameChange = 3,
	}

	//Types of message:
	//-1: Error  <- default message if none is specified
	// 1: Joined the server
	// 0: Left the server
	// 2: Any message
	// 3: Name changed
}
