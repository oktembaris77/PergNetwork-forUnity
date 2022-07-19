using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class PergSerializedClass
    {
        public ObjectType objectType;
        public int ownerClientId;
        public int clientId;
        public int packetId;
        public bool isMine;
        public bool sceneObject;
        public IPergSerialized serializationCallbacks;
        public PergSerializedClass(ObjectType objectType, int ownerClientId, int clientId, int packetId, bool isMine, bool sceneObject, IPergSerialized serializationCallbacks)
        {
            this.objectType = objectType;
            this.ownerClientId = ownerClientId;
            this.clientId = clientId;
            this.packetId = packetId;
            this.isMine = isMine;
            this.sceneObject = sceneObject;
            this.serializationCallbacks = serializationCallbacks;
        }
    }
}