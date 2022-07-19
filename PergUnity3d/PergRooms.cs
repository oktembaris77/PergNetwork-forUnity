using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

namespace PergUnity3d
{
    public class PergRooms
    {
        /*public struct PergRoomHost
        {
            public string ipAddress;
            public int port;
            public string password;

            public PergRoomHost(string ipAddress, int port, string password)
            {
                this.ipAddress = ipAddress;
                this.port = port;
                this.password = password;
            }
        }
        public struct PergRoomServer
        {
            public string password;

            public PergRoomServer(string password)
            {
                this.password = password;
            }
        }*/
        public struct RoomKey
        {
            public int roomId;
            public string key;
            public int password;
            public List<int> ownerClientIdList;

            public RoomKey(int roomId, string key, int password, List<int> ownerClientIdList)
            {
                this.roomId = roomId;
                this.key = key;
                this.password = password;
                this.ownerClientIdList = ownerClientIdList;
            }
        }
        internal static Dictionary<string, RoomKey> PergRoomList = new Dictionary<string, RoomKey>();
        internal static int lastPergRoomId = 0;
        internal static int playerCount = 0;
        /// <summary>
        /// Creates a room.
        /// </summary>
        /// <param name="password">Room password</param>
        /// <param name="generateRoomKey">Generate unique room key using room id ?</param>
        /// <returns>Returns the key of the created room.</returns>
        internal static string CreatePergRoom(int ownerClientId, int password, bool generateRoomKey = false)
        {
            int roomId = ++lastPergRoomId;
            string key = "-1";
            string dicKey = "";

            if (generateRoomKey)
            {
                key = GeneratePergRoomKeyWithRoomId(roomId);
                PergRPC.clientIdList.Add(ownerClientId);
                PergRPC.SendMethod("GetRoomKey", Targets.SpecificClientsForServer, Protocols.TCP, key);
            }
            else key = "0";

            PergRoomList.Add(key, new RoomKey(roomId, key, password, new List<int>()));

            Debug.Log("Oda oluşturuldu.");

            if(ownerClientId > 0)
            {
                PergRoomList[key].ownerClientIdList.Add(ownerClientId);
                Server.Server.clients[ownerClientId].inRoom = true;
                Server.Server.clients[ownerClientId].roomKey = key;
            }

            return key;
        }
        public static bool JoinPergRoom(int ownerClientId, int password, string roomKey = "-1")
        {
            Debug.Log("Room password: " + password + " roomKey:" + roomKey);
            bool check = false;

            if (PergRoomList.TryGetValue(roomKey, out RoomKey rk))
            {
                if (roomKey != "-1" && rk.key == roomKey && rk.password == password && rk.ownerClientIdList.Count < 6 && !Server.Server.clients[ownerClientId].inRoom)
                {
                    Debug.Log("Odaya giriş yapıldı.");
                    rk.ownerClientIdList.Add(ownerClientId);
                    Server.Server.clients[ownerClientId].inRoom = true;
                    Server.Server.clients[ownerClientId].roomKey = roomKey;
                    check = true;
                }
            }
            return check;
        }
        public static void LeavePergRoom(int ownerClientId)
        {
            PergRoomList.TryGetValue(Server.Server.clients[ownerClientId].roomKey, out RoomKey rk);

            //If the player leaving the room is the master of the room, transfer the mastery.
            if (rk.ownerClientIdList.Count > 1 && GetThisPergRoomList(Server.Server.clients[ownerClientId].roomKey).ownerClientIdList[0] == ownerClientId) //Room Master
            {
                PergRPC.clientIdList.Add(GetThisPergRoomList(Server.Server.clients[ownerClientId].roomKey).ownerClientIdList[1]);
                PergRPC.SendMethod("NewRoomMaster", Targets.SpecificClientsForServer, Protocols.TCP);
            }

            rk.ownerClientIdList.Remove(ownerClientId);

            if (rk.ownerClientIdList.Count == 0)
            {
                GameManager.mailAndClientId.Remove(rk.roomId);
                RemovePergRoom(Server.Server.clients[ownerClientId].roomKey);
            }
            else
            {
                //Send information to the players in the room to leave the room.
                for (int i = 0; i < PergRooms.GetPlayerCount(Server.Server.clients[ownerClientId].roomKey); i++)
                {
                    PergRPC.clientIdList.Add(PergRooms.GetThisPergRoomList(Server.Server.clients[ownerClientId].roomKey).ownerClientIdList[i]);
                    PergRPC.SendMethod("UpdatePlayerCount", Targets.SpecificClientsForServer, Protocols.TCP, PergRooms.GetPlayerCount(Server.Server.clients[ownerClientId].roomKey));
                }
            }

            Server.Server.clients[ownerClientId].inRoom = false;
            Server.Server.clients[ownerClientId].roomKey = "-1";
        }
        internal static void RemovePergRoom(string roomKey)
        {
            PergRoomList.Remove(roomKey);
        }
        public static int GetPlayerCount(string roomKey)
        {
            return PergRoomList[roomKey].ownerClientIdList.Count;
        }
        /// <summary>
        /// Generates a unique room key using the roomId.
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static string GeneratePergRoomKeyWithRoomId(int roomId = 0)
        {
            string key = "";
            System.Random rand = new System.Random();
            int rnd = -1;

            while (true)
            {
                for (int i = 0; i < 6; i++)
                {
                    int rndSelect = rand.Next(0, 2);
                    if (rndSelect == 0) //Number
                    {
                        key += rand.Next(0, 10).ToString();
                    }
                    else //Latter
                    {
                        rnd = rand.Next(65, 91);
                        char chr = (char)rnd;
                        key += chr.ToString();
                    }
                }
                //Is this room key unique?
                if (!PergRoomList.TryGetValue(key, out RoomKey v))
                {
                    break;
                }
                else key = "";
            }

            return key;
        }
        public static void CreateRoom(int ownerClientId, int password, bool generateRoomKey)
        {
            PergRPC.SendMethod("CreatePergRoom", Targets.ServerOnly, Protocols.TCP, ownerClientId, password, generateRoomKey);
        }
        public static void LeaveRoom(int ownerClientId)
        {
            PergRPC.SendMethod("LeavePergRoom", Targets.ServerOnly, Protocols.TCP, ownerClientId);
        }
        public static RoomKey GetThisPergRoomList(string roomKey)
        {
            return PergRoomList[roomKey];
        }
        public static bool IsRoomMaster(int ownerClientId, string roomKey)
        {
            if (PergRoomList[roomKey].ownerClientIdList[0] == ownerClientId) return true;
            else return false;
        }
        public static void GetPlayerCountInRoom(int ownerClientId, string roomKey)
        {
            PergRPC.SendMethod("GetPlayerCountInRoom", Targets.ServerOnly, Protocols.TCP, ownerClientId, roomKey);
        }
    }
}