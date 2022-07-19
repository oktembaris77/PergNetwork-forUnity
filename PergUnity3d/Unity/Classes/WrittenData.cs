using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class WrittenData
    {
        public object data;
        public VarType varType;

        public WrittenData(object data, VarType varType)
        {
            this.data = data;
            this.varType = varType;
        }
    }
}
