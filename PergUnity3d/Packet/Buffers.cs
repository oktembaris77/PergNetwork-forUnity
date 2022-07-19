using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class Buffers
    {
        /// <summary>
        /// Key, benzersiz bir id olmalı. Key->int, Value->RPC Method Id Listesi
        /// Key silinince, ilgili RPC Method Listesi de silinir ve artık kullanılmaz
        /// </summary>
        public static Dictionary<int, List<RPCBuffer>> PergRPCBuffers = new Dictionary<int, List<RPCBuffer>>();
        internal static int interviewRPCBufferKey = 0;

        public static void SendBuffers(int clientId)
        {
            for (int i = 0; i < PergRPCBuffers.Count; i++)
            {
                foreach(RPCBuffer rpcBuffer in PergRPCBuffers.ElementAt(i).Value)
                {
                    ParameterInfo[] parameterInfos = (ParameterInfo[])PergUnity3d.PergRPC.GetPergRPCParametersList(PergUnity3d.PergRPC.GetPergRPCName((int)rpcBuffer.parameters[0])); //Parametre tipleri belirlenir

                    foreach(int id in rpcBuffer.clientSceneIdList)
                    {
                        PergRPC.clientSceneIdList.Add(id);
                    }

                    //PergRPCBuffers keyini bu methodda parametre olarak vermemiz gerekiyor.
                    interviewRPCBufferKey = PergRPCBuffers.ElementAt(i).Key;

                    PergUnity3d.Server.ServerSend.PergRPC(clientId, parameterInfos, rpcBuffer.parameters, Targets.OnlyMe, rpcBuffer.protocols, true);
                }
            }
        }
        public static void SetBuffers(int clientId, object[] parameters, Protocols protocols, int[] clientSceneIdList)
        {
            List<RPCBuffer> buffers;
            if (!Buffers.PergRPCBuffers.TryGetValue(clientId, out buffers))
            {
                Buffers.PergRPCBuffers.Add(clientId, new List<RPCBuffer>());
                Buffers.PergRPCBuffers[clientId].Add(new RPCBuffer(parameters, protocols, clientSceneIdList));
            }
            else
            {
                Buffers.PergRPCBuffers[clientId].Add(new RPCBuffer(parameters, protocols, clientSceneIdList));
            }
        }
    }
}
