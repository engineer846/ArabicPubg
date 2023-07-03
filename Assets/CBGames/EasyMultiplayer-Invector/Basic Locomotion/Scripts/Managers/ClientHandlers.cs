using Mirror;

namespace EMI.Managers
{
    public struct ClientJumpToScene : NetworkMessage
    {
        public Utils.PointsUtil.PointType pointType;
        public string sceneName;
        public string pointName;
        public string teamName;
        public int connectionId;
        public string unloadScene;
    }
}
