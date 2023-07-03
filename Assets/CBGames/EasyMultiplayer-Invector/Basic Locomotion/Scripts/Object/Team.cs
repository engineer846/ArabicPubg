using EMI.Managers;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Object
{
    public class Team : NetworkBehaviour
    {
        public Delegates.StringDelegate OnTeamChanged;

        [SerializeField, Tooltip("If you want to automatically assign the team based on the \"CharacterTeam\" of the Network Manager.")]
        protected bool autoAssignCharacterTeam = false;
        [SyncVar(hook = nameof(Hook_SetTeam)), SerializeField, Tooltip("The name of the team you want this object to be a member of")]
        protected string _teamName = "";
        public string teamName { 
            get 
            { 
                return _teamName; 
            } 
            protected set 
            { 
                _teamName = value;
                OnTeamChanged?.Invoke(_teamName);
                SetEnemyTeams();
            } 
        }
        [SerializeField, Tooltip("If you want to use a different team data set than what is applied to the NetworkManager, than apply one here. Otherwise leave this blank to use the same team data set that is on the network manager.")]
        protected TeamData teamData = null;
        protected List<string> enemyTeams = new List<string>();

        protected virtual void Awake()
        {
            if (!teamData)
            {
                teamData = EMI_NetworkManager.instance.teams;
                if (!teamData)
                {
                    Destroy(this);
                    return;
                }

                if (autoAssignCharacterTeam)
                {
                    teamName = EMI_NetworkManager.instance.characterTeam;
                }

                int foundIndex = teamData.teams.FindIndex(x => x.name == teamName);
                if (foundIndex < 0)
                {
                    Destroy(this);
                    return;
                }
            }
            else
            {
                int foundIndex = teamData.teams.FindIndex(x => x.name == teamName);
                if (foundIndex < 0)
                {
                    Destroy(this);
                    return;
                }
                teamName = teamData.teams[foundIndex].name;
            }
        }

        public virtual bool IsEnemy(string teamName)
        {
            return enemyTeams.Contains(teamName);
        }
    
        /// <summary>
        /// Only the server can call the following function. 
        /// It will change the team of this object. It will also automatically re-update
        /// to the new set of enemy teams for you based on your new team name.
        /// </summary>
        /// <param name="newTeamName"></param>
        [ServerCallback]
        public virtual void ChangeTeam(string newTeamName)
        {
            teamName = newTeamName;
        }

        protected virtual void SetEnemyTeams()
        {
            enemyTeams.Clear();
            teamData = EMI_NetworkManager.instance.teams;
            int foundIndex = teamData.teams.FindIndex(x => x.name == teamName);
            enemyTeams.AddRange(teamData.teams[foundIndex].enemyTeams);
        }
        #region Hooks
        protected virtual void Hook_SetTeam(string oldTeam, string newTeam)
        {
            OnTeamChanged?.Invoke(newTeam);
            SetEnemyTeams();
        }
        #endregion
    }
}
