using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class PergConverter
    {
        public static VarType StringToVarType(string type)
        {
            VarType varType = VarType.none;

            switch (type)
            {
                case "Int":
                    varType = VarType._int;
                    break;
                case "Float":
                    varType = VarType._float;
                    break;
                case "Trigger":
                    varType = VarType._boolean;
                    break;
                case "Bool":
                    varType = VarType._boolean;
                    break;
            }

            return varType;
        }
        public static Type WrittenDataConvert(WrittenData writtenData)
        {
            switch (writtenData.varType)
            {
                case VarType._short: //short
                    WrittenDataToT<short>(writtenData);
                    break;
                case VarType._int: //int
                    WrittenDataToT<int>(writtenData);
                    break;
                case VarType._long: //long
                    WrittenDataToT<long>(writtenData);
                    break;
                case VarType._float: //float
                    WrittenDataToT<float>(writtenData);
                    break;
                case VarType._string: //string
                    WrittenDataToT<string>(writtenData);
                    break;
                case VarType._byte: //byte
                    WrittenDataToT<byte>(writtenData);
                    break;
                case VarType._boolean: //bool
                    WrittenDataToT<bool>(writtenData);
                    break;
                case VarType._Vector3: //Vector3
                    WrittenDataToT<UnityEngine.Vector3>(writtenData);
                    break;
                case VarType._Quaternion: //Quaternion
                    WrittenDataToT<UnityEngine.Quaternion>(writtenData);
                    break;
            }
            
            return WrittenDataToT<Type>(writtenData);
        }
        public static T WrittenDataToT<T>(WrittenData writtenData)
        {
            return (T)writtenData.data;
        }
        public static short WrittenDataToShort(WrittenData writtenData)
        {
            return (short)writtenData.data;
        }
        public static int WrittenDataToInt(WrittenData writtenData)
        {
            return (int)writtenData.data;
        }
        public static long WrittenDataToLong(WrittenData writtenData)
        {
            return (long)writtenData.data;
        }
        public static float WrittenDataToFloat(WrittenData writtenData)
        {
            return (float)writtenData.data;
        }
        public static string WrittenDataToString(WrittenData writtenData)
        {
            return (string)writtenData.data;
        }
        public static byte WrittenDataToByte(WrittenData writtenData)
        {
            return (byte)writtenData.data;
        }
        public static bool WrittenDataToBool(WrittenData writtenData)
        {
            return (bool)writtenData.data;
        }
        public static UnityEngine.Vector3 WrittenDataToVector3(WrittenData writtenData)
        {
            return (UnityEngine.Vector3)writtenData.data;
        }
        public static UnityEngine.Quaternion WrittenDataToQuaternion(WrittenData writtenData)
        {
            return (UnityEngine.Quaternion)writtenData.data;
        }
    }
}

/*
 none,
_short,
_int,
_long,
_float,
_string,
_byte,
_boolean,
_Vector3,
_Quaternion,
 */