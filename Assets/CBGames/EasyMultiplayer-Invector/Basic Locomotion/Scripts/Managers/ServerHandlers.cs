using Mirror;

namespace EMI.Managers
{
    public struct JumpToScene : NetworkMessage
    {
        public Utils.PointsUtil.PointType pointType;
        public string sceneName;
        public string pointName;
        public string teamName;
        public string unloadScene;
    }

    public struct SpawnPlayer : NetworkMessage
    {
        public string prefabName;
        public Utils.PointsUtil.PointType pointType;
        public string sceneName;
        public string pointName;
        public string teamName;
        public string unloadScene;
    }

    public struct FinishedMoving : NetworkMessage
    {
        public string sceneName;
    }
}
