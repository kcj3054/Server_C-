using System.Net;
using System.Net.Sockets;
namespace Common;

public class Session
{ 
    public Int32 sessionId = 0;
    private static readonly Int32 HeaderSize = 2;
    public Socket? _socket;
    
    private Int32 _disconnected = 0;
    private object _lock = new();
    
   public void Init(Socket socket)
    {
        _socket = socket;
        sessionId++;
        
        if (_socket.Connected)
        {
            Console.WriteLine("Session클래스의 _socket이 연결되었습니다 ");
            
            SocketAsyncEventArgs recvArgs = new();
            SocketAsyncEventArgs sendArgs = new();
            
            sendArgs.Completed += (OnSendCompleted);
            recvArgs.Completed += (OnRecvCompleted);
            
            recvArgs.SetBuffer(new byte[1024], 0, 1024);
            RegisterRecv(recvArgs);
        }
        else
        {
            Console.WriteLine("Session클래스의 _socket이 연결되어있지않습니다 ");
        }
    }

    public void RegisterRecv(SocketAsyncEventArgs args)
    {
        // args 재사용할 때 터진다 args 다시 찾아보자 
        bool pending = _socket.ReceiveAsync(args);
        if (pending == false)
        {
            OnRecvCompleted(null, args);
        }
    }
    
    public void OnRecvCompleted(Object sender, SocketAsyncEventArgs args)
    {
        //hashCode 값 확인하기 s
        Console.WriteLine($"Hash Code : {_socket.GetHashCode()}");
        //사용 후 콜백함수 제거 ! 
        
        if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
        {
            OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
            
           // args.Completed -= (OnRecvCompleted);
            RegisterRecv(args);
        }
        else
        {
            Console.WriteLine($" OnRecvCompleted Error ");
        }

    }

    public void Send(ArraySegment<byte> buffer)
    {
        _socket?.Send(buffer);
    }
    public bool OnRecv(ArraySegment<byte> buffer)
    {
        int processLen = 0; // 패킷하나 끊을 때마다 processLen에 dataSize를 넣게되면 패킷 크기를 알 수 있다 
        bool isOk = false;
        while (true)
        {
            if (buffer.Count < HeaderSize)
            {
                isOk = false;
                break;
            }
            
            ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);

            if (buffer.Count < dataSize)
            {
                isOk = false;
                break;
            }
            
            OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
            break;
            // processLen += dataSize;
            // buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
        }
        
        return isOk;
        
    }
    public virtual void OnRecvPacket(ArraySegment<byte> buffer)
    {
        
    }
    public void OnSendCompleted(Object sender, SocketAsyncEventArgs args)
    {
        if (args.SocketError == SocketError.Success)
        {
            _socket?.Send(args.Buffer);
        }
        else
        {
            Console.WriteLine($" OnSendCompleted Error ");
        }
    }
    
    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            return;

        _socket?.Shutdown(SocketShutdown.Both);
        _socket?.Close();
    }
}