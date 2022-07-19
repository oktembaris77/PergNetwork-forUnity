using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class RPCBuffer
    {
        public object[] parameters { get; set; }
        public Protocols protocols { get; set; }
        public int[] clientSceneIdList { get; set; }

        public RPCBuffer(object[] parameters, Protocols protocols, int[] clientSceneIdList)
        {
            this.parameters = parameters;
            this.protocols = protocols;
            this.clientSceneIdList = clientSceneIdList;
        }
    }
}
