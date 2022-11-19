# Sequence Diagram

```mermaid
sequenceDiagram
    participant C as Client
    participant GS as GameServer Server
  
    autonumber
    
    C ->> GS: ReqEnter
    GS ->> C: ShowRoomList
    C ->> GS: EnterRoom
    
    
```