using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Common;

namespace Server;

//Room안에 존재하는 user들은 playerId로 관리하면된다 
public class RoomManager 
{
    private static RoomManager _roomManager= new();
    public static RoomManager Instance { get { return _roomManager; } }
    public List<GameRoom> gameRooms { get; set; } = new(); //room넣기 
}

public class GameRoom
{
    public ConcurrentDictionary<ushort, ushort> playerAttackValue { get; set; } = new();
    public ushort RoomId { get; set; }
    public List<Int32> Room { get; set; } = new();
    public void Enter(Int32 sessionId)
    {
        Room.Add(sessionId);
    }
    public void Leave(Int32 sessionId)
    {
        Room.Remove(sessionId);
    }
}