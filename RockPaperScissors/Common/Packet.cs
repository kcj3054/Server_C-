using MessagePack;

namespace Common;

public class Packet
{ 
    public ushort size { get; set; }
    public ushort id { get; set; }
}
public class PlayerIdReq : Packet
{
    public ushort value { get; set; }
}
public class PlayerEnterReq  : Packet
{
    public ushort playerId { get; set; }
}
public class PlayerExitReq  : Packet
{
    public ushort playerId { get; set; }
}

public class PlayerExitRes  : Packet
{
    public bool isExit { get; set; }
}

public class PlayerIdRes : Packet
{ 
    public Boolean isOk { get; set; }
}
public class PlayerEnterRes  : Packet
{
    public ushort playerId { get; set; }
    public ushort roomNumber { get; set; }
}
public class PlayerAttackRes : Packet
{
    public ushort playerId { get; set; }
    public ushort value { get; set; }  // 가위(1), 바위(2), 보(3) 중하나...
    
    public ushort RoomNumber { get; set; }
}

public class PlayerAttackReq : Packet
{
    public ushort RoomNumber { get; set; }
    public ushort PlayerId { get; set; }
}

public class GameResultRes : Packet
{
    public Boolean isVictory { get; set; }
    public ushort RoomNumber { get; set; }
}

public class GameRestart : Packet
{
    
}