using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class PergNetwork
    {
        public static bool isClient;
        public static bool isServer;
        public static ServerType serverType;

        public static List<LoginRequestBufferClass> LoginRequestBufferList = new List<LoginRequestBufferClass>();
        public static List<ConnectedClientBufferClass> connectedClientBufferList = new List<ConnectedClientBufferClass>();
        public static Dictionary<string, int> GeneratedUniqueClientIdList = new Dictionary<string, int>();
        public static void ConnectToServer()
        {
            Client.instance.ConnectToServer();
        }
       
        //Server . clients scene id
        /// <summary>
        /// Sets the client's scene ID.
        /// </summary>
        /// <param name="ownerClientId">Owner client ID.</param>
        /// <param name="sceneId">New scene ID.</param>
        public static void SetClientSceneId(int ownerClientId, int sceneId)
        {
            PergRPC.SendMethod("SetClientSceneId", Targets.ServerOnly, Protocols.TCP, ownerClientId, sceneId);
        }
        public static void SendBuffers(int ownerClientId)
        {
            PergRPC.SendMethod("SendBuffers", Targets.ServerOnly, Protocols.TCP, ownerClientId);
        }
        public static void Instantiate(int ownerClientId, string path, int prefabIndex, Vector3 position, Quaternion rotation, int teamId, int sceneId)
        {
            Debug.LogError("Instantiate");
            PergRPC.SendMethod("SpawnObject", Targets.AllBuffered, Protocols.TCP, ownerClientId, GameManager.lastObjectIndex, position, rotation, path, prefabIndex, teamId, sceneId);
            GameManager.lastObjectIndex++;
        }
        public static void InstantiateSceneObject(string path, GameObject gameObject, Vector3 position, Quaternion rotation, int teamId, int sceneId)
        {
            PergRPC.SendMethod("SpawnSceneObject", Targets.AllBuffered, Protocols.TCP, GameManager.lastObjectIndex, path, gameObject, position, rotation, teamId, sceneId);
            GameManager.lastObjectIndex++;
        }
        public static void Destroy(int ownerClientId, int clientId)
        {
            PergRPC.SendMethod("DestroyObject", Targets.AllViaServer, Protocols.TCP, ownerClientId, clientId);
        }
        public static void JoinedToLobby(int pergTeamId, int sceneId)
        {
            PergRPC.SendMethod("WelcomeReceived", Targets.AllBuffered, Protocols.TCP, Client.instance.myId, GameManager.lastObjectIndex, pergTeamId, sceneId); 
            ///Debug.LogError("JoinedToLobby gitti id: " + Client.instance.myId);
            GameManager.lastObjectIndex++;
        }
        public static void SpawnPlayer(int ownerClientId, string path, Vector3 position, Quaternion rotation, int sceneId)
        {
            
            PergRPC.SendMethod("SpawnPlayer", Targets.AllBuffered, Protocols.TCP, ownerClientId, GameManager.players[ownerClientId].clientId, path, position, rotation, sceneId/*new Vector3(-0.42f, -1.14f, -1.94f)*/);
        }
        public static void DisconnectServer(int ownerClientId, string uniqueKey)
        {
            PergRPC.SendMethod("RemoveBuffer", Targets.ServerOnly, Protocols.TCP, uniqueKey); // Before the server is disconnected.
            PergRPC.SendMethod("DisconnectInfo", Targets.ServerOnly, Protocols.TCP, ownerClientId, uniqueKey);
            PergRPC.SendMethod("DisconnectInfo", Targets.AllViaServer, Protocols.TCP, ownerClientId, uniqueKey);
        }
        public static void PlayerName(string playerName)
        {
            Client.instance.playerName = playerName;
        }
        public static bool PermissionToSend(List<WrittenData> v1, int clientId, int packetId)
        {
            bool permission = false;

            if (v1.Count == /*PergSerialized.serializedParameterCount*/ PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount * 2)
            {
                for (int i = 0; i < /*PergSerialized.serializedParameterCount*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount; i++)
                {
                    switch (v1[i].varType)
                    {
                        case VarType._short: //short
                            if (permission) break;
                            short dataShort_1 = PergConverter.WrittenDataToShort(v1[i + /*PergSerialized.serializedParameterCount]*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]);
                            short dataShort_2 = PergConverter.WrittenDataToShort(v1[i]);
                            if (dataShort_1 == dataShort_2)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._int: //int
                            if (permission) break;
                            int dataInt_1 = PergConverter.WrittenDataToInt(v1[i + /*PergSerialized.serializedParameterCount]*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]);
                            int dataInt_2 = PergConverter.WrittenDataToInt(v1[i]);
                            if (dataInt_1 == dataInt_2)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._long: //long
                            if (permission) break;
                            long dataLong_1 = PergConverter.WrittenDataToLong(v1[i + /*PergSerialized.serializedParameterCount]*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]);
                            long dataLong_2 = PergConverter.WrittenDataToLong(v1[i]);
                            if (dataLong_1 == dataLong_2)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._float: //float
                            if (permission) break;
                            float dataFloat_1 = PergConverter.WrittenDataToFloat(v1[i + /*PergSerialized.serializedParameterCount]*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]);
                            float dataFloat_2 = PergConverter.WrittenDataToFloat(v1[i]);
                            if (dataFloat_1 == dataFloat_2)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._string: //string
                            if (permission) break;
                            string dataString_1 = PergConverter.WrittenDataToString(v1[i + /*PergSerialized.serializedParameterCount]*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]);
                            string dataString_2 = PergConverter.WrittenDataToString(v1[i]);
                            if (dataString_1 == dataString_2)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._byte: //byte
                            if (permission) break;
                            byte dataByte_1 = PergConverter.WrittenDataToByte(v1[i + /*PergSerialized.serializedParameterCount]*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]);
                            byte dataByte_2 = PergConverter.WrittenDataToByte(v1[i]);
                            if (dataByte_1 == dataByte_2)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._boolean: //bool
                            if (permission) break;
                            bool dataBool_1 = PergConverter.WrittenDataToBool(v1[i + /*PergSerialized.serializedParameterCount]*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]);
                            bool dataBool_2 = PergConverter.WrittenDataToBool(v1[i]);
                            if (dataBool_1 == dataBool_2)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._Vector3: //Vector3
                            if (permission) break;
                            Vector3 dataVector3_1 = PergConverter.WrittenDataToVector3(v1[i + /*PergSerialized.serializedParameterCount*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]); 
                            Vector3 dataVector3_2 = PergConverter.WrittenDataToVector3(v1[i]);

                            if (dataVector3_1.x == dataVector3_2.x && dataVector3_1.y == dataVector3_2.y && dataVector3_1.z == dataVector3_2.z)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                        case VarType._Quaternion: //Quaternion
                            if (permission) break;
                            Quaternion dataQuaternion_1 = PergConverter.WrittenDataToQuaternion(v1[i + /*PergSerialized.serializedParameterCount*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount]); 
                            Quaternion dataQuaternion_2 = PergConverter.WrittenDataToQuaternion(v1[i]);
                            if (dataQuaternion_1.x == dataQuaternion_2.x && dataQuaternion_1.y == dataQuaternion_2.y && dataQuaternion_1.z == dataQuaternion_2.z && dataQuaternion_1.w == dataQuaternion_2.w)
                            {
                                permission = false;
                            }
                            else
                            {
                                permission = true;
                                break;
                            }
                            break;
                    }
                }

                for (int i = 0; i < /*PergSerialized.serializedParameterCount*/PergSerialized.pergSerializationType[clientId][packetId].serializedParameterCount; i++) 
                {
                    PergSerialized.pergSerializationType[clientId][packetId].currentWrittenData.RemoveAt(0);
                }
            }
            return permission;
        }
    }
}
