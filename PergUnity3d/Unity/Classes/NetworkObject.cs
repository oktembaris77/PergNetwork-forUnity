using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class NetworkObject
    {
        public UnityEngine.GameObject gameObject = null;
        public ObjectType objectType;
        public bool isMine = false; // Is it mine?
        public bool isSceneObject = false; // Is it scene object?
        public int ownerClientId = 0; // The ID of the client that owns the object.
        public int clientId = 0; // Unique ID in the room.
        public int teamId = 0; // Team ID
        public int sceneId = 0; // Scene ID
        public string ownerPlayerName = null; // Owner Player Name

        public NetworkObject()
        {
           
        }
        public NetworkObject(ObjectType objectType, bool isMine, bool isSceneObject, int ownerClientId, int clientId, int teamId, int sceneId)
        {
            this.objectType = objectType;
            this.isMine = isMine;
            this.isSceneObject = isSceneObject;
            this.ownerClientId = ownerClientId;
            this.clientId = clientId;
            this.teamId = teamId;
        }
    }
}
