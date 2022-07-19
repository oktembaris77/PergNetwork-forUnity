using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public interface ILobbyCallbacks
    {
        void OnJoinedLobby();
        void OnLeftLobby();
    }
}