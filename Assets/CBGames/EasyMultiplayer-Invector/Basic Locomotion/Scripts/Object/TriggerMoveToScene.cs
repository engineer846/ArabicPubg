using EMI.Managers;
using UnityEngine;
using EMI.Utils;
using Mirror;

namespace EMI.Objects
{
    public class TriggerMoveToScene : MonoBehaviour
    {
        [SerializeField, Tooltip("If you want to unload this scene when moving to the new one.")]
        protected bool unloadThisScene = true;
        [SerializeField, Tooltip("The scene you want to travel to."), Scene]
        protected string sceneToTravelTo = "TravelToExample";
        [SerializeField, Tooltip("If you want to send all clients connected to the server to this scene.")]
        protected bool sendAllClients = false;
        [SerializeField, Tooltip("The point name to jump to in the new scene.")]
        protected string pointName = "";
        [SerializeField, Tooltip("The type of point with the above name to jump to.")]
        protected PointsUtil.PointType pointType = PointsUtil.PointType.JumpToPoint;
        [SerializeField, Tooltip("If you want to only jump to a point that is a member of this team. Otherwise leave this blank to look for any point with the above settings.")]
        protected string teamName = "";

        protected virtual void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (sendAllClients || (!sendAllClients && col.gameObject.GetComponent<NetworkIdentity>().hasAuthority))
                {
                    EMI_NetworkManager.instance.RequestJumpToScene(
                        pointType,
                        sceneToTravelTo.GetCleanSceneName(),
                        pointName,
                        teamName,
                        (unloadThisScene) ? gameObject.scene.name.GetCleanSceneName() : ""
                    );
                }
            }
        }
    }
}
