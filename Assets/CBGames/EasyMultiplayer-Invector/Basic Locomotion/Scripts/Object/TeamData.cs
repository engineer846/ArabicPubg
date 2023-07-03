using System;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Object
{
    [CreateAssetMenu(fileName = "TeamData", menuName = "EMI/Generate New Team Data", order = 1), Serializable]
    public class TeamData : ScriptableObject
    {
        [Serializable]
        public struct TeamInfo
        {
            public string name;
            public List<string> enemyTeams;
        }
        [SerializeField]
        public List<TeamInfo> teams = new List<TeamInfo>();
    }
}
