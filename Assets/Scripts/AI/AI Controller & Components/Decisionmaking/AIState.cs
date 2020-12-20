using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    public enum AIStateEnum
    {
        TestState
    }



    public abstract class AIState
    {
        //public abstract void SetUpState(AIController aiController);
        public abstract void SetUpState(AIController aiController);

        public abstract void OnStateEnter();

        public abstract void OnStateExit();

        public abstract void UpdateState();


    }

    public abstract class AIState_HumanoidSoldier : AIState
    {
        protected AIController_HumanoidSoldier aiController;

        public override void SetUpState(AIController aiController)
        {
            this.aiController = (AIController_HumanoidSoldier)aiController;
        }

        //public override void OnStateEnter()
        //{

        //}


        //public override abstract void OnStateExit();


        //public override abstract void OnStateUpdate();

    }

    public class AIst_HumSol_MovingToZeroPoint : AIState_HumanoidSoldier
    {
        // AIController aiController;

        /*public override void SetUpState(AIController aiController) 
        {
            this.aiController = aiController;
        }*/

        public override void OnStateEnter() { }

        public override void OnStateExit() { }

        public override void UpdateState() { }
    }

}
