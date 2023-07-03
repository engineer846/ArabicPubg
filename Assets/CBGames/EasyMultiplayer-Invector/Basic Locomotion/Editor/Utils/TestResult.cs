using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Editors
{
    [System.Serializable]
    public struct TestResult
    {
        public string compName;
        public TestResultType result;
        public string details;
        public GameObject target;
        public string sceneName;
    }

    [System.Serializable]
    public enum TestResultType { Pass, Warning, Critical }
}
