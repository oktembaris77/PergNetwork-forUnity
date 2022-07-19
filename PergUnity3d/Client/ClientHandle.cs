using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class ClientHandle
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myid = _packet.ReadInt();

            Client.instance.myId = _myid;

            Callbacks callbacks = new Callbacks(CallbackMethods.OnConnectedServer); //Run callbacks.

            Client.instance.isConnected = true;
            //PergNetwork.JoinedToLobby(); //Callback te çağrılır

            //PergRPC.SendMethod("WelcomeReceived", Targets.AllBuffered, Protocols.TCP, Client.instance.myId);
            //ClientSend.WelcomeReceived();

            Debug.LogError(_msg);

            Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
            

        }
    }
}
