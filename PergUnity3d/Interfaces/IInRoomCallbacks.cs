using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public interface IInRoomCallbacks
    {
        void OnPlayerEnteredRoom(int ownerClientId, int clientId);
        void OnPlayerLeftRoom(int clientId);
    }
}
