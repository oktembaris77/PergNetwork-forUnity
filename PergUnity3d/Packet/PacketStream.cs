using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class PacketStream
    {
        public Packet packet = new Packet((int)AlreadyClientPackets.serializedData);
        public SerializedTargets targets = SerializedTargets.Others;
        public SerializationType serializationType = SerializationType.Continuous;
        public DataEvent dataEvent;
        public List<int> clientIdList = new List<int>();
        public List<int> clientSceneIdList = new List<int>();
        public int fromClient = -1;
        public int ownerClientId = -1;
        public int clientId = -1;
        public int packetId = -1;
        public bool permissionSend = true;
        internal bool packetWrited = false;
        internal bool packetLenghtWrited = false;

        public void WriteData(object _object, VarType _varType)
        {
            if (!packetWrited)
            {
                packet.Write((int)targets);
                packet.Write(ownerClientId);
                packet.Write(clientId);
                packet.Write(packetId);
                if (targets == SerializedTargets.SpecificClients)
                {
                    packet.Write(clientIdList.Count);
                    for (int i = 0; i < clientIdList.Count; i++)
                    {
                        packet.Write(clientIdList[i]);
                    }
                }
                packetWrited = true;
            }

            PergSerialized.AddCurrentWrittenData(new WrittenData(_object, _varType), clientId, packetId);

            packet.WriteObject(_object, _varType);
        }
        public void ClientSendData()
        {
            //Warning Messages
            PergMessages.SerializedPacketClientListWarnings(this, targets);

            //Last Data - Current Data Control
            if (serializationType == SerializationType.Trigger)
            {
                permissionSend = PergNetwork.PermissionToSend(/*PergSerialized.currentWrittenData*/PergSerialized.pergSerializationType[clientId][packetId].currentWrittenData, clientId, packetId);
                PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount = 0;
            }
            else if (serializationType == SerializationType.Continuous)
            {
                permissionSend = true;
            }
            else
            {

            }

            //Send
            if (permissionSend)
                PergSerialized.ClientSendSerializedDataPacket(packet);
        }
        public void ServerSendData()
        {
            int clientIdListCount = 0;
            switch (targets)
            {
                case SerializedTargets.All:
                    for (int i = 1; i <= Server.Server.MaxPlayers; i++)
                    {
                        if (Server.Server.clients[i].tcp.socket != null)
                        {
                            PergSerialized.ServerSendSerializedDataPacket(this, packet, i, !packetLenghtWrited);
                            packetLenghtWrited = true;
                        }
                    }
                    break;
                case SerializedTargets.OnlyMe:
                    PergSerialized.ServerSendSerializedDataPacket(this, packet, fromClient);
                    break;
                case SerializedTargets.Others:
                    for (int i = 1; i <= Server.Server.MaxPlayers; i++)
                    {
                        if (Server.Server.clients[i].tcp.socket != null && i != fromClient)
                        {
                            PergSerialized.ServerSendSerializedDataPacket(this, packet, i, !packetLenghtWrited);
                            packetLenghtWrited = true;
                        }
                    }
                    break;
                case SerializedTargets.SpecificClients:
                    //Read ClientIdList
                    clientIdListCount = packet.ReadInt();
                    for (int i = 0; i < clientIdListCount; i++)
                    {
                        clientIdList.Add(packet.ReadInt());
                    }

                    //Send Clients
                    for (int i = 1; i <= Server.Server.MaxPlayers; i++)
                    {
                        for (int k = 0; k < clientIdList.Count; k++)
                        {
                            if (Server.Server.clients[i].tcp.socket != null && i == clientIdList[k])
                            {
                                PergSerialized.ServerSendSerializedDataPacket(this, packet, i, !packetLenghtWrited);
                                packetLenghtWrited = true;
                                break;
                            }
                        }
                    }
                    break;
                case SerializedTargets.AllWithId:
                    packet.Write(fromClient);
                    for (int i = 1; i <= Server.Server.MaxPlayers; i++)
                    {
                        if (Server.Server.clients[i].tcp.socket != null)
                        {
                            PergSerialized.ServerSendSerializedDataPacket(this, packet, i, !packetLenghtWrited);
                            packetLenghtWrited = true;
                        }
                    }
                    break;
                case SerializedTargets.OthersWithId:
                    packet.Write(fromClient);
                    for (int i = 1; i <= Server.Server.MaxPlayers; i++)
                    {
                        if (Server.Server.clients[i].tcp.socket != null && i != fromClient)
                        {
                            PergSerialized.ServerSendSerializedDataPacket(this, packet, i, !packetLenghtWrited);
                            packetLenghtWrited = true;
                        }
                    }
                    break;
                case SerializedTargets.SpecificClientsWithId:
                    packet.Write(fromClient);
                    //Read ClientIdList
                    clientIdListCount = packet.ReadInt();
                    for (int i = 0; i < clientIdListCount; i++)
                    {
                        clientIdList.Add(packet.ReadInt());
                    }

                    //Send Clients
                    for (int i = 1; i <= Server.Server.MaxPlayers; i++)
                    {
                        for (int k = 0; k < clientIdList.Count; k++)
                        {
                            if (Server.Server.clients[i].tcp.socket != null && i == clientIdList[k])
                            {
                                PergSerialized.ServerSendSerializedDataPacket(this, packet, i, !packetLenghtWrited);
                                packetLenghtWrited = true;
                                break;
                            }
                        }
                    }
                    break;
            }
        }
        public object ReadData(VarType _varType)
        {
            return packet.ReadObject(_varType);
        }
        public void ClientIdListAdd(int[] clientIds)
        {
            for (int i = 0; i < clientIds.Length; i++)
            {
                clientIdList.Add(clientIds[i]);
            }
        }
        public void ClientIdListClear()
        {
            clientIdList.Clear();
        }
        public void ClientSceneIdListAdd(int[] sceneIds)
        {
            for (int i = 0; i < sceneIds.Length; i++)
            {
                clientSceneIdList.Add(sceneIds[i]);
            }
        }
        public void ClientSceneIdListClear()
        {
            clientSceneIdList.Clear();
        }
    }
}