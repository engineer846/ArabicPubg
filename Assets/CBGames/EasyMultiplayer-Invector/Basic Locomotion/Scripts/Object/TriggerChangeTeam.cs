using Invector.vCharacterController;
using Mirror;
using System.Collections;
using UnityEngine;

namespace EMI.Object
{
    public class TriggerChangeTeam : MonoBehaviour
    {
        [SerializeField, Tooltip("The team name to change the entering object to.")]
        protected string newTeamName = "";
        [SerializeField, Tooltip("Execute on Trigger Enter")]
        protected bool onEnter = true;
        [SerializeField, Tooltip("Execute on Trigger Exit")]
        protected bool onExit = false;
        protected bool canShowMessage = true;
        protected virtual void OnTriggerEnter(Collider col)
        {
            if (!onEnter) return;
            if (col.transform.root.GetComponent<Team>() && !string.IsNullOrEmpty(newTeamName))
            {
                // change the colliders team
                col.transform.root.GetComponent<Team>().ChangeTeam(newTeamName);

                // if HUD Controller is around and this is the owner, show a change team message
                if (canShowMessage && FindObjectOfType<vHUDController>() && col.transform.root.GetComponent<NetworkIdentity>() && col.transform.root.GetComponent<NetworkIdentity>().hasAuthority && NetworkClient.active)
                {
                    canShowMessage = false;
                    FindObjectOfType<vHUDController>().ShowText($"You are now on the {newTeamName} team!");
                    StartCoroutine(ResetMessage());
                }
            }
        }

        protected virtual void OnTriggerExit(Collider col)
        {
            if (!onExit) return;
            if (col.transform.root.GetComponent<Team>() && !string.IsNullOrEmpty(newTeamName))
            {
                // change the colliders team
                col.transform.root.GetComponent<Team>().ChangeTeam(newTeamName);

                // if HUD Controller is around and this is the owner, show a change team message
                if (canShowMessage && FindObjectOfType<vHUDController>() && col.transform.root.GetComponent<NetworkIdentity>() && col.transform.root.GetComponent<NetworkIdentity>().hasAuthority && NetworkClient.active)
                {
                    canShowMessage = false;
                    FindObjectOfType<vHUDController>().ShowText($"You are now on the {newTeamName} team!");
                    StartCoroutine(ResetMessage());
                }
            }
        }

        protected virtual IEnumerator ResetMessage()
        {
            yield return new WaitForSeconds(1);
            canShowMessage = true;
        }
    }
}
