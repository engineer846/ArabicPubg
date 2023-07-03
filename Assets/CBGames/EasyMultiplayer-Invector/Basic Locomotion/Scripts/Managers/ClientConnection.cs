using EMI.Player;
using EMI.UI;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Managers
{
    /// <summary>
    /// This component is dynamically spawned for each connected client. This also contains
    /// reference information about each client that is automatically synced with all other
    /// clients.
    /// </summary>
    public class ClientConnection : NetworkBehaviour
    {
        #region Properties
        [SerializeField, Tooltip("If you want to have the server auto generate a random name for this player.")]
        protected bool randomPlayerName = false;

        #region Delegates
        public Delegates.GameObjectDelegate OnPlayerCharacterChanged;
        public Delegates.StringDelegate OnPlayerNameChanged;
        public Delegates.BoolDelegate OnPlayerDeadChanged;
        public Delegates.IntDelegate OnPlayerHealthChanged;
        public Delegates.IntDelegate OnPlayerMaxHealthChanged;
        public Delegates.StringDelegate OnSceneChanged;
        #endregion

        #region SyncVars
        [SyncVar(hook = nameof(TriggerNameChangedDelegate))] public string playerName = "";
        [SyncVar(hook = nameof(SetPlayerClientRef)), SerializeField] protected GameObject _playerCharacter = null;
        public GameObject playerCharacter
        {
            get
            {
                return _playerCharacter;
            }
            set
            {
                if (!GameObject.Equals(_playerCharacter, value))
                {
                    OnPlayerCharacterChanged?.Invoke(value);
                    _playerCharacter = value;
                    if (NetworkServer.active)
                        NetworkServer.ReplacePlayerForConnection(GetComponent<NetworkIdentity>().connectionToClient, _playerCharacter, true);
                }
            }
        }
        [SyncVar(hook = nameof(TriggerHealthChanged))] public int playerHealth = 100;
        [SyncVar(hook = nameof(TriggerMaxHealthChanged))] public int playerMaxHealth = 100;
        [SyncVar(hook = nameof(TriggerSceneChanged))] public string inScene = "";
        [SyncVar(hook = nameof(TriggerDeadChangedDelegate))] public bool isDead = false;
        [SyncVar] public int connId = 0;
        #endregion

        #region Internal
        public bool sceneInitialized { get; protected set; } = false;
        public bool nameInitialized { get; protected set; } = false;
        public bool healthInitialized { get; protected set; } = false;
        #endregion
        #endregion

        #region Initialization
        protected virtual void Start()
        {
            DontDestroyOnLoad(this);
            if (NetworkServer.active)
            {
                StartCoroutine(InitConnId());
            }

            // If you have the UI Package imported.
            if (GetComponent<NetworkIdentity>().isLocalPlayer == true)
            {
                if (UIValueStorage.instance != null)
                {
                    UIValueStorage.instance.CallUpdateName(); // Tells the UIValueStorage to find this on the server and update the playerName according to the owning players value
                }
            }

            // Update the player list
            EMI_NetworkManager.instance.playerList.Add(this);

            if (NetworkServer.active && randomPlayerName)
            {
                playerName = $"{RandomName()} {RandomName()}";
            }
        }
        protected virtual void OnDestroy()
        {
            // Update the player list
            if (EMI_NetworkManager.instance && EMI_NetworkManager.instance.playerList.Contains(this))
                EMI_NetworkManager.instance.playerList.Remove(this);
        }
        protected virtual IEnumerator InitConnId()
        {
            yield return new WaitUntil(() => NetworkServer.active);
            connId = GetComponent<NetworkIdentity>().connectionToClient.connectionId;
        }
        #endregion

        #region Random Player Name
        protected virtual string RandomName()
        {
            List<string> options = new List<string>()
            {
                "Aaron",
                "Brittany",
                "Cealven",
                "Daniel",
                "Easton",
                "Frankie",
                "Genve",
                "Hillary",
                "Indigo",
                "John",
                "Jackson",
                "Kristina",
                "Lenney",
                "Monreo",
                "Nigeal",
                "Ozze",
                "Penny",
                "Quincy",
                "Smith",
                "Tuscan",
                "Uncle",
                "Vinney",
                "Wes",
                "Wick",
                "Weston",
                "Xena",
                "Yumma",
                "Zank"
            };
            return options[Random.Range(0, options.Count)];
        }
        #endregion

        #region Hooks
        protected virtual void TriggerSceneChanged(string oldValue, string newValue)
        {
            OnSceneChanged?.Invoke(newValue);
            sceneInitialized = true;
        }
        protected virtual void TriggerHealthChanged(int oldValue, int newValue)
        {
            OnPlayerHealthChanged?.Invoke(newValue);
            healthInitialized = true;
        }
        protected virtual void TriggerMaxHealthChanged(int oldValue, int newValue)
        {
            OnPlayerMaxHealthChanged?.Invoke(newValue);
        }
        protected virtual void TriggerDeadChangedDelegate(bool oldValue, bool newValue)
        {
            OnPlayerDeadChanged?.Invoke(newValue);
        }
        protected virtual void TriggerNameChangedDelegate(string oldValue, string newValue)
        {
            OnPlayerNameChanged?.Invoke(newValue);
            nameInitialized = true;
        }
        protected virtual void SetPlayerClientRef(GameObject oldGO, GameObject newGO)
        {
            if (oldGO)
                oldGO.GetComponent<BasicNetworkCalls>().client = null;
            if (newGO)
                newGO.GetComponent<BasicNetworkCalls>().client = this;
        }
        #endregion

        #region RPCs
        #region Commands
        [Command(requiresAuthority = true)]
        public virtual void Cmd_UpdatePlayerName(string value)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            playerName = value;
            #endif
        }
        #endregion
        #endregion
    }
}
