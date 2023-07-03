using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
using EMI.Utils;
using System.Collections.Generic;
using EMI.Object;
using EMI.Player;


namespace EMI.Managers
{
    public class EMI_NetworkManager : NetworkManager
    {
        #region Properties
        #region Instance
        public static EMI_NetworkManager instance = null;
        #endregion

        #region Delegates
        public delegate void NCDelegate(NetworkConnection conn);
        public NCDelegate OnClientConnectedToServer;         // Called on the server when a new client connects
        public NCDelegate OnClientDisconnectedFromServer;    // Called on the server when a new client disconnects
        public Delegates.VoidDelegate OnClientStarted;       // Called when the local client is started
        public Delegates.VoidDelegate OnClientStopped;       // Called when the local client is stopped
        public Delegates.VoidDelegate OnClientConnected;     // Called when the local client is connected to the server
        public Delegates.VoidDelegate OnClientDisconnected;  // Called when the local client is disconnected from the server
        public Delegates.VoidDelegate OnServerStarted;       // Called when the local server is started
        public Delegates.VoidDelegate OnServerStopped;       // Called when the local server is stopped
        public NCDelegate OnServerAddedPlayer;               // Called on the server when the client requests to add a player (ClientConnection)
        public NCDelegate OnClientMarkedReady;               // Called on the server when a client says their "ready"
        public CallbackList<ClientConnection> playerList = new CallbackList<ClientConnection>(); // List of currently connected ClientConnection's (available for all clients & server - fully synced)
        #endregion

        #region Modifiables
        [Tooltip("The object to spawn when your own client calls to spawn your character.")]
        public GameObject characterToSpawn = null;
        [SerializeField, Tooltip("The default set of team data to use in your project. Can be overridden by the individual \"Teams\" component.")]
        protected TeamData _teams = null;
        public TeamData teams { get { return _teams; } protected set { _teams = value; } }
        [Tooltip("The name of the team to put your own character on when you spawn.")]
        public string characterTeam = "";
        [SerializeField, Tooltip("If you want to automatically spawn your character when you connect to the server")] 
        protected bool autoSpawnCharacter = false;
        [SerializeField, Tooltip("If you want to enable a runtime debug window, Requires the \"DebugWindow\" component, when you click the \"~\" key")] protected bool debugWindow = false;
        [SerializeField, Tooltip("If you want this component to log everything it does to the console")] 
        protected bool verboseLogging = false;
        [SerializeField, Scene, Tooltip("The scene you want to move to when you join the server. This scene will be addively loaded if it is not already loaded.")] 
        protected string moveToSceneOnJoin;
        [SerializeField, Tooltip("The name of the object, must also be tagged with \"NetworkSpawnPoint\", you want to jump to after joining the server and loading the scenes. This particular jump will ignore team settings. So this point must be unique and part of one of the loaded scenes.")] 
        protected string jumpToPointName = "";
        #endregion

        #region Internal
        // dictionary is only useable by the server
        protected Dictionary<NetworkConnection, GameObject> _playerPrefabList = new Dictionary<NetworkConnection, GameObject>();
        // bool only usabled by the client
        protected bool _spawnedPlayerPrefab = false;
        #endregion
        #endregion

        #region Initialization
        public override void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this) Destroy(this);
            base.Awake();
        }
        
        #if UNITY_SERVER || UNITY_EDITOR
        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            OnClientMarkedReady?.Invoke(conn);
        }
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            OnServerAddedPlayer?.Invoke(conn);
        }
        
        /// <summary>
        /// Called as soon as the server is started
        /// </summary>
        public override void OnStartServer()
        {
            // Server Only Handlers (Receives client requests)
            NetworkServer.RegisterHandler<JumpToScene>(ClientRequestedJumpToScene);
            NetworkServer.RegisterHandler<SpawnPlayer>(SpawnPlayer);
            NetworkServer.RegisterHandler<FinishedMoving>(PlayerFinishedSceneMove);
            if (verboseLogging) Debug.Log("<color=blue>Server</color> finished registering handlers.");
            OnServerStarted?.Invoke();

            base.OnStartServer();
        }

        /// <summary>
        /// Called as soon as the server is stopped
        /// </summary>
        public override void OnStopServer()
        {
            NetworkServer.UnregisterHandler<JumpToScene>();
            NetworkServer.UnregisterHandler<SpawnPlayer>();
            NetworkServer.UnregisterHandler<FinishedMoving>();
            if (verboseLogging) Debug.Log("<color=blue>Server</color> finished un-registering handlers.");
            OnServerStopped?.Invoke();

            base.OnStopServer();
        }
        #endif

        /// <summary>
        /// This is called as soon as the client successfully connects to the server
        /// </summary>
        public override void OnStartClient()
        {
            NetworkClient.RegisterHandler<ClientJumpToScene>(JumpToScene);
            if (verboseLogging) Debug.Log("<color=green>Client</color>Finished registering handlers.");
            OnClientStarted?.Invoke();

            base.OnStartClient();
        }

        public override void OnStopClient()
        {
            NetworkClient.UnregisterHandler<ClientJumpToScene>();
            if (verboseLogging) Debug.Log("<color=green>Client</color>Finished un-registering handlers.");
            _spawnedPlayerPrefab = false; // reset the initial spawn ticker to make sure if the client joins another server the spawn happens correctly.
            OnClientStopped?.Invoke();

            base.OnStopClient();
        }
        #endregion

        #region Server Actions
        public override void OnServerConnect(NetworkConnection conn) 
        {
            OnClientConnectedToServer?.Invoke(conn);
            base.OnServerConnect(conn);
        }
        public override void OnServerDisconnect(NetworkConnection conn)
        {
            OnClientDisconnectedFromServer?.Invoke(conn);
            base.OnServerDisconnect(conn);
        }
        #endregion

        #region Connections
        public override void OnClientConnect()
        {
            base.OnClientConnect(); // marks client as ready and creates a "ClientConnection" object for this client
            OnClientConnected?.Invoke();
            if (verboseLogging) Debug.Log($"<color=green>Client</color> connected to server: [connectionId={NetworkClient.connection.connectionId}]");
            if (!string.IsNullOrEmpty(moveToSceneOnJoin))
            {
                // immediately request the server to be moved to this target scene, if auto sapwning the server reply will trigger the spawn
                RequestJumpToScene(PointsUtil.PointType.NetworkSpawnPoint, moveToSceneOnJoin, jumpToPointName, "", "");
            }
            else if (autoSpawnCharacter && !_spawnedPlayerPrefab)
            {
                // if no scene to jump to just ask the server to spawn your character in it's main scene
                //RequestSpawnCharacter(characterToSpawn.name, PointsUtil.PointType.NetworkSpawnPoint, SceneManager.GetActiveScene().name, jumpToPointName, characterTeam);
                RequestSpawnCharacter(characterToSpawn.name, PointsUtil.PointType.NetworkSpawnPoint, SceneManager.GetActiveScene().name, jumpToPointName, "", "");
            }
        }
        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            OnClientDisconnected?.Invoke();
        }
        #endregion

        #region Scene Management
        /// <summary>
        /// The client sends a request to the server to jump to a particular scene with the requested data
        /// </summary>
        /// <param name="sceneName"></param>
        [Client]
        public virtual void RequestJumpToScene(PointsUtil.PointType pointType, string sceneName, string pointName, string teamName = "", string unloadScene = "")
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            // Ask the server to jump you to this information
            if (verboseLogging) Debug.Log($"<color=green>Client</color> sending <color=purple>JumpToScene</color> request with: <color=magenta>[PointType: {pointType}, SceneName: {sceneName.GetCleanSceneName()}, PointName: {pointName}, TeamName: {teamName}]</color>");
            NetworkClient.Send(new JumpToScene
            {
                pointType = pointType, //The type of point to jump to (Respawn, NetworkSpawn, Jump)
                sceneName = sceneName.GetCleanSceneName(), //Scene to jump to
                pointName = pointName, //Point name in the new scene to jump to
                teamName = teamName,    //Only use points that are a member of this team
                unloadScene = unloadScene
            });
            #endif
        }

        /// <summary>
        /// This is the response from the server to the client requesting to jump to a scene.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        [Client]
        protected virtual void JumpToScene(ClientJumpToScene data)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (verboseLogging) Debug.Log($"<color=green>Client</color> received <color=purple>ClientJumpToScene</color> request from the server with: <color=magenta>[PointType: {data.pointType}, SceneName: {data.sceneName.GetCleanSceneName()}, PointName: {data.pointName}, TeamName: {data.teamName}, ConnectionId: {data.connectionId}, UnloadScene: {data.unloadScene}]</color>");
            StartCoroutine(JumpToScene(data.pointType, data.sceneName.GetCleanSceneName(), data.pointName, data.teamName, data.connectionId, data.unloadScene));
            #endif
        }
        
        [Client]
        protected virtual IEnumerator JumpToScene(PointsUtil.PointType pointType, string sceneName, string pointName, string teamName, int connectionId, string unloadScene)
        {
            #if !UNITY_SERVER || UNITY_EDITOR
            if (SceneManager.GetSceneByName(sceneName).name.GetCleanSceneName() != sceneName.GetCleanSceneName() && !SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                if (verboseLogging) Debug.Log($"<color=green>Client</color> loading scene: <color=magenta>{sceneName}</color>");
                AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!loadScene.isDone)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(0.001f);
                if (verboseLogging) Debug.Log($"<color=green>Client</color> finished loading scene: <color=magenta>{sceneName}</color>");
            }
            
            //if (verboseLogging) Debug.Log($"<color=green>Client</color> setting active scene: <color=magenta>{sceneName}</color>");
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            yield return new WaitForSeconds(0.001f); // Wait for the scene to finalize switching.
            GameObject targetPlayer = GameObject.FindGameObjectsWithTag("Player").ToList().Find(x =>
                x.GetComponent<BasicNetworkCalls>() &&
                x.GetComponent<BasicNetworkCalls>().client != null &&
                x.GetComponent<BasicNetworkCalls>().client.connId == connectionId
            );
            if (targetPlayer)
            {
                if (verboseLogging) Debug.Log($"<color=green>Client</color> moving target player: <color=magenta>{targetPlayer}</color> to scene: <color=magenta>{sceneName.GetCleanSceneName()}</color>");
                SceneManager.MoveGameObjectToScene(targetPlayer, SceneManager.GetSceneByName(sceneName.GetCleanSceneName()));
                if (verboseLogging) Debug.Log($"<color=green>Client</color> looking for target point - TYPE: <color=magenta>{pointType}</color>, NAME: <color=magenta>{pointName}</color>, TEAMNAME: <color=magenta>{teamName}</color>, INSCENE: <color=magenta>{sceneName}</color>");
                GameObject targetPoint = PointsUtil.GetPoint(pointType, pointName, teamName, sceneName);
                if (targetPoint != null)
                {
                    if (verboseLogging) Debug.Log($"<color=green>Client</color> jumping target player: <color=magenta>{targetPlayer}</color> to target point: <color=magenta>{targetPoint}</color>");
                    // point found jump the player to it
                    targetPlayer.transform.position = targetPoint.transform.position;
                    targetPlayer.transform.rotation = targetPoint.transform.rotation;
                }
                else
                {
                    if (verboseLogging) Debug.Log($"<color=green>Client</color> jumping target player: <color=magenta>{targetPlayer}</color> to Vector3.zero since no target point was found.");
                    // No point found, just reset the player
                    targetPlayer.transform.position = Vector3.zero;
                    targetPlayer.transform.rotation = Quaternion.identity;
                }
            }
            else if (verboseLogging)
            {
                Debug.Log($"<color=green>Client</color> failed to find any target player tagged with \"Player\" with [connectionId={connectionId}], skipping move targetPlayer method.");
            }

            if (!_spawnedPlayerPrefab && autoSpawnCharacter)
            {
                // If your character has not yet been spawned then spawn your character in the new scene
                RequestSpawnCharacter(
                    prefabName: characterToSpawn.name,
                    pointType: PointsUtil.PointType.NetworkSpawnPoint,
                    sceneName: sceneName,
                    pointName: pointName,
                    teamName: teamName,
                    unloadScene: unloadScene
                );
            }
            else
            {
                // Character has finished spawning, now make sure they're in the proper scene
                NetworkClient.Send(new FinishedMoving
                {
                    sceneName = sceneName
                });
                if (!string.IsNullOrEmpty(unloadScene))
                {
                    try
                    {
                        if (!NetworkServer.active)
                        {
                            if (verboseLogging) Debug.Log($"<color=green>Client</color> unload scene: <color=magenta>{unloadScene}</color>");
                            SceneManager.UnloadSceneAsync(unloadScene);
                        }
                        #if UNITY_SERVER || UNITY_EDITOR
                        else
                        {
                            UnloadIfEmpty(unloadScene);
                        }
                        #endif
                    }
                    catch { }
                }
            }
            #endif
            yield return null;
        }

        #if UNITY_SERVER || UNITY_EDITOR
        [Server]
        public virtual void UnloadIfEmpty(string sceneName)
        {
            if (verboseLogging) Debug.Log($"<color=blue>Server</color> Check if scene: <color=magenta>{sceneName.GetCleanSceneName()}</color> can be unloaded.");
            foreach (ClientConnection conn in FindObjectsOfType<ClientConnection>().ToList())
            {
                if (conn.inScene.GetCleanSceneName() == sceneName.GetCleanSceneName())
                {
                    if (verboseLogging) Debug.Log($"<color=blue>Server</color> Cannot unload: <color=magenta>{sceneName.GetCleanSceneName()}</color> because clients are in that scene.");
                    return;
                }
            }
            if (verboseLogging) Debug.Log($"<color=blue>Server</color> Unloading scene: <color=magenta>{sceneName.GetCleanSceneName()}</color>");
            SceneManager.UnloadSceneAsync(sceneName.GetCleanSceneName());
        }

        /// <summary>
        /// This is called on the server from a client when they have finised moving their character to a new scene.
        /// </summary>
        [Server]
        public virtual void PlayerFinishedSceneMove(NetworkConnection conn, FinishedMoving data)
        {
            ClientConnection targetConn = ClientUtil.GetClientConnection(conn.connectionId);
            if (targetConn.playerCharacter != null && targetConn.playerCharacter.scene.name.GetCleanSceneName() != data.sceneName.GetCleanSceneName())
            {
                if (verboseLogging) Debug.Log("<color=blue>Server</color> received client finished moving and the client object on the server isn't on the right scene, moving them to the correct scene.");
                SceneManager.MoveGameObjectToScene(targetConn.playerCharacter, SceneManager.GetSceneByName(data.sceneName.GetCleanSceneName()));
            }
            else if (verboseLogging)
            {
                Debug.Log("<color=blue>Server</color> received client finished moving but no playerCharacter was assigned to this client connection. Not doing anything.");
            }
        }

        /// <summary>
        /// The server will receive this when a client requests to be jumped to a scene.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        [Server]
        public virtual void ClientRequestedJumpToScene(NetworkConnection conn, JumpToScene data)
        {
            if (verboseLogging) Debug.Log($"<color=blue>Server</color> received <color=purple>JumpToScene</color> request from client <color=green>[connectionId={conn.connectionId}]</color> with data: <color=magenta>[PointType: {data.pointType}, PointName: {data.pointName}, SceneName: {data.sceneName}, TeamName: {data.teamName}, UnloadScene: {data.unloadScene}]</color>");
            StartCoroutine(ServerLoadScene(data.pointType, data.sceneName, data.pointName, data.teamName, conn, data.unloadScene));
        }

        /// <summary>
        /// Server Only Method - Will additively load the scene and tell the NetworkConn to load it as well.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="pointName"></param>
        /// <param name="teamName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        [Server]
        protected virtual IEnumerator ServerLoadScene(PointsUtil.PointType pointType, string sceneName, string pointName, string teamName = "", NetworkConnection conn = null, string unloadScene = "")
        {
            if (SceneManager.GetSceneByName(sceneName).name.GetCleanSceneName() != sceneName && !SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                if (verboseLogging) Debug.Log($"<color=blue>Server</color> loading scene: <color=magenta>{sceneName}</color>");
                AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!loadScene.isDone)
                {
                    yield return null;
                }
                yield return new WaitForSeconds(0.001f);
                if (verboseLogging) Debug.Log($"<color=blue>Server</color> finished loading scene: <color=magenta>{sceneName}</color>");
            }
            if (conn != null)
            {
                yield return new WaitForSeconds(0.001f); // Wait for the scene to finialize before telling people to move to it.

                ClientConnection cc = ClientUtil.GetClientConnection(conn.connectionId);
                if (cc.playerCharacter != null)
                {
                    GameObject foundPoint = PointsUtil.GetPoint(pointType, pointName, teamName, sceneName.GetCleanSceneName());
                    if (verboseLogging) Debug.Log($"<color=blue>Server</color> jumping target player <color=magenta>{cc.playerCharacter}</color> to scene: <color=magenta>{sceneName.GetCleanSceneName()}</color>");
                    //SceneManager.MoveGameObjectToScene(cc.playerCharacter, SceneManager.GetSceneByName(sceneName.GetCleanSceneName()));
                    if (foundPoint == null)
                    {
                        if (verboseLogging) Debug.Log($"<color=blue>Server</color> no target point found, jumping character to <color=magenta>Vector3.zero</color>");
                        cc.playerCharacter.transform.position = Vector3.zero;
                    }
                    else
                    {
                        if (verboseLogging) Debug.Log($"<color=blue>Server</color> jumping character to point: <color=magenta>{foundPoint}</color>");
                        cc.playerCharacter.transform.position = foundPoint.transform.position;
                        cc.playerCharacter.transform.rotation = foundPoint.transform.rotation;
                    }
                }
                else if (verboseLogging)
                {
                    Debug.Log($"<color=blue>Server</color> not target player found for connectionID: <color=magenta>{conn.connectionId}</color>, skipping character scene moving.");
                }

                if (verboseLogging) Debug.Log($"<color=blue>Server</color> sending <color=purple>ClientJumpToScene</color> to client <color=green>[connectionId={conn.connectionId}]</color> with data: <color=magenta>[PointType: {pointType}, PointName: {pointName}, SceneName: {sceneName}, TeamName: {teamName}, ConnectionId: {conn.connectionId}, UnloadScene: {unloadScene}]</color>");
                conn.Send(new ClientJumpToScene
                {
                    pointType = pointType,
                    sceneName = sceneName,
                    pointName = pointName,
                    teamName = teamName,
                    connectionId = conn.connectionId,
                    unloadScene = unloadScene
                });
            }
        }
        #endif
        #endregion

        #region Player Spawning
        /// <summary>
        /// This will be called by the vThirdPersonController character that you own that is spawned.
        /// This lets you know if you need to request getting spawned again or not. This is only for 
        /// first time connecting clients. Every spawn after that doesn't really matter.
        /// </summary>
        [Client]
        public virtual void MarkInitialSpawnCompleted()
        {
            _spawnedPlayerPrefab = true;
        }

        /// <summary>
        /// This client will ask the server to spawn a new character for them.
        /// </summary>
        /// <param name="prefabName">Name of the prefab to spawn</param>
        /// <param name="pointType">The type of point of where to spawn the prefab</param>
        /// <param name="sceneName">The name of the unity scene to move this prefab to</param>
        /// <param name="pointName">The name of the point in the pointType group you want to jump this prefab to.</param>
        /// <param name="teamName">The teamName to assign this prefab to (if has team component)</param>
        /// <param name="unloadScene">The name of the unity scene to unload</param>
        [Client]
        public virtual void RequestSpawnCharacter(string prefabName, PointsUtil.PointType pointType, string sceneName = "", string pointName = "", string teamName = "", string unloadScene = "")
        {
            if (verboseLogging) Debug.Log($"<color=green>Client</color> sending <color=purple>SpawnPlayer</color> request to server with data: <color=magenta>[PrefabName: {prefabName}, PointType: {pointType}, SceneName: {sceneName}, PointName: {pointName}, TeamName: {teamName}]</color>");
            NetworkClient.Send(new SpawnPlayer
            {
                prefabName = prefabName,
                pointType = pointType,
                sceneName = sceneName,
                pointName = pointName,
                teamName = teamName,
                unloadScene = unloadScene
            });
        }
        /// <summary>
        /// This is called when a client wishes to spawn a player
        /// </summary>
        /// <param name="conn">The Networkconnection of the client sending the request</param>
        /// <param name="data">The data with which to spawn the prefab</param>
        [Server]
        protected virtual void SpawnPlayer(NetworkConnection conn, SpawnPlayer data)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            StartCoroutine(E_SpawnPlayer(conn, data));
            #endif
        }
        protected virtual IEnumerator E_SpawnPlayer(NetworkConnection conn, SpawnPlayer data)
        {
            #if UNITY_SERVER || UNITY_EDITOR
            if (verboseLogging) Debug.Log($"<color=blue>Server</color> received <color=purple>SpawnPlayer</color> request from <color=green>[connectionId={conn.connectionId}]</color> with data: <color=magenta>[PrefabName: {data.prefabName}, PointType: {data.pointType}, SceneName: {data.sceneName}, PointName: {data.pointName}, TeamName: {data.teamName}]</color>");
            GameObject prefab = spawnPrefabs.Find(x => x.name == data.prefabName);
            if (prefab)
            {
                if (verboseLogging) Debug.Log($"<color=blue>Server</color> found target spawnable prefab in list: <color=magenta>{prefab}</color>");
                GameObject point = PointsUtil.GetPoint(data.pointType, data.pointName, data.teamName, data.sceneName);
                GameObject character = null;
                if (point)
                {
                    if (verboseLogging) Debug.Log($"<color=blue>Server</color> locally spawning: <color=magenta>{data.prefabName}</color> in scene <color=magenta>{SceneManager.GetActiveScene().name}</color> at found point.");
                    character = Instantiate(prefab, point.transform.position, point.transform.rotation);
                }
                else
                {
                    if (verboseLogging) Debug.Log($"<color=blue>Server</color> locally spawning: <color=magenta>{data.prefabName}</color> at Vector3.zero in scene <color=magenta>{SceneManager.GetActiveScene().name}</color> because no point was found.");
                    character = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                }
                if (character.GetComponent<Team>() && !string.IsNullOrEmpty(data.teamName))
                {
                    if (verboseLogging) Debug.Log($"<color=blue>Server</color> setting: <color=magenta>{data.prefabName}</color> team to: <color=magenta>{data.teamName}</color>.");
                    character.GetComponent<Team>().ChangeTeam(data.teamName);
                }

                if (_playerPrefabList.ContainsKey(conn))
                {
                    if (verboseLogging) Debug.Log($"<color=blue>Server</color> destroying: <color=magenta>{_playerPrefabList[conn].name}</color> from the network.");
                    Destroy(_playerPrefabList[conn]);
                    yield return new WaitForSeconds(0.001f); // wait for destroy to propigate across the network
                }
                if (verboseLogging) Debug.Log($"<color=blue>Server</color> spawning: <color=magenta>{character.name}</color> across the network.");
                NetworkServer.Spawn(character, conn);    // instantiates this character across the network and makes it owned by the conn
                yield return new WaitForSeconds(0.001f); // wait for the spawn request to settle across the network
                
                ClientConnection clientConn = ClientUtil.GetClientConnection(conn.connectionId);
                if (clientConn)
                {
                    clientConn.playerCharacter = character;
                }

                if (_playerPrefabList.ContainsKey(conn))
                {
                    _playerPrefabList[conn] = character;
                }
                else
                {
                    // first time character has been spawned for this connection.
                    _playerPrefabList.Add(conn, character);

                    if (verboseLogging) Debug.Log($"<color=blue>Server</color> triggering <color=purple>JumpToScene</color> like it was called from <color=green>[connectionId={conn.connectionId}]</color> with data: <color=magenta>[PointType: {data.pointType}, PointName: {data.pointName}, SceneName: {data.sceneName}, TeamName: {data.teamName}]</color>");
                    ClientRequestedJumpToScene(conn, new JumpToScene
                    {
                        pointType = data.pointType,
                        sceneName = data.sceneName,
                        pointName = data.pointName,
                        teamName = data.teamName,
                        unloadScene = data.unloadScene
                    });
                }
            }
            else
            {
                Debug.LogError($"<color=blue>Server</color> failed to spawn: <color=magenta>{data.prefabName}</color> because this name was not found in the \"Registered Spawnable Prefabs\"");
            }
            #endif
            yield return null;
        }
        #endregion

        #region Debug Window
        protected virtual void Update()
        {
            if (debugWindow && Input.GetKeyDown(KeyCode.Escape))
            {
                if (Cursor.lockState == CursorLockMode.Locked || Cursor.lockState == CursorLockMode.Confined)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
        protected virtual void OnGUI()
        {
            if (debugWindow)
            {
                GUI.Window(3215, new Rect(Screen.width - 250, 0, 220, (NetworkClient.active) ? 250 : 140), NetworkManagerDebugWindow, "Network Manager");
            }
        }
        protected virtual void NetworkManagerDebugWindow(int windowId)
        {
            if (!NetworkServer.active)
            {
                if (!NetworkClient.active)
                {
                    if (GUI.Button(new Rect(10, 20, 200, 20), "Start Server & Client"))
                    {
                        StartHost();
                    }
                }
                if (GUI.Button(new Rect(10, 50, 200, 20), "Start Server Only"))
                {
                    StartServer();
                }
            }
            else
            {
                if (NetworkServer.active && NetworkClient.active)
                {
                    if (GUI.Button(new Rect(10, 20, 200, 20), "Stop Server + Client"))
                    {
                        StopClient();
                        StopServer();
                    }
                }
                if (GUI.Button(new Rect(10, 50, 200, 20), "Stop Server"))
                {
                    StopServer();
                }
            }
            if (!NetworkClient.active)
            {
                if (GUI.Button(new Rect(10, 80, 200, 20), "Join Server"))
                {
                    //networkAddress = GetIP(networkAddress);
                    StartClient();
                }
                networkAddress = GUI.TextField(new Rect(10, 110, 200, 20), networkAddress);
            }
            else
            {
                if (GUI.Button(new Rect(10, 80, 200, 20), "Leave Server"))
                {
                    StopClient();
                }
            }
        }
        #endregion
    }
}
