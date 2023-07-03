using UnityEngine;
using Mirror;
using EMI.Utils;

namespace EMI.Object
{
    /// <summary>
    /// Use this to syncronize your specified settings with the server on each client.
    /// </summary>
    public class ServerSync : NetworkBehaviour
    {
        #region Properties

        #region Modifiables
        [Tooltip("How fast you want to reconcile(move) from your position to the received server position.\n0 = Very fast movement, 0.5 = Very Slow Movement")]
        [SerializeField, Range(0, 0.5f)] protected float interpolation = 0.5f;
        [Tooltip("How far off you have to be from the server position before you just snap to it forcefully.")]
        [SerializeField, Range(0, 10)] protected float forceSnapDistance = 2.0f;
        [Tooltip("If you want to smoothly changes values to the server scale value when it updates. If false, " +
            "will snap to the new scale value")]
        [SerializeField] protected bool smoothScaling = false;
        
        [Tooltip("If you want to syncronize your position with the server")]
        [SerializeField] protected bool position = true;
        [Tooltip("If you want to syncronize your rotation with the server")]
        [SerializeField] protected bool rotation = true;
        [Tooltip("If you want to syncronize your scale with the server")]
        [SerializeField] protected bool scale = true;
        #endregion

        #region Internal
        protected bool _serverUpdate = false;
        protected Vector3 _serverPos, _serverScale, _prevServerPos, _prevServerScale = Vector3.zero;
        protected Quaternion _serverRot, _prevServerRot = Quaternion.identity;
        protected float _reconcilePosSpeed, _reconcileRotSpeed, _reconcileScaleSpeed = 0;
        protected float _syncTimer = 0;
        protected bool _isEnabled = true;
        #endregion
        #endregion

        #region Initialization
        protected virtual void Start()
        {
            if (NetworkServer.active)
                SetSyncVarDirtyBit(1UL); //Immediately sync all your settings with the clients as soon as this object is initialized.
        }
        #endregion

        #region State Serialization
        public virtual Vector3 GetServerPosition()
        {
            return _serverPos;
        }
        public virtual Quaternion GetServerRotation()
        {
            return _serverRot;
        }
        /// <summary>
        /// Only called by the server to send updates to all clients.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="initialState"></param>
        /// <returns></returns>
        [Server]
        public override void OnSerialize(NetworkWriter writer, bool initialState)
        {
            //bool sendChanges = base.OnSerialize(writer, initialState); //Capture any syncvars, synclists, syncdicts that have changed and send them (if any)
            base.OnSerialize(writer, initialState);
            if (initialState)
            {
                // Only write changes you want sync'd - immediately
                if (position) writer.WriteVector3(transform.position);
                if (rotation) writer.WriteQuaternion(transform.rotation);
                if (scale) writer.WriteVector3(transform.localScale);
                //return true;
            }
            else
            {
                // Only write changes you want sync'd - only when SetDirtyBit = 1UL
                if (position) writer.WriteVector3(transform.position);
                if (rotation) writer.WriteQuaternion(transform.rotation);
                if (scale) writer.WriteVector3(transform.localScale);
                if (NetworkServer.active)
                {
                    _serverPos = transform.position; 
                    _serverRot = transform.rotation; 
                    _serverScale = transform.localScale;
                }
                //return sendChanges && _isEnabled;
            }
        }

        /// <summary>
        /// Called on all clients automatically when it detects that a server has sent an update.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="initialState"></param>
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);
            if (position == true)
            {
                _serverPos = reader.ReadVector3(); // Store the retreived server position for later processing
            }
            if (rotation == true)
            {
                _serverRot = reader.ReadQuaternion(); // Store the retreived server rotation for later processing
            }
            if (scale == true)
            {
                _serverScale = reader.ReadVector3(); // Store the retreived server scale for later processing
            }
            SetReconcilationSpeeds(); // Tell the client how fast to move to the received server data
            _serverUpdate = true; // Tell the client to process the received server changes
        }
        #endregion

        #region Reconcilation Settings
        /// <summary>
        /// Prior to updating position, rotation, scale on this object this tells the object
        /// how fast to update these values based on their current offset from these values.
        /// </summary>
        [Client]
        protected virtual void SetReconcilationSpeeds()
        {
            float pastData = syncInterval + interpolation;

            if (position)
            {
                _reconcilePosSpeed = ((_serverPos - transform.position).sqrMagnitude) / pastData;
            }
            if (rotation)
            {
                _reconcileRotSpeed = Quaternion.Angle(transform.rotation, _serverRot) / pastData;
            }
            if (scale)
            {
                _reconcileScaleSpeed = ((_serverScale - transform.localScale).sqrMagnitude) / pastData;
            }
        }
        #endregion

        #region Heartbeat
        #region Client Methods
        protected virtual void FixedUpdate()
        {
            if (NetworkClient.active && !NetworkServer.active && _isEnabled)
                UpdateClient();
        }
        /// <summary>
        /// Update the client object position, rotation, and scale to match the servers.
        /// </summary>
        [Client]
        protected virtual void UpdateClient()
        {
            if (!NetworkClient.active || NetworkServer.active) return;
            if (_serverUpdate)
            {
                if (UpdateClientPosition() && UpdateClientRotation() && UpdateClientScale())
                    _serverUpdate = false;
            }
        }

        /// <summary>
        /// Smoothly reconcile the players objects with the received server position.
        /// </summary>
        /// <returns>Has finished it reconcilation cycle.</returns>
        [Client]
        protected virtual bool UpdateClientPosition()
        {
            if (!position) return true;
            if ((_serverPos - transform.position).sqrMagnitude > forceSnapDistance) // The client is too far, force them to the right position
            {
                transform.position = _serverPos;
                return true;
            }
            else
            {
                // Smoothly reconcile the clients position to the servers
                transform.position = Vector3.MoveTowards(transform.position, _serverPos, _reconcilePosSpeed * Time.unscaledDeltaTime);
                return transform.position.CloseEnough(_serverPos); // Keep doing this until they're close enough
            }
        }
        /// <summary>
        /// Smoothly reconcile the objects rotation with the servers rotation
        /// </summary>
        /// <returns>true, when reconcilation is done</returns>
        [Client]
        protected virtual bool UpdateClientRotation()
        {
            if (!rotation) return true;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _serverRot, _reconcileRotSpeed * Time.unscaledDeltaTime);
            return transform.rotation.CloseEnough(_serverRot, 0);
        }
        
        /// <summary>
        /// Smoothly or snap reconcile the objects scale with the servers scale
        /// </summary>
        /// <returns></returns>
        [Client]
        protected virtual bool UpdateClientScale()
        {
            if (!scale) return true;
            if (smoothScaling)
            {
                transform.localScale = Vector3.MoveTowards(transform.localScale, _serverScale, _reconcileScaleSpeed * Time.unscaledDeltaTime);
                return transform.localScale.CloseEnough(_serverScale, 0);
            }
            else //Don't smoothly update values, just snap to the new value.
            {
                transform.localScale = _serverScale;
                return true;
            }
        }
        #endregion

        #region Server Methods
        protected virtual void Update()
        {
            if (NetworkServer.active && _isEnabled)
                ServerSendUpdates();
        }

        /// <summary>
        /// Server only method that will send updates to all clients based on the sync interval 
        /// and when it has processes any changes in your desired values.
        /// </summary>
        [Server]
        protected virtual void ServerSendUpdates()
        {
            if (!NetworkServer.active) return; // Only send updates if your the server
            _syncTimer += Time.unscaledDeltaTime;
            if (_syncTimer >= syncInterval)
            {
                if (HasProcessedUpdates()) // Has the servers position, rotation, or scale changes (based on your desired watch values?)
                {
                    _syncTimer = 0; //Reset the counter
                    SetSyncVarDirtyBit(1UL); //Send updates to all clients
                }
                else
                {
                    _syncTimer = syncInterval; //Prevent a stack overflow from numbers counting too high
                }
            }
        }
        [Server]
        protected virtual bool HasProcessedUpdates()
        {
            if ((position && _prevServerPos != transform.position) ||
                (rotation && _prevServerRot != transform.rotation) ||
                (scale && _prevServerScale != transform.localScale))
            {
                _prevServerPos = transform.position;
                _prevServerRot = transform.rotation;
                _prevServerScale = transform.localScale;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #endregion

        #region Enabling/Disabling
        /// <summary>
        /// You can't just disable this component because it will throw network errors.
        /// This is a way to disable/enable this component correctly.
        /// </summary>
        /// <param name="enabled"></param>
        public virtual void Enable(bool enabled)
        {
            _isEnabled = enabled;
            if (_isEnabled == true && !NetworkServer.active)
            {
                if (position) transform.position = _serverPos;
                if (rotation) transform.rotation = _serverRot;
                if (scale) transform.localScale = _serverScale;
            }
        }
        public virtual void SmoothEnable()
        {
            _isEnabled = true;
        }
        #endregion

        #region Last State
        public virtual Vector3 GetLastServerPosition()
        {
            return _serverPos;
        }
        public virtual Quaternion GetLastServerRotation()
        {
            return _serverRot;
        }
        public virtual Vector3 GetLastServerScale()
        {
            return _serverScale;
        }
        #endregion
    }
}
