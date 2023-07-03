using EMI.Player;
using EMI.Object;
using Mirror;
using UnityEngine;
using EMI.Utils;
using EMI.Managers;

namespace Invector.vCharacterController
{
    public class MP_vThirdPersonController : vThirdPersonController
    {
        #region Properties
        public Delegates.FloatDelegate onMaxHealthChanged;
        protected BasicNetworkCalls nc = null;
        [SerializeField, Tooltip("Advanced users only. If you want to allow damaging yourself or not"), vEditorToolbar("Health")]
        protected bool preventSelfDamage = true;
        public override bool ragdolled
        {
            get
            {
                return _ragdolled;
            }
            set
            {
                if (NetworkServer.active && _ragdolled != value)
                {
                    _ragdolled = value;
                    if (nc) nc.ragdolled = value;
                }
            }
        }
        public override bool isStrafing
        {
            get
            {
                if ((NetworkServer.active || NetworkClient.active) && nc)
                {
                    return sprintOnlyFree && isSprinting ? false : nc.isStrafing;
                }
                else
                {
                    return sprintOnlyFree && isSprinting ? false : _isStrafing;
                }
            }
            set
            {
                _isStrafing = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.isStrafing = _isStrafing;
                }
                #endif
            }
        }

        #region Health
        public override int MaxHealth
        {
            get
            {
                return maxHealth;
            }
            protected set
            {
                maxHealth = value;
                onMaxHealthChanged?.Invoke(maxHealth);
            }
        }
        public override float currentHealth
        {
            get
            {
                if ((NetworkServer.active || NetworkClient.active) && nc)
                {
                    return nc.currentHealth;
                }
                else
                {
                    return _currentHealth;
                }
            }
            protected set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    onChangeHealth.Invoke(_currentHealth);
                }
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.currentHealth = value; // update all other clients to match server
                }
                #endif

                if (!_isDead && _currentHealth <= 0)
                {
                    if (((NetworkServer.active || NetworkClient.active) && nc && nc.currentHealth <= 0) ||
                        (!NetworkClient.active && !NetworkServer.active) && currentHealth <= 0)
                    {
                        isDead = true;
                    }
                }
                else if (isDead && (((NetworkServer.active || NetworkClient.active) && nc && nc.currentHealth <= 0) ||
                        (!NetworkClient.active && !NetworkServer.active) && currentHealth <= 0))
                {
                    isDead = false;
                }
            }
        }
        public override bool isDead
        {
            get
            {
                return _isDead;
            }
            set
            {
                _isDead = value;
                if (_isDead)
                    onDead.Invoke(gameObject);
                if (((NetworkClient.active || NetworkServer.active) && nc && nc.isDead) ||
                        (!NetworkClient.active && !NetworkServer.active))
                {
                    _isDead = true;
                }
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && nc)
                {
                    nc.isDead = value; // update all other clients to match server
                }
                #endif
            }
        }
        #endregion
        #endregion

        #region Initilization
        protected virtual void Awake()
        {
            RegisterAnimatorStateInfos(); // adding this here to prevent late joiners from getting error if someone is already in a dead state
        }
        protected override void Start()
        {
            nc = GetComponent<BasicNetworkCalls>();
            base.Start();
        }
        #endregion

        #region Team Damaging
        /// <summary>
        ///  Override the damage to work properly with multiplayer team based mechanics.
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(vDamage damage)
        {
            Debug.Log(damage);
            if (preventSelfDamage && damage.sender != null && damage.sender.gameObject && damage.sender.transform.root.Equals(gameObject)) return;
            Team myTeam = GetComponent<Team>();
            if (!GetComponent<Team>())
            {
                base.TakeDamage(damage);
                TriggerDamageReaction(damage);
            }
            else
            {
                Team senderTeam = null;
                if (damage.sender != null)
                    senderTeam = (Team)damage.sender.gameObject.FindComponent(typeof(Team));
                if (senderTeam == null || senderTeam.IsEnemy(myTeam.teamName))
                {
                    base.TakeDamage(damage);
                    TriggerDamageReaction(damage);
                }
            }
        }
        #endregion
    }
}
