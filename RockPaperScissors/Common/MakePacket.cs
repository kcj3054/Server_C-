namespace Common;

public static class MakePacket
{
    static byte[] segment = new byte[1024];
    
    public static ArraySegment<byte> ClassToByte(Packet packet)
    {
        if (packet is PlayerIdReq)
        {
            PlayerIdReq playerIdReq = new();
            playerIdReq = (PlayerIdReq)packet;
            
            ArraySegment<byte> verifySegment = new(segment);
            byte[] verifyBuf22=BitConverter.GetBytes(playerIdReq.size);
            byte[] verifyBuf2 = BitConverter.GetBytes(playerIdReq.id);
            byte[] verifyBuf3 = BitConverter.GetBytes(playerIdReq.value);

            Array.Copy(verifyBuf22, 0, verifySegment.Array, verifySegment.Offset, verifyBuf22.Length);
            Array.Copy(verifyBuf2, 0, verifySegment.Array, verifySegment.Offset + verifyBuf22.Length, verifyBuf2.Length);
            Array.Copy(verifyBuf3, 0, verifySegment.Array, verifySegment.Offset + verifyBuf22.Length + verifyBuf2.Length, verifyBuf3.Length);

            return verifySegment;
        }
        else if (packet is PlayerEnterReq)
        {
            PlayerEnterReq playerEnterReq = new();
            playerEnterReq = (PlayerEnterReq)packet;
            
            ArraySegment<byte> openSegment = new(segment);
            
            byte[] buffer = BitConverter.GetBytes(playerEnterReq.size);
            byte[] buffer2 = BitConverter.GetBytes(playerEnterReq.id);
            byte[] buffer3 = BitConverter.GetBytes(playerEnterReq.playerId);

            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            Array.Copy(
                buffer3, 0, openSegment.Array, openSegment.Offset + buffer.Length + buffer2.Length, buffer3.Length);
            
            return openSegment;
        }
        else if (packet is PlayerAttackRes)
        {
            PlayerAttackRes playerAttackRes = new();
            playerAttackRes = (PlayerAttackRes)packet;
            
            ArraySegment<byte> openSegment = new(segment);

            byte[] buffer22 = BitConverter.GetBytes(playerAttackRes.size);
            byte[] buffer2 = BitConverter.GetBytes(playerAttackRes.id);
            byte[] buffer3 = BitConverter.GetBytes(playerAttackRes.playerId);
            byte[] buffer4 = BitConverter.GetBytes(playerAttackRes.value);
            byte[] buffer5 = BitConverter.GetBytes(playerAttackRes.RoomNumber);

            Array.Copy(buffer22, 0, openSegment.Array, openSegment.Offset, buffer22.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer22.Length, buffer2.Length);
            Array.Copy(
                buffer3, 0, openSegment.Array, openSegment.Offset + buffer22.Length + buffer2.Length, buffer3.Length);
            Array.Copy(
                buffer4, 0, openSegment.Array,
                openSegment.Offset + buffer22.Length + buffer2.Length + buffer3.Length
                , buffer4.Length);
            Array.Copy(
                buffer5, 0, openSegment.Array,
                openSegment.Offset + buffer22.Length + buffer2.Length + buffer3.Length + buffer4.Length
                , buffer5.Length);
            return openSegment;
        }
        else if (packet is PlayerExitReq)
        {
            PlayerExitReq playerExitReq = new();
            playerExitReq = (PlayerExitReq)packet;
            
            ArraySegment<byte> openSegment = new(segment);
        
            byte[] buffer1 = BitConverter.GetBytes(playerExitReq.size);
            byte[] buffer2 = BitConverter.GetBytes(playerExitReq.id);
            byte[] buffer3 = BitConverter.GetBytes(playerExitReq.playerId);
        
            Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
            Array.Copy(buffer3, 0, openSegment.Array, openSegment.Offset + buffer1.Length + buffer2.Length, buffer3.Length);

            return openSegment;
        }
        else if (packet is PlayerEnterRes)
        {
            PlayerEnterRes playerEnterReq = new();
            playerEnterReq = (PlayerEnterRes)packet;
            
            ArraySegment<byte> openSegment = new(segment);

            byte[] buffer22 = BitConverter.GetBytes(playerEnterReq.size);
            byte[] buffer2 = BitConverter.GetBytes(playerEnterReq.id);
            byte[] buffer3 = BitConverter.GetBytes(playerEnterReq.playerId);
            byte[] buffer4 = BitConverter.GetBytes(playerEnterReq.roomNumber);

            Array.Copy(buffer22, 0, openSegment.Array, openSegment.Offset, buffer22.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer22.Length, buffer2.Length);
            Array.Copy(
                buffer3, 0, openSegment.Array, openSegment.Offset + buffer22.Length + buffer2.Length, buffer3.Length);
            Array.Copy(
                buffer4, 0, openSegment.Array, openSegment.Offset + buffer22.Length + buffer2.Length + buffer3.Length,
                buffer4.Length);
        }
        else if (packet is PlayerAttackReq)
        {
            PlayerAttackReq playerAttackReq = new();
            playerAttackReq = (PlayerAttackReq)packet;
            
            ArraySegment<byte> open = new(segment);
            
            byte[] buffer22 = BitConverter.GetBytes(playerAttackReq.size);
            byte[] buffer2 = BitConverter.GetBytes(playerAttackReq.id);
            byte[] buffer4 = BitConverter.GetBytes(playerAttackReq.RoomNumber);

            Array.Copy(buffer22, 0, open.Array, open.Offset, buffer22.Length);
            Array.Copy(buffer2, 0, open.Array, open.Offset + buffer22.Length, buffer2.Length);
            Array.Copy(
                buffer4, 0, open.Array, open.Offset + buffer22.Length + buffer2.Length,
                buffer4.Length);
            
            return open;
        }
        else if (packet is PlayerExitRes)
        {
            PlayerExitRes playerExitRes = new();
            playerExitRes = (PlayerExitRes)packet;
            
            ArraySegment<byte> openSegment = new(segment);
        
            byte[] buffer1 = BitConverter.GetBytes(playerExitRes.size);
            byte[] buffer2 = BitConverter.GetBytes(playerExitRes.id);
            byte[] buffer3 = BitConverter.GetBytes(playerExitRes.isExit);
        
            Array.Copy(buffer1, 0, openSegment.Array, openSegment.Offset, buffer1.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer1.Length, buffer2.Length);
            Array.Copy(buffer3, 0, openSegment.Array, openSegment.Offset + buffer1.Length + buffer2.Length, buffer3.Length);

            return segment;
        }
        else if (packet is GameResultRes)
        {
            GameResultRes gameResultRes = new();
            gameResultRes = (GameResultRes)packet;

        }
        return null;
    }

    public static ArraySegment<byte> VerifyPlayerIdPacket(Packet packet, Boolean isOK)
    {
        PlayerIdRes playerIdRes = new();
        playerIdRes = (PlayerIdRes)packet;
        
        ArraySegment<byte> openSegment = new (segment);
        if (isOK)
        {
            playerIdRes.isOk = true;

            byte[] buffer22 = BitConverter.GetBytes(playerIdRes.size);
            byte[] buffer2 = BitConverter.GetBytes(playerIdRes.id);
            byte[] buffer3 = BitConverter.GetBytes(playerIdRes.isOk);

            Array.Copy(buffer22, 0, openSegment.Array, openSegment.Offset, buffer22.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer22.Length, buffer2.Length);
            Array.Copy(
                buffer3, 0, openSegment.Array, openSegment.Offset + buffer22.Length + buffer2.Length, buffer3.Length);
        }
        else
        {
            playerIdRes.isOk = false;

            byte[] buffer22 = BitConverter.GetBytes(playerIdRes.size);
            byte[] buffer2 = BitConverter.GetBytes(playerIdRes.id);
            byte[] buffer3 = BitConverter.GetBytes(playerIdRes.isOk);

            Array.Copy(buffer22, 0, openSegment.Array, openSegment.Offset, buffer22.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer22.Length, buffer2.Length);
            Array.Copy(
                buffer3, 0, openSegment.Array, openSegment.Offset + buffer22.Length + buffer2.Length, buffer3.Length);
        }
        return openSegment;
    }
    
}