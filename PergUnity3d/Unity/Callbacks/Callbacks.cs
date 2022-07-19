using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class Callbacks
    {
        public static PergUnity3d.Server.Server.PacketHandler serializeData; // Serialized Server Handle

        public static SortedDictionary<int, Dictionary<int, List<PergSerializedClass>>> serializationCallbacks = new SortedDictionary<int, Dictionary<int, List<PergSerializedClass>>>();
        public static List<IConnectionCallbacks> connectionCallbacks = new List<IConnectionCallbacks>();
        public static List<ILobbyCallbacks> lobbyCallbacks = new List<ILobbyCallbacks>();
        public static List<IInRoomCallbacks> inRoomCallbacks = new List<IInRoomCallbacks>();
        public static List<IInstantiateCallbacks> instantiateCallbacks = new List<IInstantiateCallbacks>();

        public Callbacks(CallbackMethods callbackMethods, int ownerClientId = -1, int clientId = -1, bool sceneObject = false, int prefabIndex = -1, Vector3 position = new Vector3(), Quaternion rotation= new Quaternion(), int sceneId = 0)
        {
            switch (callbackMethods)
            {
                case CallbackMethods.OnConnectedServer:
                    foreach (IConnectionCallbacks connectionCallbacks in connectionCallbacks)
                    {
                        connectionCallbacks.OnConnectedServer();
                    }
                    break;
                case CallbackMethods.OnDisconnectedServer:
                    foreach (IConnectionCallbacks connectionCallbacks in connectionCallbacks)
                    {
                        connectionCallbacks.OnDisconnectedServer();
                    }
                    break;
                case CallbackMethods.OnJoinedLobby:
                    foreach (ILobbyCallbacks lobbyCallbacks in lobbyCallbacks)
                    {
                        lobbyCallbacks.OnJoinedLobby();
                    }
                    break;
                case CallbackMethods.OnPlayerEnteredRoom:
                    foreach (IInRoomCallbacks inRoomCallbacks in inRoomCallbacks)
                    {
                        inRoomCallbacks.OnPlayerEnteredRoom(ownerClientId, clientId);
                    }
                    break;
                case CallbackMethods.OnInstantiated:
                    foreach (IInstantiateCallbacks instantiateCallbacks in instantiateCallbacks)
                    {
                        instantiateCallbacks.OnInstantiated(ownerClientId, clientId, sceneObject, prefabIndex, position, rotation, sceneId);
                    }
                    break;
            }
        }
        public Callbacks(CallbackMethods callbackMethods, string uniqueKey)
        {
            switch (callbackMethods)
            {
                case CallbackMethods.OnDisconnectedServer:
                    foreach (IConnectionCallbacks connectionCallbacks in connectionCallbacks)
                    {
                        connectionCallbacks.OnDisconnectedServer(uniqueKey);
                    }
                    break;
            }
        }

        public static void AddCallbacks(CallbackTypes callbackType, object value)
        {
            switch (callbackType)
            {
                case CallbackTypes.PergSerialized:
                    PergSerializedClass pergSerializedClass = (PergSerializedClass)value;

                    if (!serializationCallbacks.TryGetValue(pergSerializedClass.ownerClientId, out Dictionary<int, List<PergSerializedClass>> p))
                    {
                        serializationCallbacks.Add(pergSerializedClass.ownerClientId, new Dictionary<int, List<PergSerializedClass>>());
                        serializationCallbacks[pergSerializedClass.ownerClientId].Add(pergSerializedClass.clientId, new List<PergSerializedClass>());
                        serializationCallbacks[pergSerializedClass.ownerClientId][pergSerializedClass.clientId].Add(pergSerializedClass);
                    }
                    else
                    {
                        if(!serializationCallbacks[pergSerializedClass.ownerClientId].TryGetValue(pergSerializedClass.clientId, out List<PergSerializedClass> pergSerializedList))
                        {
                            serializationCallbacks[pergSerializedClass.ownerClientId].Add(pergSerializedClass.clientId, new List<PergSerializedClass>());
                            serializationCallbacks[pergSerializedClass.ownerClientId][pergSerializedClass.clientId].Add(pergSerializedClass);
                        }
                        else
                        {
                            serializationCallbacks[pergSerializedClass.ownerClientId][pergSerializedClass.clientId].Add(pergSerializedClass);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callbackType"></param>
        /// <param name="objectType">0 -> None, 1 -> Player, 2 -> GameObject</param>
        /// <param name="ownerClientId"></param>
        /// <param name="clientId"></param>
        public static void RemoveCallbacks(CallbackTypes callbackType, int objectType, int ownerClientId, int clientId = -1) //@@@@@@@@@@@@6109806
        {
            if (objectType == 1)
            {
                switch (callbackType)
                {
                    case CallbackTypes.PergSerialized:
                        if (serializationCallbacks.TryGetValue(ownerClientId, out Dictionary<int, List<PergSerializedClass>> p))
                        {
                            serializationCallbacks.Remove(ownerClientId);
                        }
                        break;
                }
            }
            else if (objectType == 2)
            {
                switch (callbackType)
                {
                    case CallbackTypes.PergSerialized:
                        if (serializationCallbacks.TryGetValue(ownerClientId, out Dictionary<int, List<PergSerializedClass>> p1) && p1.TryGetValue(clientId, out List<PergSerializedClass> p2))
                        {
                            p1.Remove(clientId);
                        }
                        break;
                }
            }
        }
        private static int GetSerializationCallbacksCount()
        {
            int count = 0;

            return count;
        }
        public static List<PergSerializedClass> GetSerializedCallbacks()
        {
            List<PergSerializedClass> pergSerializedClasses = new List<PergSerializedClass>();

            for (int i = 0; i < serializationCallbacks.Count; i++)
            {
                for (int k = 0; k < serializationCallbacks.ElementAt(i).Value.Count; k++)
                {
                    foreach (PergSerializedClass pergSerializedClass in serializationCallbacks.ElementAt(i).Value.ElementAt(k).Value)
                    {
                        pergSerializedClasses.Add(pergSerializedClass);
                    }
                }
            }

            return pergSerializedClasses;
        }
    }
}
