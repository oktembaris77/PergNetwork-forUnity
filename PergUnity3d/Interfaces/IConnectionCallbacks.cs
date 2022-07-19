using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public interface IConnectionCallbacks
    {
        void OnConnectedServer();
        void OnDisconnectedServer();
        void OnDisconnectedServer(string uniqueKey);
    }
}
