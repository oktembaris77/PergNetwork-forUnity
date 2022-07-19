using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class MonoBehaviourPergCallbacks : MonoBehaviourPerg, IConnectionCallbacks, IInRoomCallbacks, ILobbyCallbacks, IInstantiateCallbacks
    {
        public virtual void OnConnectedServer()
        {
        }
        public virtual void OnDisconnectedServer()
        {
        }
        public virtual void OnDisconnectedServer(string uniqueKey)
        {
        }
        public virtual void OnJoinedLobby()
        {
        }

        public virtual void OnLeftLobby()
        {
        }

        public virtual void OnPlayerEnteredRoom(int ownerClientId, int clientId)
        {
        }

        public virtual void OnPlayerLeftRoom(int clientId)
        {
        }

        public virtual void OnInstantiated(int ownerClientId, int clientId, bool sceneObject, int prefabIndex, Vector3 position, Quaternion rotation, int sceneId)
        { 
        }
    }
}