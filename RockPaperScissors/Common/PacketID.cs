namespace ServerCommon;

public enum PacketID
{
    //플에이어 입장 요청 응답
    PlayerEnterReq = 1,
    PlayerEnterRes = 2,
    
    //플레이어 퇴장, 요청 응답
    PlayerExitReq = 3,
    PlayerExitRes = 4,
    
    //플레이어가 '가위, 바위, 보' 중 하나의 값을 낸다면 그것은 결정하는 것이다  해당 행위를 attack으로 네이밍 
    PlayerAttackRes = 5,
    PlayerAttackReq = 6, // 서버 -> 클라가 공격 요청을 보내는 것이다. 
    
    //==============================================
    GameResult = 100,
    GameReStart = 101,
    
    //================================================
    ShowRoomList = 200,
    
    
    //===========================================
    VerifyPlayerId = 500,
}