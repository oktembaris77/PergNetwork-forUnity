using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace PergUnity3d.Server
{
    public class ServerHandle
    {
        public static void PergRPC(int _fromClient, Packet _packet)
        {
            int methodId = _packet.ReadInt();
            int target = _packet.ReadInt();
            int protocol = _packet.ReadInt();

            int clientIdListCount = 0;
            if (target == (int)Targets.SpecificClients)
            {
                clientIdListCount = _packet.ReadInt();
                for (int i = 0; i < clientIdListCount; i++)
                {
                    PergUnity3d.PergRPC.clientIdList.Add(_packet.ReadInt());
                }
            }

            ParameterInfo[] parameterInfos = (ParameterInfo[])PergUnity3d.PergRPC.GetPergRPCParametersList(PergUnity3d.PergRPC.GetPergRPCName(methodId)); //Parametre tipleri belirlenir
            int parametersCount = parameterInfos.Length;
            object[] parameters = new object[parametersCount + 1];
            parameters[0] = methodId;
            for (int i = 1; i <= parametersCount; i++)
            {
                parameters[i] = _packet.ReadObject(parameterInfos[i - 1].ParameterType.Name);
            }

            // Sadece server için
            if (target == (int)Targets.ServerOnly)
            {
                object[] parametersServer = new object[parametersCount];
                for (int i = 1; i <= parametersCount; i++)
                {
                    parametersServer[i - 1] = parameters[i];
                }

                PergUnity3d.PergRPC.MethodInvoke(PergUnity3d.PergRPC.GetPergRPCName(methodId), parametersServer);
            }
            // Cliente-lere gönderilecek
            else
            {
                //PergPRC den gelen veriler şuan serverda. Merkezi serverda işlem yapılacak olursa bu veriler kullanılacak.

                Debug.Log("Client Sayısı: " + Server.clients.Count);
                ServerSend.PergRPC(_fromClient, parameterInfos, parameters, (Targets)target, (Protocols)protocol, false, true);
            }
        }
    }
}
