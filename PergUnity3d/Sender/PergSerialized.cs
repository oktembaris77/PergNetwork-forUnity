using System;
using System.Collections.Generic;
using System.Text;
using PergUnity3d;
using PergUnity3d.Server;

public class PergSerialized
{
    public static Dictionary<int, Dictionary<int, PergSerializationType>> pergSerializationType = new Dictionary<int, Dictionary<int, PergSerializationType>>();
    internal static void ClientSendSerializedDataPacket(Packet packet)
    {
        packet.WriteLength();
        PergUnity3d.Client.instance.udp.SendData(packet);
    }
    internal static void ServerSendSerializedDataPacket(PacketStream packetStream, Packet packet, int clientId, bool writeLenght = true)
    {
        if (packetStream.clientSceneIdList.Count > 0)
        {
            foreach (int id in packetStream.clientSceneIdList)
            {
                if(id == Server.clients[clientId].sceneId)
                {
                    if (writeLenght)
                        packet.WriteLength();
                    Server.clients[clientId].udp.SendData(packet);
                }
            }
            ClearSpesificScenesList(packetStream);
            packetStream.ClientSceneIdListClear();
        }
        else
        {
            if (writeLenght)
                packet.WriteLength();
            Server.clients[clientId].udp.SendData(packet);
        }
    }
    internal static void ClearSpesificScenesList(PacketStream packetStream)
    {
        packetStream.clientSceneIdList.Clear();
    }
    public static void AddCurrentWrittenData(WrittenData writtenData, int clientId, int packetId)
    {
        if(!pergSerializationType.TryGetValue(clientId, out Dictionary<int,PergSerializationType> v1))
        {
            //Tanımla
            pergSerializationType.Add(clientId, new Dictionary<int, PergSerializationType>());
            pergSerializationType[clientId].Add(packetId, new PergSerializationType());

            //Ekle
            pergSerializationType[clientId][packetId].serializedParameterCount++;
            pergSerializationType[clientId][packetId].currentWrittenData.Add(writtenData);
        }
        else
        {
            if (!pergSerializationType[clientId].TryGetValue(packetId, out PergSerializationType v2))
            {
                //Tanımla
                pergSerializationType[clientId].Add(packetId, new PergSerializationType());

                //Ekle
                pergSerializationType[clientId][packetId].serializedParameterCount++;
                pergSerializationType[clientId][packetId].currentWrittenData.Add(writtenData);
            }
            else
            {
                //Ekle
                pergSerializationType[clientId][packetId].serializedParameterCount++;
                pergSerializationType[clientId][packetId].currentWrittenData.Add(writtenData);
            }
        }
    }
    public static void AddServerPacketHandler(int key, Server.PacketHandler packetHandler)
    {
        Server.packetHandlers.Add(key, packetHandler);
    }
    public static void AddClientPacketHandler(int key, PergUnity3d.Client.PacketHandler packetHandler)
    {
        PergUnity3d.Client.packetHandlers.Add(key, packetHandler);
    }
}