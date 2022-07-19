using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class PergSerializationType
    {
        public List<WrittenData> currentWrittenData = new List<WrittenData>(); //Şuanda yazılan değer
        public int serializedParameterCount = 0;
        public PergSerializationType()
        {

        }
        public PergSerializationType(List<WrittenData> currentWrittenData, int serializedParameterCount)
        {
            this.currentWrittenData = currentWrittenData;
            this.serializedParameterCount = serializedParameterCount;
        }
    }
}
