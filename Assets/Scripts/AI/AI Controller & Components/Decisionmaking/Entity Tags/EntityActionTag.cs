using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    //this tags are added automaticly by decisions & the ai controller

  

    [System.Serializable]
    public class EntityActionTag
    {
        public enum Type
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

        public Type type;
        public GameEntity shootAtTarget;

        public EntityActionTag(Type type)
        {
            this.type = type;
        }
    }
}


