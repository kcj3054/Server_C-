using System.Net;
using System.Net.Sockets;
using Common;
namespace Client;

public class ClientSession : Session
{
    static ClientSession clientSession = new();
    public static ClientSession Instance { get { return clientSession; } }
    
    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
        ClientHandler.Instance.Execute(id, buffer);
    }
    
    public Socket? OnConnect(EndPoint endPoint)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            _socket.Connect(endPoint);
            return _socket;
        }
        catch (Exception e)
        {
            Console.WriteLine("아직 서버가 일어나지 않았습니다 한번 더 시도하겠습니다");
        }
        return null;
    }

    public bool OnConnected()
    {
        if (_socket.Connected)
        {
            return true;
        }
        return false;
    }
}