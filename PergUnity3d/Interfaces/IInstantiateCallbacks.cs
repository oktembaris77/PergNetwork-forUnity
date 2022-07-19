using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public interface IInstantiateCallbacks
    {
        void OnInstantiated(int ownerClientId, int clientId, bool sceneObject, int prefabIndex, Vector3 position, Quaternion rotation, int sceneId);
    }
}
