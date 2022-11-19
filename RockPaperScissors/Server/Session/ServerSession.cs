using Common;
namespace Server;

public class ServerSession : Session
{
    static SessionManager sessionManager = new();
    public static SessionManager Instance { get { return sessionManager; } }
    
    public sealed override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2); // 패킷 아이디 
        
        ServerHandler.Instance.Execute(id, buffer);
    }

    //Todo : 추후 사용 예정
    // public void OnConnected(EndPoint endPoint)
    // {
    //     Console.WriteLine($"OnConnected  : {endPoint}");
    //     
    //     //연결이 되었을 때 client입장! 
    //     //Program.Room.Enter(this)..
    // }
    // public void OnDisconnected(EndPoint endPoint)
    // {
    //     SessionManager.Instance.Remove(this);
    //     
    //     Console.WriteLine($"OnDisconnected : {endPoint}");
    // }
}