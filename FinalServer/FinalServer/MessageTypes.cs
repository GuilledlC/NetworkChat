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
	// 1: Joined the server
	// 0: Left the server <- default message if none is specified, user is kicked lol
	// 2: Any message
}
