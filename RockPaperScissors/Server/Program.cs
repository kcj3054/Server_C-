using System.Net;
using System.Net.Sockets;
using Common;
using Server;

Listener listener = new();
IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 123);

listener.Init(endPoint, OnAcceptHandler);

ServerHandler.Instance.RegisterPacketHandler();

var line = Console.ReadLine();

void OnAcceptHandler(Socket clientSocket)
{
    if (clientSocket.Connected)
    {
       SessionManager.Instance.Generate(clientSocket);  // 접속한 session을 sessionManager의 Dic에 넣어놓기 ! 
    }
    
}