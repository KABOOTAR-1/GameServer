
Console.Title = "Game Server";

GameServer.GameLogic.IsRunning = true;

Thread mainThread= new Thread(new ThreadStart(GameServer.GameLogic.MainThread));
mainThread.Start(); 
GameServer.Server.Start(4, 26950);



