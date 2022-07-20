using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PergUnity3d.Server
{
    public class ServerSend
    {
        private static void SendTCPData(int _toClient, int _fromClient, Packet _packet, bool packetLenghtWrited = true)
        {
            if(_fromClient == -1 || Server.clients[_toClient].roomKey == Server.clients[_fromClient].roomKey)// -1 -> Server
            {
                if (PergUnity3d.PergRPC.clientSceneIdList.Count > 0)
                {
                    foreach (int id in PergUnity3d.PergRPC.clientSceneIdList)
                    {
                        if (id == Server.clients[_toClient].sceneId)
                        {
                            if (packetLenghtWrited)
                                _packet.WriteLength();
                            Server.clients[_toClient].tcp.SendData(_packet);
                            break;
                        }
                    }
                    ClearSpesificScenesList();
                }
                else
                {
                    if (packetLenghtWrited)
                        _packet.WriteLength();
                    Server.clients[_toClient].tcp.SendData(_packet);
                }
            }
        }
        private static void SendUDPData(int _toClient, int _fromClient, Packet _packet, bool packetLenghtWrited = true)
        {
            if (_fromClient == -1 || Server.clients[_toClient].roomKey == Server.clients[_fromClient].roomKey)// -1 -> Server
            {
                if (PergUnity3d.PergRPC.clientSceneIdList.Count > 0)
                {
                    foreach (int id in PergUnity3d.PergRPC.clientSceneIdList)
                    {
                        if (id == Server.clients[_toClient].sceneId)
                        {
                            if (packetLenghtWrited)
                                _packet.WriteLength();
                            Server.clients[_toClient].udp.SendData(_packet);
                            break;
                        }
                    }
                    ClearSpesificScenesList();
                }
                else
                {
                    if (packetLenghtWrited)
                        _packet.WriteLength();
                    Server.clients[_toClient].udp.SendData(_packet);
                }
            }
        }
        private static void SendTCPDataToAll(int _fromClient, Packet _packet) //For u silindi
        {
            _packet.WriteLength();
            for (int i = 0; i < Server.clients.Count; i++)
            {
                if ( _fromClient == -1 || Server.clients.ElementAt(i).Value.roomKey == Server.clients[_fromClient].roomKey)// -1 -> Server
                {
                    if (PergUnity3d.PergRPC.clientSceneIdList.Count > 0)
                    {
                        foreach (int id in PergUnity3d.PergRPC.clientSceneIdList)
                        {
                            if (Server.clients.ElementAt(i).Value.sceneId == id)
                            {
                                Server.clients.ElementAt(i).Value.tcp.SendData(_packet);
                            }
                        }
                        ClearSpesificScenesList();
                    }
                    else
                    {
                        Server.clients.ElementAt(i).Value.tcp.SendData(_packet);
                    }
                }
            }
        }
        private static void SendUDPDataToAll(int _fromClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (_fromClient == -1 || Server.clients[i].roomKey == Server.clients[_fromClient].roomKey)// -1 -> Server
                {
                    if (PergUnity3d.PergRPC.clientSceneIdList.Count > 0)
                    {
                        foreach (int id in PergUnity3d.PergRPC.clientSceneIdList)
                        {
                            if (Server.clients[i].sceneId == id)
                            {
                                Server.clients[i].udp.SendData(_packet);
                            }
                        }
                        ClearSpesificScenesList();
                    }
                    else
                    {
                        Server.clients[i].udp.SendData(_packet);
                    }
                }
            }
        }
        private static void SendTCPDataToAll(int _except, int _fromClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _except)
                {
                    if (_fromClient == -1 || Server.clients[i].roomKey == Server.clients[_fromClient].roomKey)// -1 -> Server
                    {
                        if (PergUnity3d.PergRPC.clientSceneIdList.Count > 0)
                        {
                            foreach (int id in PergUnity3d.PergRPC.clientSceneIdList)
                            {
                                if (Server.clients[i].sceneId == id)
                                {
                                    Server.clients[i].tcp.SendData(_packet);
                                }
                            }
                            ClearSpesificScenesList();
                        }
                        else
                        {
                            Server.clients[i].tcp.SendData(_packet);
                        }
                    }
                }
            }
        }
        private static void SendUDPDataToAll(int _except, int _fromClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _except)
                {
                    if (_fromClient == -1 || Server.clients[i].roomKey == Server.clients[_fromClient].roomKey)// -1 -> Server
                    {
                        if (PergUnity3d.PergRPC.clientSceneIdList.Count > 0)
                        {
                            foreach (int id in PergUnity3d.PergRPC.clientSceneIdList)
                            {
                                if (Server.clients[i].sceneId == id)
                                {
                                    Server.clients[i].udp.SendData(_packet);
                                }
                            }
                            ClearSpesificScenesList();
                        }
                        else
                        {
                            Server.clients[i].udp.SendData(_packet);
                        }
                    }
                }
            }
        }

        #region Packets
        //Burası gönderilen paketlerin bölgesi
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_fromClient"></param>
        /// <param name="parameterInfos"></param>
        /// <param name="parameters"></param>
        /// <param name="targets"></param>
        /// <param name="protocols"></param>
        /// <param name="fromSendBuffer">Bu method eğer SendBuffer'dan çağrıldıysa "true" olur.</param>
        /// <param name="fromServerHandle">Bu method eğer ServerHandle'dan çağrıldıysa "true" olur.</param>
        public static void PergRPC(int _fromClient, ParameterInfo[] parameterInfos, object[] parameters, Targets targets, Protocols protocols, bool fromSendBuffer = false, bool fromServerHandle = false)
        {
            using (Packet _packet = new Packet((int)AvailableServerPackets.pergRPC))
            {
                _packet.Write((int)parameters[0]);
                for (int i = 1; i <= parameterInfos.Length; i++)
                {
                    _packet.WriteObject(parameters[i], parameterInfos[i - 1].ParameterType.Name);
                }

                //Targete göre verileri clientlerine gönder
                switch (targets)
                {
                    case Targets.All:
                        if(protocols == Protocols.TCP) SendTCPDataToAll(_fromClient, _fromClient, _packet); // Gönderen istemciye gitmicek, yerelde çalışacak
                        else SendUDPDataToAll(_fromClient, _fromClient, _packet);
                        break;
                    case Targets.OnlyMe:
                        int fromClient = _fromClient;
                        if(fromSendBuffer)
                            fromClient = Buffers.interviewRPCBufferKey;

                        if (protocols == Protocols.TCP) SendTCPData(_fromClient, fromClient, _packet);
                        else SendUDPData(_fromClient, fromClient, _packet);
                        break;
                    case Targets.Others:
                        if (protocols == Protocols.TCP) SendTCPDataToAll(_fromClient, _fromClient, _packet);
                        else SendUDPDataToAll(_fromClient, _fromClient, _packet);
                        break;
                    case Targets.SpecificClients:
                    case Targets.SpecificClientsForServer:
                        bool packetLenghtWrited = true;

                        if (protocols == Protocols.TCP)
                        {
                            foreach (int id in PergUnity3d.PergRPC.clientIdList)
                            {
                                SendTCPData(id, _fromClient, _packet, packetLenghtWrited);
                                if (packetLenghtWrited)
                                    packetLenghtWrited = false;
                            }
                        }
                        else
                        {
                            foreach (int id in PergUnity3d.PergRPC.clientIdList)
                            {
                                SendUDPData(id, _fromClient, _packet, packetLenghtWrited);
                                if (packetLenghtWrited)
                                    packetLenghtWrited = false;
                            }
                        }
                        PergUnity3d.PergRPC.clientIdList.Clear();
                        break;
                    case Targets.AllViaServer:
                        if (protocols == Protocols.TCP) SendTCPDataToAll(_fromClient, _packet); // Gönderen istemciye de serverdan gidecek. Yerelde çalışmayacak.
                        else SendUDPDataToAll(_fromClient, _packet);
                        break;
                    case Targets.AllBuffered:
                        Debug.Log("ServerSend -> All Buffered - fromClient:" + _fromClient);
                        if (!fromSendBuffer)
                            Buffers.SetBuffers(_fromClient, parameters, protocols, PergUnity3d.PergRPC.clientSceneIdList.ToArray());
                        if (protocols == Protocols.TCP) SendTCPDataToAll(_fromClient, _fromClient, _packet); // Gönderen istemciye gitmicek, yerelde çalışacak
                        else SendUDPDataToAll(_fromClient, _fromClient, _packet);
                        break;
                    case Targets.OthersBuffered:
                        if (!fromSendBuffer)
                            Buffers.SetBuffers(_fromClient, parameters, protocols, PergUnity3d.PergRPC.clientSceneIdList.ToArray());
                        if (protocols == Protocols.TCP) SendTCPDataToAll(_fromClient, _fromClient, _packet); // Gönderen istemciye gitmeyecek
                        else SendUDPDataToAll(_fromClient, _fromClient, _packet);
                        break;
                    case Targets.AllBufferedViaServer:
                        if (!fromSendBuffer)
                            Buffers.SetBuffers(_fromClient, parameters, protocols, PergUnity3d.PergRPC.clientSceneIdList.ToArray());
                        if (protocols == Protocols.TCP) SendTCPDataToAll(_fromClient, _packet); // Gönderen istemciye de gidicek, gönderenin yerelinde çalışmayacak
                        else SendUDPDataToAll(_fromClient, _packet);
                        break;
                }
            }
        }
        #endregion
        internal static void ClearSpesificScenesList()
        {
            PergUnity3d.PergRPC.clientSceneIdList.Clear();
        }
    }
}