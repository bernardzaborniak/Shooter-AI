using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace BenitosAI
//{
    //added to thew tags by the designer in edit mode, this tags inform other entities about what kind of threat this unit poses
    [System.Serializable]
    public class EntityThreatTag 
    {
        public enum Type
        {
            SoldierWithGun,
            FireDamage 
            //etc...
        }

        public Type type;

        public EntityThreatTag(Type type)
        {
            this.type = type;
        }
    }
//}
