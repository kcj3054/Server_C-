using System.Runtime.InteropServices;
using Common;
using ServerCommon;
namespace Client;

public class StartGame
{ 
    public void Start()
    {
        Console.WriteLine("게임을 시작하겠습니다 !  소문자로 yes를 누르면 게임이 시작됩니다\n다른 숫자를 것을 입력하면 종료됩니다.");
        if (Console.ReadLine() == "yes")
        {
            InputPlayerId();
            EnterPlayer();
        }
        //cnt값을 조절하면 console.ReadLine으로 조절이 가능하다 -> 판수를 조절해야한다 
        while (true)
        {
            // if (ClientHandler.count > 3)
            // {
            //     var a = Console.ReadLine();
            // }
            ;
        }
    }
    private void InputPlayerId()
    {
        while (true)
        {
            Console.WriteLine("플레이어 번호를 10 ~ 999번 사이에서 입력해주세요 !!");
            ClientHandler.playerId = Console.ReadLine();
            ClientHandler.count++;
            
            int checkNumber = int.Parse(ClientHandler.playerId);
            
            if (checkNumber >= 10 && checkNumber <= 999)
            {
                VerifyPlayerId(checkNumber);
                Thread.Sleep(100);

                if (ClientHandler.playerIdOk)
                {
                    break;
                }
                Console.WriteLine("이미 다른 플레이어가 선점한 아이디어입니다.\n재시도하거나 다른 번호를 눌러주세요");
            }
            else
            {
                Console.WriteLine("다시 입력해주세요 10 ~ 999번 사이에서 입력해주세요");
            }
        }
    }
    private void VerifyPlayerId(Int32 playerNum)
    {
        PlayerIdReq playerIdReq = new()
        {
            size = 6,
            id = (ushort)PacketID.VerifyPlayerId,
            value = (ushort)playerNum
        };
        
        var verifySegment = MakePacket.ClassToByte(playerIdReq);
        Console.WriteLine($"[VerifyPlayerId] : {ClientSession.Instance._socket.GetHashCode()}");
        ClientSession.Instance.Send(verifySegment);
    }
    private void EnterPlayer()
    {
        //플레이어 입장 
        PlayerEnterReq playerEnterReq = new()
        {
            size = 8,
            id = (Int32)PacketID.PlayerEnterReq,
            playerId = ushort.Parse(ClientHandler.playerId),
        };

        var openSegment = MakePacket.ClassToByte(playerEnterReq);
        Console.WriteLine($"[EnterPlayer] : {ClientSession.Instance._socket.GetHashCode()}");
        ClientSession.Instance.Send(openSegment);
    }
}