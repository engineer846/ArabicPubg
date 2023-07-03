using EMI.Object;
using EMI.Player;
using Mirror;
using System.Collections;
using UnityEngine;

namespace Invector.vShooter
{
    public class MP_vShooterWeapon : vShooterWeapon
    {
        #region Properties
        protected vShooterWeaponNetworkCalls swnc = null;
        public bool i_ammo, i_clipSize, i_shootFrequency, i_minDmgDist, i_maxDmgDist, i_minDmg, i_maxDmg = false;
        public override int ammo 
        {
            get 
            {
                if (swnc && swnc.hasAuthority && NetworkClient.active)
                {
                    if (_ammo < 5)
                    {
                        return swnc.ammo;
                    }
                    else
                    {
                        return _ammo;
                    }
                }
                else if (swnc)
                {
                    #if UNITY_SERVER || UNITY_EDITOR
                    if (!i_ammo && NetworkServer.active) // initiliaze starting ammo across the newtork
                    {
                        ammo = _ammo;
                        i_ammo = true;
                        changeAmmoHandle?.Invoke(_ammo);
                        return _ammo;
                    }
                    #endif
                    return swnc.ammo;
                }
                else
                {
                    return _ammo;
                }
            } 
            set 
            {
                _ammo = value;
                if (NetworkClient.active && swnc && swnc.hasAuthority)
                {
                    StartCoroutine(VerifyAmmo());
                }
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    if (swnc)
                    {
                        swnc.ammo = value;
                    }
                    else
                    {
                        StartCoroutine(SetAmmo(value));
                    }
                }
                #endif
            } 
        }
        public override int clipSize 
        { 
            get 
            {
                if (swnc)
                {
                    #if UNITY_SERVER || UNITY_EDITOR
                    if (!i_clipSize && NetworkServer.active)
                    {
                        clipSize = _clipSize;
                        i_clipSize = true;
                        return _clipSize;
                    }
                    #endif
                    return swnc.clipSize;
                }
                else
                {
                    return _clipSize;
                }
            } 
            set 
            { 
                _clipSize = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active)
                {
                    if (swnc)
                    {
                        swnc.clipSize = value;
                    }
                    else
                    {
                        StartCoroutine(SetClipSize(value));
                    }
                }
                #endif
            } 
        }
        public override float shootFrequency 
        { 
            get 
            {
                if (swnc)
                {
                    #if UNITY_SERVER || UNITY_EDITOR
                    if (!i_shootFrequency && NetworkServer.active)
                    {
                        shootFrequency = _shootFrequency;
                        i_shootFrequency = true;
                        return _shootFrequency;
                    }
                    #endif
                    return swnc.shootFrequency;
                }
                else
                {
                    return _shootFrequency;
                }
            } 
            set 
            { 
                _shootFrequency = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && swnc)
                {
                    swnc.shootFrequency = value;
                }
                #endif
            } 
        }
        public override float minDamageDistance 
        { 
            get 
            {
                if (swnc)
                {
                    #if UNITY_SERVER || UNITY_EDITOR
                    if (!i_minDmgDist && NetworkServer.active)
                    {
                        minDamageDistance = _minDamageDistance;
                        i_minDmgDist = true;
                        return _minDamageDistance;
                    }
                    #endif
                    return swnc.minDamageDistance;
                }
                else
                {
                    return _minDamageDistance;
                }
            } 
            set 
            { 
                _minDamageDistance = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && swnc)
                {
                    swnc.minDamageDistance = value;
                }
                #endif
            } 
        }
        public override float maxDamageDistance 
        { 
            get 
            {
                if (swnc)
                {
                    #if UNITY_SERVER || UNITY_EDITOR
                    if (!i_maxDmgDist && NetworkServer.active)
                    {
                        maxDamageDistance = _maxDamageDistance;
                        i_maxDmgDist = true;
                        return _maxDamageDistance;
                    }
                    #endif
                    return swnc.maxDamageDistance;
                }
                else
                {
                    return _maxDamageDistance;
                }
            } 
            set 
            { 
                _maxDamageDistance = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && swnc)
                {
                    swnc.maxDamageDistance = value;
                }
                #endif
            } 
        }
        public override int minDamage 
        { 
            get 
            {
                if (swnc)
                {
                    #if UNITY_SERVER || UNITY_EDITOR
                    if (!i_minDmg && NetworkServer.active)
                    {
                        minDamage = _minDamage;
                        i_minDmg = true;
                        return _minDamage;
                    }
                    #endif
                    return swnc.minDamage;
                }
                else
                {
                    return _minDamage;
                }
            } 
            set 
            { 
                _minDamage = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && swnc)
                {
                    swnc.minDamage = value;
                }
                #endif
            } 
        }
        public override int maxDamage 
        { 
            get 
            {
                if (swnc)
                {
                    #if UNITY_SERVER || UNITY_EDITOR
                    if (!i_maxDmg && NetworkServer.active)
                    {
                        maxDamage = _maxDamage;
                        i_maxDmg = true;
                        return _maxDamage;
                    }
                    #endif
                    return swnc.maxDamage;
                }
                else
                {
                    return _maxDamage;
                }
            } 
            set 
            { 
                _maxDamage = value;
                #if UNITY_SERVER || UNITY_EDITOR
                if (NetworkServer.active && swnc)
                {
                    swnc.maxDamage = value;
                }
                #endif
            } 
        }
        #endregion

        #region Initilization
        protected override void Start()
        {
            swnc = GetComponent<vShooterWeaponNetworkCalls>();
            base.Start();
        }
        #endregion

        #region Overrides
        public override void SetActiveAim(bool value)
        {
            ShooterNetworkCalls nc = GetComponentInParent<ShooterNetworkCalls>();
            if (nc != null && !nc.hasAuthority)
            {
                foreach (Camera cam in GetComponentsInChildren<Camera>(true))
                {
                    cam.gameObject.SetActive(false);
                }
            }
            base.SetActiveAim(value);
        }
        public override void SetScopeZoom(float value)
        {
            if (swnc == null || (!NetworkServer.active && !NetworkClient.active) || (swnc != null && swnc.hasAuthority && NetworkClient.active))
                base.SetScopeZoom(value);
        }
        #endregion

        #region Additions
        [Server]
        protected virtual IEnumerator SetAmmo(int ammo)
        {
            yield return new WaitUntil(() => swnc != null);
            swnc.ammo = ammo;
        }
        [Server]
        protected virtual IEnumerator SetClipSize(int size)
        {
            yield return new WaitUntil(() => swnc != null);
            swnc.clipSize = size;
        }
        [Client]
        protected virtual IEnumerator VerifyAmmo()
        {
            yield return new WaitForSeconds(0.001f);
            if (_ammo != swnc.ammo)
            {
                ammo = swnc.ammo;
                changeAmmoHandle?.Invoke(ammo);
            }
        }
        [Server]
        public virtual int GetInternalAmmo()
        {
            return _ammo;
        }
        [Server]
        public virtual int GetInternalClipSize()
        {
            return _clipSize;
        }
        [Server]
        public virtual float GetInternalShootFrequency()
        {
            return _shootFrequency;
        }
        [Server]
        public virtual float GetInternalMinDamageDistance()
        {
            return _minDamageDistance;
        }
        [Server]
        public virtual float GetInternalMaxDamageDistance()
        {
            return _maxDamageDistance;
        }
        [Server]
        public virtual int GetInternalMinDmage()
        {
            return _minDamage;
        }
        [Server]
        public virtual int GetInternalMaxDamage()
        {
            return _maxDamage;
        }
        #endregion
    }
}
