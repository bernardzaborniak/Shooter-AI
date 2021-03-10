using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [System.Serializable]
    public class ConsiderationInputParams 
    {
        public enum InputParamsType
        {
            Range,
            DesiredFloatValue, 
            WeaponID,
            Direction, //usefull for things like prioritise targets in front of me ? - not used yet
            LineOfFire,
            LineOfSight,
            InformationFreshness
        }

        // Range
        public float min;
        public float max;

        // Desired Float Value
        public float desiredFloatValue;

        // WeaponID
        public int weaponID;

        //Direction
        public Vector3 direction;

        // Line of Fire
        public LayerMask lineOfFireLayerMask;

        // Line of Sight
        public LayerMask lineOfSightLayerMask;

        // Information Freshness
        [Tooltip("most of times used in this context: If the information is older than x seconds, ignore it")]
        public float informationFreshnessThreshold;
    }
}


