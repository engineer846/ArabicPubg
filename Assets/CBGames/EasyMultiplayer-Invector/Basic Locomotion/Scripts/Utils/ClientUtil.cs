using EMI.Managers;
using EMI.Player;
using Invector.vCharacterController;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EMI.Utils
{
    public static class ClientUtil
    {
        /// <summary>
        /// This will return the "ClientConnection" component that you own.
        /// </summary>
        /// <returns>The ClientConnection component.</returns>
        public static ClientConnection GetMyClientConnectionObject()
        {
            return GameObject.FindObjectsOfType<ClientConnection>().ToList().Find(x => x.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer == true || x.gameObject.GetComponent<NetworkIdentity>().hasAuthority == true);
        }

        /// <summary>
        /// This will return the "vThirdPersonController" component that you own. Does not work if you're the server.
        /// </summary>
        /// <returns></returns>
        public static vThirdPersonController GetMyCharacterControllerObject()
        {
            return GameObject.FindObjectsOfType<vThirdPersonController>().ToList().Find(x => x.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer == true || x.gameObject.GetComponent<NetworkIdentity>().hasAuthority == true);
        }

        /// <summary>
        /// Will return the client connection component that matches this connection id.
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        public static ClientConnection GetClientConnection(int connId)
        {
            if (NetworkServer.active)
                return GameObject.FindObjectsOfType<ClientConnection>().ToList().Find(x => x.GetComponent<NetworkIdentity>().connectionToClient.connectionId == connId);
            else
                return GameObject.FindObjectsOfType<ClientConnection>().ToList().Find(x => x.connId == connId);
        }

        /// <summary>
        /// Will return the client connection component that matches the passed in player gameobject
        /// </summary>
        /// <param name="connId"></param>
        /// <returns></returns>
        public static ClientConnection GetClientConnection(GameObject player)
        {
            return GameObject.FindObjectsOfType<ClientConnection>().ToList().Find(x => x.playerCharacter.Equals(player));
        }
    }
}
