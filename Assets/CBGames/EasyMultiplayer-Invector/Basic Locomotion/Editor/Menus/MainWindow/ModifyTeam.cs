using EMI.Object;
using static EMI.Object.TeamData;
using System.Collections.Generic;
using UnityEditor;

namespace EMI.Menus
{
    public class ModifyTeam
    {
        public static TeamData teamData = null;

        public static List<string> FindTeamDataPaths()
        {
            List<string> retVal = new List<string>();
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(TeamData).Name);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                retVal.Add(path);
            }
            return retVal;
        }

        #region Add/Remove Enemy Team From Target Team
        public static void AddEnemyTeam(string teamName, string enemyTeam)
        {
            teamName = teamName.Trim().ToLower();
            enemyTeam = enemyTeam.Trim().ToLower();
            int found = teamData.teams.FindIndex(x => x.name == teamName);
            if (found < 0)
            {
                teamData.teams.Add(new TeamInfo()
                {
                    name = teamName,
                    enemyTeams = new List<string>() { enemyTeam }
                });
            }
            else if (!teamData.teams[found].enemyTeams.Contains(enemyTeam))
            {
                teamData.teams[found].enemyTeams.Add(enemyTeam);
            }
        }

        public static  void RemoveEnemyTeam(string teamName, string enemyTeam)
        {
            teamName = teamName.Trim().ToLower();
            enemyTeam = enemyTeam.Trim().ToLower();
            int found = teamData.teams.FindIndex(x => x.name == teamName);
            if (found > -1 && !teamData.teams[found].enemyTeams.Contains(enemyTeam))
            {
                teamData.teams[found].enemyTeams.Remove(enemyTeam);
            }
        }
        #endregion

        #region Add/Remove Teams
        public static void AddTeam(string teamName)
        {
            teamName = teamName.Trim().ToLower();
            int found = teamData.teams.FindIndex(x => x.name == teamName);
            if (found < 0)
            {
                teamData.teams.Add(new TeamInfo()
                {
                    name = teamName,
                    enemyTeams = new List<string>()
                });
            }
        }
        public static void RemoveTeam(string teamName)
        {
            teamName = teamName.Trim().ToLower();
            int found = teamData.teams.FindIndex(x => x.name == teamName);
            if (found > -1)
            {
                teamData.teams.RemoveAt(found);
            }
        }
        #endregion

        #region Change Team Name
        public static void ChangeTeamName(string oldTeamName, string newTeamName)
        {
            oldTeamName = oldTeamName.Trim().ToLower();
            newTeamName = newTeamName.Trim().ToLower();
            for(int i = 0; i < teamData.teams.Count; i++)
            {
                // update enemy team settings on this team
                for(int j = 0; j < teamData.teams[i].enemyTeams.Count; j++)
                {
                    if (teamData.teams[i].enemyTeams[j].Trim().ToLower() == oldTeamName)
                    {
                        teamData.teams[i].enemyTeams[j] = newTeamName;
                    }
                }

                // update the team name if it matches the old team name
                if (teamData.teams[i].name.ToLower().Trim() == oldTeamName)
                {
                    teamData.teams[i] = new TeamInfo()
                    {
                        name = newTeamName,
                        enemyTeams = teamData.teams[i].enemyTeams
                    };
                }
            }

        }
        #endregion
    }
}
