using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class EnvironmentalDangerTag : MonoBehaviour
    {
        public enum DangerType
        {
            Grenade,
            Fire
        }

        public DangerType dangerType;
        public float dangerLevel = 1;
        public bool dangerActive = false;


    }
}
