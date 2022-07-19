using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using PergUnity3d;
using UnityEngine;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace PergUnity3d {
    public class PergRPC
    {
        public static PergRPC instance;
        public static object obj;
        public static List<int> clientIdList = new List<int>();

        public static List<int> clientSceneIdList = new List<int>();

        public static List<MethodInfo> MethodInfoList = new List<MethodInfo>();

        internal static Dictionary<string, Type> MethodRPCObject = new Dictionary<string, Type>();

        public PergRPC()
        {

        }
        public PergRPC(object obj)
        {

        }

        /// <summary>
        /// Unity'deki PergRPCAttribute niteliğine sahip bütün methodların bilgilerini alır.
        /// </summary>
        /// <returns></returns>
        public static List<MethodInfo> GetMethodInfos()
        {
            List<MethodInfo> MethodInfoList = new List<MethodInfo>();
            MethodRPCObject.Clear();
            //Game Engine
            MethodInfo[] methodInfos = obj.GetType().Assembly.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(PergRPCAttribute), false).Length > 0)
                      .ToArray();

            //Perg
            MethodInfo[] pergMethodInfos = new PergRPCMethods().GetType().Assembly.GetTypes()
                     .SelectMany(t => t.GetMethods())
                     .Where(m => m.GetCustomAttributes(typeof(PergRPCAttribute), false).Length > 0)
                     .ToArray();

            //methodInfo.DeclaringType
            foreach (MethodInfo methodInfo in methodInfos)
            {
                MethodInfoList.Add(methodInfo);
                MethodRPCObject.Add(methodInfo.Name, methodInfo.DeclaringType);
            }
            foreach (MethodInfo methodInfo in pergMethodInfos)
            {
                MethodInfoList.Add(methodInfo);
                MethodRPCObject.Add(methodInfo.Name, methodInfo.DeclaringType);
            }

            return MethodInfoList;
        }

        /// <summary>
        /// Unity'deki PergRPCAttribute niteliğine sahip bütün methodların adlarını alır.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetPergRPCList()
        {
            //List<MethodInfo> methodInfos = GetMethodInfos(); 6109806
            List<string> PergRPCMethodNameList = new List<string>();

            foreach (MethodInfo methodInfo in MethodInfoList)
            {
                string methodName = methodInfo.Name;
                PergRPCMethodNameList.Add(methodName);
            }

            return PergRPCMethodNameList;
        }

        public static int GetPergRPCId(string methodName)
        {
            int methodId = -1;

            //List<MethodInfo> methodInfos = GetMethodInfos(); 6109806

            for (int i = 0; i < MethodInfoList.Count; i++)
            {
                if (MethodInfoList[i].Name == methodName)
                {
                    methodId = i;
                }
            }

            return methodId;
        }

        public static string GetPergRPCName(int methodId)
        {
            string methodName = "";

            //List<MethodInfo> methodInfos = GetMethodInfos(); 6109806

            for (int i = 0; i < MethodInfoList.Count; i++)
            {
                if (i == methodId)
                {
                    methodName = MethodInfoList[i].Name;
                }
            }

            return methodName;
        }

        /// <summary>
        /// TCP protokolü kullanılması önerilir.
        /// PergRPC methodunun id değeri ve methodun parametreleri sırayla pakete yazılır ve gönderilir.
        /// Sıkça gönderilmesi gerekmeyen durumlarda kullanılabilir.
        /// Seri veri paketleri göndermek istiyorsanız SerializedDataPacket metodunu kullanın.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public static void SendMethod(string methodName, Targets targets, Protocols protocols, params object[] parameters)
        {
            bool centralServer = false;
            bool hostClient = false;
            int RPCMethodId = GetPergRPCId(methodName);

            //Eğer server tipi Cooparative ise
            if (PergNetwork.serverType == ServerType.HostClient)
            {
                hostClient = true;
                //Target herkes ise
                if (targets == Targets.All || targets == Targets.AllBuffered)
                {
                    //İlk indeksinde RPC method id olan bir object[] parametre dizisi oluştur
                    object[] _parameters = GetPreparedParameterList(parameters,RPCMethodId);

                    //Target AllBuffered ise Buffer'ı kaydet. Burası server-client olduğu direk burda da kaydedebiliriz.
                    if (targets == Targets.AllBuffered)
                    {
                        //Buffer'ı kaydet
                        Buffers.SetBuffers(Client.instance.myId, _parameters, protocols, clientSceneIdList.ToArray());
                        //Kaydettikten sonra, ayrıca ServerSend'de de kaydedilmemesi için targeti AllBuffered'dan All a dönüştür
                        targets = Targets.All;
                    }

                    //Methodu bu client için yerelde hemen çalıştır
                    MethodInvoke(methodName, parameters);
                }
                //Target Specific client ise
                else if(targets == Targets.SpecificClients)
                {
                    //Spesific client listte kendim varsam, onu da yerelde çalıştır ve listeden sil
                    foreach (int id in clientIdList) //PergRPC.
                    {
                        //Client id benim ise
                        if(id == Client.instance.myId)
                        {
                            //Bu clienti listeden sil
                            clientIdList.Remove(id); //PergRPC.
                            //Yerelde çalıştır
                            MethodInvoke(methodName, parameters);
                            if (clientIdList.Count == 0) return;
                        }
                    }
                }
            }
            //Server tipi none ise (sadece client ise)
            else if(PergNetwork.serverType == ServerType.none)
            {
                //Eğer AllViaServer veya AllBufferedViaServer ise serverdan zaten RPC gelecek. Yerelde yollanmayacak.
                //Target herkes ise
                if (targets == Targets.All || targets == Targets.AllBuffered)
                    MethodInvoke(methodName, parameters); //Yerelde çalıştır
            }
            else if(PergNetwork.serverType == ServerType.CentralServer)
            {
                centralServer = true;
            }

            ParameterInfo[] parameterInfos = (ParameterInfo[])GetPergRPCParametersList(methodName);

            if (!centralServer && targets != Targets.SpecificClientsForServer || hostClient && targets != Targets.SpecificClientsForServer)
            {
                //int _fromClient, ParameterInfo[] parameterInfos, object[] parameters, Targets targets, Protocols protocols
                using (Packet _packet = new Packet((int)AlreadyClientPackets.pergRPC))
                {
                    _packet.Write(RPCMethodId);
                    _packet.Write((int)targets);
                    _packet.Write((int)protocols);
                    if (targets == Targets.SpecificClients)
                    {
                        _packet.Write(clientIdList.Count);
                        foreach (int id in clientIdList)
                        {
                            _packet.Write(id);
                        }

                        //Warning Messages
                        PergMessages.RPCSpecificClientlistWarnings(methodName, targets);

                        //Clear List
                        clientIdList.Clear();
                    }

                    //Parametre tipine göre, packet verilerini oku
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        _packet.WriteObject(parameters[i], parameterInfos[i].ParameterType.Name);
                    }

                    _packet.WriteLength();

                    if (protocols == Protocols.UDP) Client.instance.udp.SendData(_packet);
                    else Client.instance.tcp.SendData(_packet);
                }
            }
            else if(centralServer || hostClient)
            {
                if(targets == Targets.SpecificClientsForServer || targets == Targets.OthersBuffered || targets == Targets.Others)
                {
                    object[] _parameters = GetPreparedParameterList(parameters, RPCMethodId);
                    Server.ServerSend.PergRPC(-1, parameterInfos, _parameters, targets, protocols);
                }
            }
        }

        public static object[] GetPergRPCParametersList(string methodName)
        {
            //List<MethodInfo> methodInfos = GetMethodInfos(); 6109806
            object[] parameters = null;

            foreach (MethodInfo methodInfo in MethodInfoList)
            {
                if (methodInfo.Name == methodName)
                {
                    parameters = methodInfo.GetParameters();
                }
            }

            return parameters;
        }
        public static object[] GetPergRPCParametersList(int methodId)
        {
            //List<MethodInfo> methodInfos = GetMethodInfos(); 6109806
            object[] parameters = null;

            for(int i =0; i< MethodInfoList.Count;i++)
            {
                if(i == methodId)
                {
                    parameters = MethodInfoList[i].GetParameters();
                }
            }

            return parameters;
        }
        private static object[] GetPreparedParameterList(object[] parameters, int RPCMethodId)
        {
            //İlk indeksinde RPC method id olan bir object[] parametre dizisi oluştur
            object[] _parameters = new object[parameters.Length + 1];
            _parameters[0] = RPCMethodId;
            for (int i = 1; i <= parameters.Length; i++)
            {
                _parameters[i] = parameters[i - 1];
            }

            return _parameters;
        }
        public static void MethodInvoke(string methodName, params object[] parameters)
        {
            /*
             Activator.CreateInstance + Invoke -> 2.17ms 1.17
             Only Invoke -> 1.1
             Only direct class call -> 0.5
            */
            float stime1 = Time.realtimeSinceStartup * 1000;

            Type type = MethodRPCObject[methodName];
            MethodInfo methodInfo = type.GetMethod(methodName);
            object instance = Activator.CreateInstance(type);

            methodInfo.Invoke(instance, parameters);

            float stime2 = Time.realtimeSinceStartup * 1000;
        }
    }
}
