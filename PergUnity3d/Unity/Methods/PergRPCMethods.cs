using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class PergRPCMethods
    {
        [PergRPC]
        public void WelcomeReceived(int ownerClientId, int clientId, int pergTeamId, int sceneId)
        {
            //Bağlı olan bütün clientlere bağlantı bilgisi gider.

            if (!GameManager.players.TryGetValue(ownerClientId, out NetworkObject networkObject))
            {
                bool isMine = false;
                if (ownerClientId == Client.instance.myId)
                {
                    isMine = true;
                    Callbacks callbacks = new Callbacks(CallbackMethods.OnJoinedLobby);
                }
                GameManager.players.Add(ownerClientId, new NetworkObject(ObjectType.Player, isMine, false, ownerClientId, clientId, pergTeamId, sceneId));
            }
            else Debug.LogError("This client ID already exists.");
        }
        [PergRPC]
        public void SpawnPlayer(int ownerClientId, int clientId, string path, Vector3 position, Quaternion rotation, int sceneId)
        {
            GameManager.players[ownerClientId].sceneId = sceneId;
            GameManager.lastJoinedPlayer = GameManager.players[ownerClientId];
            GameManager.players[ownerClientId].gameObject = MonoBehaviourPerg.Instantiate(Resources.Load<GameObject>(path), position, rotation); //PergNetwork -> MonoBehaviour 
            Callbacks callbacks = new Callbacks(CallbackMethods.OnPlayerEnteredRoom, ownerClientId, clientId); //Run callbacks. 
        }
        [PergRPC]
        public void DisconnectInfo(int ownerClientId, string uniqueKey)
        {
            if (PergNetwork.serverType == ServerType.CentralServer || PergNetwork.serverType == ServerType.HostClient)
            {
                SetClientSceneId(PergNetwork.GeneratedUniqueClientIdList[uniqueKey], 0);

                if (Server.Server.clients[PergNetwork.GeneratedUniqueClientIdList[uniqueKey]].inRoom) // If the player is in a room
                {
                    PergRooms.LeavePergRoom(PergNetwork.GeneratedUniqueClientIdList[uniqueKey]); // Leave room
                }

                Debug.LogError("Sunucuyla bağlantı kesildi. ClientId: " + PergNetwork.GeneratedUniqueClientIdList[uniqueKey]);
                Callbacks callbacks = new Callbacks(CallbackMethods.OnDisconnectedServer, uniqueKey);

                #region Disconnect olan oyuncuyu sil
                if (PergNetwork.serverType == ServerType.HostClient && Client.instance.uniqueKey != uniqueKey)
                {
                    if (GameManager.players.TryGetValue(ownerClientId, out NetworkObject players) && players.gameObject != null)
                        MonoBehaviourPerg.Destroy(GameManager.players[ownerClientId].gameObject);
                    if (GameManager.objects.TryGetValue(ownerClientId, out Dictionary<int, NetworkObject> objects)) //Oyuncunun spawnladığı objesi varsa
                    {
                        for (int i = 0; i < objects.Count; i++) //Oyuncunun spawnladığı objeleri siler
                        {
                            MonoBehaviourPerg.Destroy(objects.ElementAt(i).Value.gameObject);
                        }
                    }

                    GameManager.objects.Remove(ownerClientId);
                    GameManager.players.Remove(ownerClientId);
                }
                #endregion
            }
            else
            {
                if (uniqueKey == Client.instance.uniqueKey) // Disconnect olan bensem
                {
                    GameManager.lastObjectIndex = 0;
                    GameManager.players.Clear();
                    GameManager.objects.Clear();
                    Client.instance.Disconnect();

                    Callbacks callbacks = new Callbacks(CallbackMethods.OnDisconnectedServer);

                    Client.packetHandlers.Clear();
                    Callbacks.serializationCallbacks.Clear();

                    Callbacks.connectionCallbacks.Clear();
                    Callbacks.lobbyCallbacks.Clear();
                    Callbacks.inRoomCallbacks.Clear();
                    Callbacks.instantiateCallbacks.Clear();
                }
                else // Disconnect olan oyuncuyu sil
                {
                    if (GameManager.players.TryGetValue(ownerClientId, out NetworkObject players) && players.gameObject != null)
                        MonoBehaviourPerg.Destroy(GameManager.players[ownerClientId].gameObject);
                    if (GameManager.objects.TryGetValue(ownerClientId, out Dictionary<int, NetworkObject> objects)) //Oyuncunun spawnladığı objesi varsa
                    {
                        for (int i = 0; i < objects.Count; i++) //Oyuncunun spawnladığı objeleri siler
                        {
                            MonoBehaviourPerg.Destroy(objects.ElementAt(i).Value.gameObject);
                        }
                    }

                    GameManager.objects.Remove(ownerClientId);
                    GameManager.players.Remove(ownerClientId);
                }
            }
        }
        /// <summary>
        /// The path value or prefabIndex value must be filled in. Priority is path value.
        /// </summary>
        /// <param name="ownerClientId"></param>
        /// <param name="clientId"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="path"></param>
        /// <param name="prefabIndex"></param>
        [PergRPC]
        public void SpawnObject(int ownerClientId, int clientId, Vector3 position, Quaternion rotation, string path = "", int prefabIndex = -1, int teamId = 0, int sceneId = 0)
        {
            bool isMine = false;
            if (ownerClientId == Client.instance.myId)
                isMine = true;
            if (!GameManager.objects.TryGetValue(ownerClientId, out Dictionary<int, NetworkObject> networkObjects))
                GameManager.objects.Add(ownerClientId, new Dictionary<int, NetworkObject>());

            GameManager.objects[ownerClientId].Add(clientId, new NetworkObject(ObjectType.GameObject, isMine, false, ownerClientId, clientId, teamId, sceneId));

            GameManager.lastSpawnedObject = GameManager.objects[ownerClientId][/*GameManager.objects[ownerClientId].Count - 1*/clientId];
            //PergNetwork -> MonoBehaviour
            if (prefabIndex == -1)
                GameManager.objects[ownerClientId][/*GameManager.objects[ownerClientId].Count - 1*/clientId].gameObject = MonoBehaviourPerg.Instantiate(Resources.Load<GameObject>(path), position, rotation);

            Callbacks callbacks = new Callbacks(CallbackMethods.OnInstantiated, ownerClientId, clientId, false, prefabIndex, position, rotation, sceneId); //Run callbacks.
        }
        [PergRPC]
        public void SpawnSceneObject(int clientId, string path, GameObject gameObject, Vector3 position, Quaternion rotation, int teamId = 0, int sceneId = 0)
        {
            bool isMine = false;
            if (PergNetwork.isServer)
                isMine = true;
            if (!GameManager.objects.TryGetValue(1, out Dictionary<int, NetworkObject> networkObjects))
                GameManager.objects.Add(1, new Dictionary<int, NetworkObject>());

            GameManager.objects[1].Add(clientId, new NetworkObject(ObjectType.GameObject, isMine, true, 1, clientId, teamId, sceneId));

            GameManager.lastSpawnedObject = GameManager.objects[1][clientId];
            //PergNetwork -> MonoBehaviour
            if (gameObject == null)
                GameManager.objects[1][clientId].gameObject = MonoBehaviourPerg.Instantiate(Resources.Load<GameObject>(path), position, rotation);
            else if (path == "")
                GameManager.objects[1][clientId].gameObject = MonoBehaviourPerg.Instantiate(gameObject, position, rotation);

            Callbacks callbacks = new Callbacks(CallbackMethods.OnInstantiated, 1, clientId, true); //Run callbacks.
        }
        [PergRPC]
        public void DestroyObject(int ownerClientId, int clientId)
        {
            if (GameManager.objects.TryGetValue(ownerClientId, out Dictionary<int, NetworkObject> v) && v.TryGetValue(clientId, out NetworkObject v2))
            {
                GameObject go = GameManager.objects[ownerClientId][clientId].gameObject;
                MonoBehaviourPerg.Destroy(go);

                GameManager.objects[ownerClientId].Remove(clientId);
            }
            else
            {
                Debug.LogError("Silinenler: " + ownerClientId + " | " + clientId);
            }
        }
        [PergRPC]
        public void SendBuffers(int ownerClientId)
        {
            Buffers.SendBuffers(ownerClientId);

        }
        [PergRPC]
        public void RemoveBuffer(string uniqueKey)
        {
            Buffers.PergRPCBuffers.Remove(PergNetwork.GeneratedUniqueClientIdList[uniqueKey]);
        }
        [PergRPC]
        public void SetClientSceneId(int ownerClientId, int sceneId)
        {
            Server.Server.clients[ownerClientId].sceneId = sceneId;
        }
        [PergRPC]
        public void CreatePergRoom(int ownerClientId, int password, bool generateRoomKey)
        {
            PergRooms.CreatePergRoom(ownerClientId, password, generateRoomKey);
        }

        public void LeavePergRoom(int ownerClientId)
        {
            PergRooms.LeavePergRoom(ownerClientId);
        }
        [PergRPC]
        public static void SerializedPacketInfos(int count)
        {
            PergUnity3d.Server.Server.packetHandlers.Clear();
            PergSerialized.AddServerPacketHandler((int)AlreadyClientPackets.pergRPC, PergUnity3d.Server.ServerHandle.PergRPC);

            for (int i = 0; i < count; i++)
            {
                PergSerialized.AddServerPacketHandler(i + 2, Callbacks.serializeData); //Değişken değer
            }
        }
        [PergRPC]
        public static void GetPlayerCountInRoom(int ownerClientId, string roomKey)
        {
            PergRPC.clientIdList.Add(ownerClientId);
            PergRPC.SendMethod("GetPlayerCount", Targets.SpecificClientsForServer, Protocols.TCP, PergRooms.GetPlayerCount(roomKey));
        }
        [PergRPC]
        public static void GetPlayerCount(int playerCount)
        {
            PergRooms.playerCount = playerCount;
        }
    }
}