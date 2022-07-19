using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PergUnity3d
{
    public class InstantiatePreferences
    {
        public object[] initDatas;

        public InstantiatePreferences(object[] initDatas)
        {
            this.initDatas = initDatas;
        }
    }
}
