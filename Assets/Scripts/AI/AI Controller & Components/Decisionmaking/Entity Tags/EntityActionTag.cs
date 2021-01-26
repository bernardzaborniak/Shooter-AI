using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    public enum EntityActionTagType
    {
        //Soldier Positionin
        /* Idle,
         Sprinting,
         Walking,
         InCoverHiding,
         InCoverPeeking,
         //Soldier Combat & Interaction
         InteractionIdle,
         ChangingWeapon,
         ReloadingWeapon,
         ShootingAt,
         ThrowingGrenade
         //other actions for other units will be listed here too...*/

        ShootingAtTarget,
        ReloadingWeapon,
        ThrowingGrenade

    }

    [System.Serializable]
    public class EntityActionTag
    {
        public EntityActionTagType type;
        public GameEntity shootAtTarget;

        public EntityActionTag(EntityActionTagType type)
        {
            this.type = type;
        }
    }
}


