using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    //cuzstom clas inside decision

    [System.Serializable]
    public class AIStateCreatorInputParams //: ScriptableObject //has to be SO so it can be serialized properly
    {
        //move the declaration of this enum somewhere else? - or its good here?
        public enum InputParamsType
        {
           // None,
            GoToTp,
            Sprint,
            CharacterStance,
            HoldWeaponScanForThreat,
            MaxAimingDeviationAngle,
            WeaponID,
            Position1,
            Position2,
            LineOfFireCheck,
            Transform1
            // Position,
            //Color
        }

        //public InputParamsType inputParamsType;

        // GoToTP  
        [Tooltip("Testing tooltip")]
        public float enterTPDistance = 0.7f;
        public float exitTPDistance = 1;

        // Sprint
        public bool sprint;
       
        // Character Stance
        public EC_HumanoidCharacterController.CharacterStance characterStance;

        // Hold Weapon Scan for Threat
        public float minChangeAimDirInterval;
        public float maxChangeAimDirInterval;

        // Max Aiming Deviation Angle
        public float maxAimingDeviationAngle;

        // WeaponID
        public int weaponID;

        // Position 1
        public Vector3 position1;

        // Position 2
        public Vector3 position2;

        // Line of Fire check
        [Tooltip("Every x Seconds a line of fire raycastr is send, to check if there is nothing obstructing the shooting")]
        public float checkLineOfFireInterval;
        public LayerMask checkLineOfFireLayerMask;

        // Transform
        public Transform transform1;

    }

}

