using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace PergUnity3d.Server
{
    public class Server
    {
        public static int MaxPlayers { get; private set; }
        public static string IpAdress { get; private set; }

        public static int Port { get; private set; }
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler>();
        public static int PacketHandlersCount = 0;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static string Start(int _maxPlayers, string _ipAddress, int _port)
        {
            MaxPlayers = _maxPlayers;
            IpAdress = _ipAddress;
            Port = _port;

            Console.WriteLine("Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Parse(IpAdress), Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            //Room
            PergRooms.CreatePergRoom(0, 0, false);

            return "Server started on port " + Port + ", " + IpAdress;

            Console.WriteLine($"Server started on port {Port}, IP: {IPAddress.Parse(IpAdress)}.");
        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
            tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    //Bağlanacak client, veritabanına kaydedilir ve player için benzersiz id si yazılır.
                    //Client bağlantısı yapılır
                    clients[i].tcp.Connect(_client);

                    string uniqueKey = GetUniqueKey(i);
                    PergNetwork.connectedClientBufferList.Add(new ConnectedClientBufferClass(-1, i, uniqueKey));

                    PergRPC.clientIdList.Add(i);
                    PergRPC.SendMethod("GetMyUniqueKey", Targets.SpecificClientsForServer, Protocols.TCP, uniqueKey);

                    Buffers.SendBuffers(i);
                    return;
                }
            }

            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientId = _packet.ReadInt();

                    if (_clientId == 0)
                    {
                        return;
                    }

                    if (clients[_clientId].udp.endPoint == null)
                    {
                        clients[_clientId].udp.Connect(_clientEndPoint);
                        return;
                    }

                    if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientId].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error receiving UDP data: {_ex}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception _ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
            }
        }

        public static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i, 0));
            }

            packetHandlers.Add((int)AlreadyClientPackets.pergRPC, ServerHandle.PergRPC);
            PacketHandlersCount = packetHandlers.Count;
        }

        public static void Stop()
        {
            tcpListener.Stop();
            udpListener.Close();
        }

        public static string GetUniqueKey(int ownerClientId)
        {
            string key = "";
            System.Random rand = new System.Random();
            int rnd = -1;

            while (true)
            {
                for (int i = 0; i < 6; i++)
                {
                    int rndSelect = rand.Next(0, 2);
                    if (rndSelect == 0) //Number
                    {
                        key += rand.Next(0, 10).ToString();
                    }
                    else //Latter
                    {
                        rnd = rand.Next(65, 91);
                        char chr = (char)rnd;
                        key += chr.ToString();
                    }
                }
                //Is this room key unique?
                if (!PergNetwork.GeneratedUniqueClientIdList.TryGetValue(key, out int oci))
                {
                    PergNetwork.GeneratedUniqueClientIdList.Add(key, ownerClientId);
                    break;
                }
                else key = "";
            }

            return key;
        }
    }
}