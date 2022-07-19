using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class GameManager
    {
        public static Dictionary<int, NetworkObject> players = new Dictionary<int, NetworkObject>();
        public static Dictionary<int, Dictionary<int, NetworkObject>> objects = new Dictionary<int, Dictionary<int, NetworkObject>>();

        public static Dictionary<int, Dictionary<string, int>> mailAndClientId = new Dictionary<int, Dictionary<string, int>>(); // Room id - dicti
        public static Dictionary<int, bool> encsFilled = new Dictionary<int, bool>();
        public static Dictionary<int, string[]> encs = new Dictionary<int, string[]>();

        public static NetworkObject lastJoinedPlayer = null;
        public static NetworkObject lastSpawnedObject = null;
        public static int lastObjectIndex = 0;
    }
}