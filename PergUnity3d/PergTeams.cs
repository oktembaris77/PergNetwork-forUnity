using System;
using System.Collections.Generic;
using System.Text;

namespace PergUnity3d
{
    public class PergTeams
    {
        public struct PergTeamOptions
        {
            public int maxPlayerPerTeam;
            public string teamName;

            public PergTeamOptions(int maxPlayerPerTeam, string teamName)
            {
                this.maxPlayerPerTeam = maxPlayerPerTeam;
                this.teamName = teamName;
            }
        }
        public struct PergTeam
        {
            //int -> ownerClientId
            public List<int> players;
            public PergTeamOptions pergTeamOptions;

            public PergTeam(List<int> players, PergTeamOptions pergTeamOptions)
            {
                this.players = players;
                this.pergTeamOptions = pergTeamOptions;
            }
        }

        //int -> Team Id, PergTeamOptions
        internal static Dictionary<int, PergTeam> PergTeamList = new Dictionary<int, PergTeam>();
        internal static int lastPergTeamId = 0;

        public static void AddPergTeam(PergTeam pergTeam)
        {
            PergTeamList.Add(++lastPergTeamId, pergTeam);
        }
        public static void RemovePergTeam(int pergTeamId)
        {
            if(PergTeamList.TryGetValue(pergTeamId, out PergTeam pergTeam))
            {
                PergTeamList.Remove(pergTeamId);
            }
        }
        public static void AddPlayer(int ownerClientId, int pergTeamId)
        {
            if(PergTeamList.TryGetValue(pergTeamId, out PergTeam pergTeam))
            {
                if (pergTeam.pergTeamOptions.maxPlayerPerTeam > pergTeam.players.Count)
                {
                    pergTeam.players.Add(ownerClientId);
                }
            }
        }
        public static void RemovePlayer(int ownerClientId, int pergTeamId)
        {
            if (PergTeamList.TryGetValue(pergTeamId, out PergTeam pergTeam))
            {
                pergTeam.players.Remove(ownerClientId);
            }
        }
        public static void SetPlayerTeam(int ownerClientId, int currentPergTeamId, int newPergTeamId)
        {
            RemovePlayer(ownerClientId, currentPergTeamId);
            AddPlayer(ownerClientId, newPergTeamId);
        }
        public static Dictionary<int, PergTeam> GetPergTeamList()
        {
            return PergTeamList;
        }
        public static bool Teammate(int pergTeamId1, int pergTeamId2)
        {
            if (pergTeamId1 == pergTeamId2)
                return true;
            else return false;
        }
    }
}