using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Common;
using ServerCommon;
namespace Client;

//수정 예정 session의 것들 사용하지않는다  클라이언트는 동기로 만들자 
public class ClientHandler
{
    public static string playerId;
    public static ClientHandler clientHandler = new();
    public static ClientHandler Instance { get { return clientHandler; } }
    private Dictionary<Int32, Action<Int32, ArraySegment<byte>>> _packetHandlers = new();
    public static Boolean playerIdOk = default;
    public static Int32 count = default;
    
    public void RegisterPacketHandler()
    {
        _packetHandlers.Add((Int32)PacketID.PlayerEnterRes, PlayerEnterResponse);
        _packetHandlers.Add((Int32)PacketID.PlayerAttackReq, PlayerAttackReq);
        _packetHandlers.Add((Int32)PacketID.PlayerExitRes, PlayerExitRes); // 서버로부터 퇴장에대한 응답 패킷 
        _packetHandlers.Add((Int32)PacketID.VerifyPlayerId, VerifyPlayerId);
        _packetHandlers.Add((Int32)PacketID.GameReStart, GameReStart); // 게임 결과가 무승부일 경우 재시작 
        _packetHandlers.Add((Int32)PacketID.GameResult, GameResult);
    }

    public void Execute(Int32 headerInfo, ArraySegment<byte> bytes)
    {
        Int32 id = headerInfo;
        if (_packetHandlers.ContainsKey(id))
        {
            try
            {
                _packetHandlers[id](headerInfo, bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    private void GameReStart(Int32 number, ArraySegment<byte> buffer)
    {
        Console.WriteLine("결과가 무승부라서 재시작하라네요 이번판은 이겨보자");
    }
    
    private void GameResult(Int32 number, ArraySegment<byte> buffer)
    {
        ushort roomId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4);
        Boolean isOk = BitConverter.ToBoolean(buffer.Array, buffer.Offset + 6);

        if (isOk)
        {
            Console.WriteLine("내가 승리했다.");
        }
        else
        {
            Console.WriteLine("내가 졌네");
        }

        Console.WriteLine("게임 한판 더 하실건가요? yes or no로 대답하세요 아니면 게임을 종료합니다");
        
        //logic처리 
        string cmd = Console.ReadLine();
        count++;
        if (cmd == "yes")
        {
            // Attack(roomId);
            StartGame startGame = new();
            startGame.Start();
        }
        else if (cmd == "no")
        {
            ushort parsePlayerId = ushort.Parse(playerId);
            PlayerExitRoom(parsePlayerId);
        }
    }
    private void PlayerExitRes(Int32 number, ArraySegment<byte> buffer)
    {
        //다른 에러나, 예외처리 생략 
        Console.WriteLine("성공적으로 퇴장하셨습니다. 잘가세요");
    }
    // 플레이어 아이디 검증 
    private void VerifyPlayerId(Int32 number, ArraySegment<byte> buffer)
    {
        Boolean isOk = BitConverter.ToBoolean(buffer.Array, buffer.Offset + 4);

        if (isOk)
        {
            playerIdOk = true;
        }
        else
        {
            playerIdOk = false;
        }
    }
    //패킷 아이디 : PlayerAttackReq
    //공격 요청을 받았다 
    private void PlayerAttackReq(Int32 number, ArraySegment<byte> buffer)
    {
        ushort roomNumber = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4);
        Attack(roomNumber);
    }
    
    private void Attack(ushort roomNumber)
    {
        Console.WriteLine($"플레이분들 이제 가위, 바위, 보 중 하나를 결정해주세요");
        
        string cmd = Console.ReadLine();
        count++;
        
        Int32 stringParsing = Int32.Parse(cmd);
        
        ushort playerNumber = UInt16.Parse(playerId);
        PlayerAttackRes playerAttackRes = new()
        {
            size = 8,
            id = (ushort)PacketID.PlayerAttackRes,
            playerId = playerNumber,
            value = (ushort)stringParsing,
            RoomNumber = roomNumber,
        };

        Console.WriteLine($"playerNumber가 {playerNumber}인 저는 유저 게임{roomNumber}번 방에 값을 보냈습니다!");
        Console.WriteLine($"[Attack] : {ClientSession.Instance.GetHashCode()}");
        
        var openSegment = MakePacket.ClassToByte(playerAttackRes);
        ClientSession.Instance.Send(openSegment);    
        
    }
    
    private void PlayerExitRoom(ushort playerId)
    {
        PlayerExitReq playerExitReq = new()
        {
            size = 6,
            id = (ushort)PacketID.PlayerExitReq,
            playerId = playerId,
        };

       Console.WriteLine($"제 번호는 {playerId}입니다 퇴장을 요청하겠습니다");

       var segment = MakePacket.ClassToByte(playerExitReq);
       ClientSession.Instance.Send(segment);

    }
    private void PlayerEnterResponse(Int32 number, ArraySegment<byte> buffer)
    {
        ushort playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4); // 플레이어 아이디
        ushort roomNumber = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 6); // roomNumber 

        Console.WriteLine($"playerId  {playerId}님이 {roomNumber}에 입장되었습니다");
    }
}