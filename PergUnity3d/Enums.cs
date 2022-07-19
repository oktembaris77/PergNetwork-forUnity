using System;
using System.Collections.Generic;
using System.Text;

public enum Targets
{
    All,                     // Yerelde bana ve herkese
    OnlyMe,                  // Server üzerinden sadece bana
    Others,                  // Ben hariç herkes
    SpecificClients,         // Sadece belirli clientler
    SpecificClientsForServer,// Serverdan cliente, sadece belirli clientler
    AllViaServer,            // Server üzerinden herkese
    AllBuffered,             // Yerelde bana ve oyuna sonradan girenler dahil herkese
    OthersBuffered,          // Ben hariç, oyuna sonradan girenler dahil herkese
    AllBufferedViaServer,    // Oyuna sonradan girenler dahil, server üzerinden herkese
    ServerOnly               // Sadece servera
}                            
public enum SerializedTargets
{                            
    All,                     // Herkese
    OnlyMe,                  // Server üzerinden sadece bana
    Others,                  // Ben hariç herkes
    SpecificClients,         // Sadece belirli clientler
    AllWithId,               // Gelen verilerin en sonunda, paketin geldiği clientId'si vardır.
    OthersWithId,            // Gelen verilerin en sonunda, paketin geldiği clientId'si vardır.
    SpecificClientsWithId    // Gelen verilerin en sonunda, paketin geldiği clientId'si vardır.
}

public enum Protocols
{
    UDP,
    TCP
}

public enum VarType
{
    none,
    _short,
    _int,
    _long,
    _float,
    _string,
    _byte,
    _boolean,
    _Vector3,
    _Quaternion,
}

public enum DataEvent
{
    Write,
    Read
}

public enum ServerType
{
    HostClient,
    CentralServer,
    none
}

public enum ObjectType
{
    none,
    Player,
    GameObject
}

public enum CallbackTypes
{
    PergSerialized,
    ConnectionCallbacks,
    LobbyCallbacks,
    InRoomCallbacks
}

public enum CallbackMethods
{
    OnConnectedServer,
    OnDisconnectedServer,
    OnJoinedLobby,
    OnPlayerEnteredRoom,
    OnInstantiated
}

public enum SerializationType
{
    Continuous,
    Trigger,
    none
}