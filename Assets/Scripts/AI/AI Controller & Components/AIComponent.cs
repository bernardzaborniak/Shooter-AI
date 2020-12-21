using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class AIComponent : MonoBehaviour
    {
        protected GameEntity myEntity;

        public virtual void SetUpComponent(GameEntity entity)
        {
            myEntity = entity;
        }

        public virtual void UpdateComponent()
        {

        }
    }

}
