using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public interface IPergSerialized
    {
        /// <summary>
        /// UDP protokolü kullanır.
        /// Veriler pakete sırayla yazılır ve istemci tarafından sırayla okunur.
        /// Sıkça gönderilmesi gereken durumlarda kullanılabilir.
        /// Eğer sıkça gönderilmesi gereken bir veri değil ise SendMethod metodunu kullanın.
        /// </summary>
        void SerializedDataPacket(PacketStream packetStream);
    }
}