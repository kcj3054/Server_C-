using System.Collections.Concurrent;
using System.Net.Sockets;
using Common;

namespace Server;

public class SessionManager : ServerSession
{
    static SessionManager sessionManager = new();
    public static SessionManager Instance { get { return sessionManager; } }

    public Int32 _sessionId = 0;
    private Object _lock = new();
    public ConcurrentDictionary<Int32, Socket> _sessionsDic = new();
    public ConcurrentDictionary<Int32, Boolean> _playerIdDic = new();
    
    public void Generate(Socket socket)
    {
        ServerSession session = new();
        
        lock (_lock)
        {
            _sessionId++;
            session.sessionId = _sessionId;
        }

        ServerSession.Instance.Init(socket);
        _sessionsDic.TryAdd(_sessionId, socket);
        
        Console.WriteLine($"Connected : sessionId{_sessionId}이 생성되었습니다.");
    }

    //현재 사용을 하지 않으므로 수정하지 않았습니다. 
    public Session? Find(int id)
    {
        ServerSession? session = new();

        //_sessionsDic.TryGetValue(id, out session);
        return session;
    }
    
    //현재 사용을 하지 않으므로 수정하지 않았습니다. 
    public void Remove(Session session)
    {
        //_sessionsDic.Remove(session.sessionId, out ServerSession? serverSession);
    }
}