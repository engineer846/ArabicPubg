using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMI.Managers
{
    public class Delegates
    {
        public delegate void GameObjectDelegate(GameObject target);
        public delegate void StringDelegate(string value);
        public delegate void BoolDelegate(bool value);
        public delegate void IntDelegate(int value);
        public delegate void FloatDelegate(float value);
        public delegate void DoubleDelegate(double value);
        public delegate void VoidDelegate();
    }
}
