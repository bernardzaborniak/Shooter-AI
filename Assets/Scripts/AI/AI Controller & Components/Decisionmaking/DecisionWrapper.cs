using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [System.Serializable]
    public class DecisionWrapper
    {
        [HideInInspector]
        public string name = "name";
        public Decision decision;
        [Min(0)]
        public float weigt = 1f;
    }
}