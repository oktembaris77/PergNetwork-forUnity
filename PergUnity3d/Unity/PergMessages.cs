using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class PergMessages
    {
        public static string message = null;
        public static void RPCSpecificClientlistWarnings(string methodName, Targets targets)
        {
            if(targets == Targets.SpecificClients && PergRPC.clientIdList.Count == 0)
            {
                message = "Editor Message | " + methodName + ": Method will not be sent to any client. You must specify the client IDs for which data will be sent. -> PergRPC.clientIdList.Add(int clientId) before PergRPC.SendMethod(string methodName, Targets target, Protocols protocol, params object[] parameters)"; //Hiç bir istemciye veri gitmeyecek. Verileri gönderilecek istemci kimliklerini belirtmelisiniz.
            }
        }

        public static void SerializedPacketClientListWarnings(PacketStream packetStream, SerializedTargets serializedTargets)
        {
            if(serializedTargets == SerializedTargets.SpecificClients && packetStream.clientIdList.Count == 0)
            {
                message = "Editor Message | " + "Serialized packet will not be sent to any client. You must specify the client IDs for which data will be sent. -> packetStream.clientIdList.Add(int clientId) before packetStream.ClientSendData();"; //Hiç bir istemciye veri gitmeyecek. Verileri gönderilecek istemci kimliklerini belirtmelisiniz.
            }
        }

    }
}
