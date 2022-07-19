using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace PergUnity3d
{
    public class Client
    {
        public static Client instance;
        public static int dataBufferSize = 4096;
        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public int myId = 0;
        public string uniqueKey = "";

        public TCP tcp;
        public UDP udp;

        public bool isConnected = false;
        public string playerName = "Player";

        public delegate void PacketHandler(Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler>();
        //public static int PacketHandlersCounts = 0;

        public Client(string _ipAddress, int _port)
        {
            instance = this;
            IpAddress = _ipAddress;
            Port = _port;

            tcp = new TCP();
            udp = new UDP();

            ConnectToServer();
        }

        public void Loop()
        {

        }

        public void ConnectToServer()
        {
            InitializeClientData();

            isConnected = true;
            tcp.Connect();
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(instance.IpAddress, instance.Port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to server via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)            
                    {
                        instance.Disconnect();
                        return;
                    }
                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength); 
                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch
                {
                    Disconnect();
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)                            
                {
                    _packetLength = receivedData.ReadInt();
                                                                                                                   
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())                                                                    
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            packetHandlers[_packetId](_packet);
                        }
                    });

                    _packetLength = 0;
                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(instance.IpAddress), instance.Port);
            }

            public void Connect(int _localPort)
            {
                socket = new UdpClient(_localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (Packet _packet = new Packet())
                {
                    SendData(_packet);
                }
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    _packet.InsertInt(0, instance.myId);
                    if (socket != null)
                    {
                        socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to server via UDP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    byte[] _data = socket.EndReceive(_result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (_data.Length < 4)
                    {
                        instance.Disconnect();
                        return;
                    }

                    HandleData(_data);
                }
                catch
                {
                    Disconnect();
                }
            }

            private void HandleData(byte[] _data)
            {

                using (Packet _packet = new Packet(_data))
                {
                    int _packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(_packetLength);
                }
              
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_data))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet);
                    }
                });
            }

            public void Disconnect()
            {
                socket.Close();
                endPoint = null;
                socket = null;
            }
        }

        private void InitializeClientData()
        {
            packetHandlers.Add((int)AlreadyServerPackets.welcome, ClientHandle.Welcome);

            Console.WriteLine("Initialized packets.");
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                //ClientSend.Disconnected();
                //tcp.socket.Close();
                //udp.socket.Close();
                tcp.Disconnect();
                udp.Disconnect();
                Console.WriteLine("Disconnected from server.");
            }
        }

    }


}
