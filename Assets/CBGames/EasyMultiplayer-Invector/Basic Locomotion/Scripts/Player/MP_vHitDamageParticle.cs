using EMI.Object;
using EMI.Utils;
using UnityEngine;

namespace Invector
{
    public class MP_vHitDamageParticle : vHitDamageParticle
    {
        [SerializeField, Tooltip("If you want to display self damage or not.")]
        protected bool displaySelfDamage = false;
        protected Team team = null;
        protected virtual void Awake()
        {
            team = (Team)gameObject.FindComponent(typeof(Team));
        }

        public override void OnReceiveDamage(vDamage damage)
        {
            // check teams don't show damage on friendly (if team component available)
            if (team == null || damage.sender == null || !damage.sender.transform.root.GetComponent<Team>() || (
                damage.sender.transform.root.GetComponent<Team>() &&
                damage.sender.transform.root.GetComponent<Team>().IsEnemy(team.teamName)))
            {
                // prevent self damage (if enabled)
                if (!displaySelfDamage && damage.sender != null && damage.sender.transform.root.gameObject.Equals(transform.root.gameObject))
                    return;

                // past all checks (EX: Is enemy team and not damaging myself)
                base.OnReceiveDamage(damage);
            }
        }
    }
}
