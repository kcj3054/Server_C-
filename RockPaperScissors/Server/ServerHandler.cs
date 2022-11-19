using System.Net.Sockets;
using Common;
using ServerCommon;

namespace Server;

public class ServerHandler
{
    private static ServerHandler _serverHandler = new();
    public static ServerHandler Instance { get { return _serverHandler; } }
    private Dictionary<Int32, Action<Int32, ArraySegment<byte>>> _packetHandlers = new();

    private object _lock = new();

    public void RegisterPacketHandler()
    {
        _packetHandlers.Add((Int32)PacketID.PlayerEnterReq, PlayerEnterRoom);
        // _packetHandlers.Add((Int32)PacketID.PlayerExitReq, PlayerExitRes);     // client -> server 퇴장 요청 
        _packetHandlers.Add((Int32)PacketID.PlayerAttackRes, PlayerAttackRes); // 플레이어가 가위 바위 보 중하나를 냄
        _packetHandlers.Add((Int32)PacketID.VerifyPlayerId, VerifyPlayerId); //client -> server playerId 검증.. !
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
                throw;
            }
        }
    }

    public void VerifyPlayerId(Int32 packetHeaderInfo, ArraySegment<byte> buffer)
    {
        ushort playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4);
        Console.WriteLine($"플레이어 ID{playerId}번을 사용하고 싶다고 요청이 왔습니다.");
        Boolean isOk;

        if (SessionManager.Instance._playerIdDic.ContainsKey(playerId))
        {
            isOk = false;
        }
        else
        {
            SessionManager.Instance._playerIdDic.TryAdd(playerId, true);
            isOk = true;
        }
        
        PlayerIdRes playerIdRes = new()
        {
            size = 5,
            id = (ushort)PacketID.VerifyPlayerId,
        };
        var openSegment = MakePacket.VerifyPlayerIdPacket(playerIdRes, isOk);
        
        Console.WriteLine($"{ServerSession.Instance._socket.GetHashCode()}");
        ServerSession.Instance._socket.Send(openSegment);  //이때는 왜 괜찮을까?....

    }

    //방에 입장하고나서부터  ServerSession.Instance._socket.Send로 사용하면 session가 꼬이게된다. 
    private void PlayerEnterRoom(Int32 packetHeaderInfo, ArraySegment<byte> buffer)
    {
        lock (_lock)
        {
            ushort playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4);

            Console.WriteLine($"playerId {playerId}번님이 입장하길원합니다.");

            //현재 playerId가 접속한 roomNumber를 리턴한다 
            ushort roomNumber = EnterRoom(playerId);
            Console.WriteLine($"현재 {roomNumber}번 방에 여분의 자리가 있어서 {playerId}님을 {roomNumber}에 입장시키겠습니다. ");

            //플에이어 방요청에 대한 빈방을 생성 후 응답. 
            PlayerEnterRes playerEnterRes = new()
            {
                size = 8,
                id = (Int32)PacketID.PlayerEnterRes,
                playerId = playerId,
                roomNumber = roomNumber,
            };

            var openSegment = MakePacket.ClassToByte(playerEnterRes);
            ServerSession.Instance._socket.Send(openSegment);
            Console.WriteLine($"[PlayerEnterRoom] socket HashCode : {ServerSession.Instance._socket.GetHashCode()} ");
            
            var gameRooms = RoomManager.Instance.gameRooms;
            foreach (var room in gameRooms)
            {
                if (room.Room.Count == 2)
                {
                    int cnt = 1;
                    AttackReq(room);
                }
            }
        }
    }

    //server -> client 퇴장에 대한 요청 처리 
    //패킷 수정 예정 
    // private void PlayerExitRes(Int32 number, ArraySegment<byte> buffer)
    // {
    //     ushort playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4); // 플레이어 아이디
    //     ushort roomNumber = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 7);
    //
    //     // _socket.Close();
    //     if (SessionManager.Instance._playerIdDic.ContainsKey(playerId))
    //     {
    //         Console.WriteLine($"playerId  {playerId}님이 {roomNumber}방에서 퇴장되셨습니다, playerId 삭제");
    //         SessionManager.Instance._playerIdDic.Remove(playerId, out bool isSuccess);
    //     }
    //
    //     //퇴장 되었다고 플레이어에게 알림 
    //     PlayerExitRes playerExitRes = new()
    //     {
    //         size = 6,
    //         id = (ushort)PacketID.PlayerExitRes,
    //         isExit = true,
    //     };
    //     
    //     var segment = MakePacket.ClassToByte(playerExitRes);
    //     ServerSession.Instance._socket.Send(segment);
    //     
    //     SessionManager.Instance._playerIdDic.Remove(playerId, out bool value);
    //     Console.WriteLine($"{playerId}번님이 퇴장해서 보관 중인 playerId 목록에서 {playerId}님을 삭제합니다 ");
    // }

    //요청 받은 room에 입장한 session들에게 공격을 요청한다 
    private void AttackReq(GameRoom room)
    {
        foreach (var sessionId in room.Room)
        {
            PlayerAttackReq playerAttackReq = new()
            {
                size = 8,
                id = (Int32)PacketID.PlayerAttackReq,
                RoomNumber = room.RoomId, // !!! roomID Check.!!! 
            };

            var open = MakePacket.ClassToByte(playerAttackReq);

            // ServerSession.Instance.Send(open);     ServerSession으로 보내버리면 바로 꼬여버린다.. 
            // Console.WriteLine($"[AttackReq] : {ServerSession.Instance?.GetHashCode()}");
            
            if (SessionManager.Instance._sessionsDic.ContainsKey(sessionId))
            {
                Console.WriteLine($"해당 세션들에게 공격요청을 보냅니다.!{sessionId}");
            
                Socket? socket;
                SessionManager.Instance._sessionsDic.TryGetValue(sessionId, out socket);
                socket?.Send(open);
            
                Console.WriteLine($"[AttackReq] : {socket?.GetHashCode()}");
            
            }
        }
    }

    private void PlayerAttackRes(Int32 packetHeaderInfo, ArraySegment<byte> buffer)
    {
        ushort playerId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 4);
        ushort value = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 6);
        ushort roomNumber = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 8);

        //다른 세션인데 같은 값을 사용하고있다.. ~ => 두번째 소켓연결만 사용한다. 첫번째 소켓은 어디갔지?!!!
        // Console.WriteLine(
        //     $"[PlayerAttackRes] : {playerId}님의 HashCode : {ServerSession.Instance._socket.GetHashCode()}"); //ServerSession.Instance._socket으로 사용하는 곳이 이부분밖에 없다..! 
        //Console.WriteLine($"{playerId}님의 sessionId : {}");
        Console.WriteLine($"{playerId}님이 내신 값은 {value}입니다");

        var gameRooms = RoomManager.Instance.gameRooms;

        foreach (var room in gameRooms)
        {
            if (room.RoomId == roomNumber)
            {
                room.playerAttackValue.TryAdd(playerId, value);
                if (room.playerAttackValue.Count == 2)
                {
                    //대결하러가자 , 결과의 기준은 간단하게 하기 위해서 player1을 기준으로한다 
                    int player1 = 0;
                    int player2 = 0;

                    lock (_lock)
                    {
                        foreach (var number in room.playerAttackValue)
                        {
                            if (player1 == 0)
                            {
                                player1 = number.Value;
                            }
                            else
                            {
                                player2 = number.Value;
                            }
                        }
                        room.playerAttackValue.Clear(); ////결과를 냈으니 p1, p2 플레이어가 낸 값을들을 초기화하자  
                    }
                    Int32 result = RockPaperSx.Instance.RockPaper(player1, player2);

                    var firstPlayerSession = room.Room[0]; //이런 식으로 sessionId를 뽑아오자.. 
                    var firstSocket = SessionManager.Instance._sessionsDic[firstPlayerSession];
                    Console.WriteLine($"firstSocket : {firstSocket.GetHashCode()}");

                    var secondPlayerSession = room.Room[1]; //이런 식으로 sessionId를 뽑아오자.. 
                    var secondSocket = SessionManager.Instance._sessionsDic[secondPlayerSession];
                    Console.WriteLine($"secondSocket : {secondSocket.GetHashCode()}");

                    GameResultRes gameResultRes = new()
                    {
                        size = 7,
                        id = (ushort)PacketID.GameResult,
                        RoomNumber = room.RoomId,
                        isVictory = true,
                    };

                    byte[] sampleByte = new byte[1024];
                    ArraySegment<byte> open = new(sampleByte);

                    byte[] buffer22 = BitConverter.GetBytes(gameResultRes.size);
                    byte[] buffer2 = BitConverter.GetBytes(gameResultRes.id);
                    byte[] buffer3 = BitConverter.GetBytes(gameResultRes.RoomNumber);

                    Array.Copy(buffer22, 0, open.Array, open.Offset, buffer22.Length);
                    Array.Copy(buffer2, 0, open.Array, open.Offset + buffer22.Length, buffer2.Length);
                    Array.Copy(buffer3, 0, open.Array, open.Offset + buffer22.Length + buffer2.Length, buffer3.Length);

                    //client로 보낼 결과 값 
                    if (result == (Int32)Enums.WIN)
                    {
                        //승리 패킷 
                        byte[] buffer4 = BitConverter.GetBytes(gameResultRes.isVictory);
                        Array.Copy(
                            buffer4, 0, open.Array, open.Offset + buffer22.Length + buffer2.Length + buffer3.Length,
                            buffer4.Length);

                        firstSocket.Send(open);
                        Console.WriteLine($"Victory FirstSocket : {firstSocket.GetHashCode()}");

                        //패배 패킷
                        gameResultRes.isVictory = false;
                        buffer4 = BitConverter.GetBytes(gameResultRes.isVictory);
                        Array.Copy(
                            buffer4, 0, open.Array, open.Offset + buffer22.Length + buffer2.Length + buffer3.Length,
                            buffer4.Length);
                        secondSocket.Send(open);
                        Console.WriteLine($"Victory SecondSocket : {secondSocket.GetHashCode()}");
                    }
                    else if (result == (Int32)Enums.DREW)
                    {
                        GameRestart gameRestart = new()
                        {
                            size = 4,
                            id = (ushort)PacketID.GameReStart
                        };

                        byte[] regameByte = new byte[1024];
                        ArraySegment<byte> regameSegment = new(regameByte);

                        buffer22 = BitConverter.GetBytes(gameRestart.size);
                        buffer2 = BitConverter.GetBytes(gameRestart.id);

                        Array.Copy(buffer22, 0, regameSegment.Array, regameSegment.Offset, buffer22.Length);
                        Array.Copy(
                            buffer2, 0, regameSegment.Array, regameSegment.Offset + buffer22.Length, buffer2.Length);

                        //모든 플레이어에게 다시 게임을 시작할 것을 요구한다 
                        firstSocket.Send(regameSegment);
                        secondSocket.Send(regameSegment);
                    }
                    else //졌을 때 
                    {
                        //패배 패킷
                        gameResultRes.isVictory = false;
                        byte[] buffer4 = BitConverter.GetBytes(gameResultRes.isVictory);
                        Array.Copy(
                            buffer4, 0, open.Array, open.Offset + buffer22.Length + buffer2.Length + buffer3.Length,
                            buffer4.Length);
                        firstSocket.Send(open);

                        Console.WriteLine($"Loose FirstSocket : {firstSocket.GetHashCode()}");
                        //승리 패킷  
                        gameResultRes.isVictory = true;
                        buffer4 = BitConverter.GetBytes(gameResultRes.isVictory);
                        Array.Copy(
                            buffer4, 0, open.Array, open.Offset + buffer22.Length + buffer2.Length + buffer3.Length,
                            buffer4.Length);
                        secondSocket.Send(open);

                        Console.WriteLine($"Loose SecondSocket : {secondSocket.GetHashCode()}");
                    }
                    room.Room.Clear();
                    room.playerAttackValue.Clear();
                    //clear 문 추가 
                }
                break;
            }
        }
    }

    private ushort EnterRoom(ushort playerId)
    {
        Socket socket;

        if (SessionManager.Instance._sessionsDic.ContainsKey(ServerSession.Instance.sessionId))
        {
            SessionManager.Instance._sessionsDic.TryGetValue(ServerSession.Instance.sessionId, out socket);
        }

        //첫방 생성 시.. 
        Boolean isEnter = false;
        ushort roomNumber = 0;
        var gameRooms = RoomManager.Instance.gameRooms;

        if (gameRooms.Count == 0)
        {
            roomNumber = 1;
            RoomManager.Instance.gameRooms.Add(new GameRoom());
            var gameRoom = new GameRoom();

            gameRoom.RoomId = 1;
            gameRoom.Enter(ServerSession.Instance.sessionId);
            Console.WriteLine($"EnterROOM SessionId : {ServerSession.Instance.sessionId}");

            Int32 roomIndex = roomNumber;
            RoomManager.Instance.gameRooms[--roomIndex] = gameRoom;
        }
        else
        {
            foreach (GameRoom room in gameRooms)
            {
                if (room.Room.Count < 2)
                {
                    room.Enter(ServerSession.Instance.sessionId);
                    Console.WriteLine($"EnterROOM SessionId : {ServerSession.Instance.sessionId}");
                    isEnter = true;

                    roomNumber = room.RoomId;
                    break;
                }
                roomNumber = room.RoomId;
            }

            //room에 들어가지 못햇을 경우 room을 하나 만들어서 넣자 
            if (!isEnter)
            {
                GameRoom newGameRoom = new();
                RoomManager.Instance.gameRooms.Add(newGameRoom);
                roomNumber = (ushort)RoomManager.Instance.gameRooms.Count;

                newGameRoom.RoomId = roomNumber;

                //newGameRoom.Enter(_socket);
                newGameRoom.Enter(ServerSession.Instance.sessionId);
                Int32 roomIndex = roomNumber;

                RoomManager.Instance.gameRooms[--roomIndex] = newGameRoom;
                isEnter = true;
            }
        }
        return roomNumber;
    }

    //결과 값을 서버도 찍을려고 console 로그 찍어놓자 

}