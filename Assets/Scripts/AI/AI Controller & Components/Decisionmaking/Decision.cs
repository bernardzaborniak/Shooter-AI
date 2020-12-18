using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decision : MonoBehaviour
{
    //every decision has a list of considerations based on which to decide
    [SerializeField] Consideration[] considerations;
    //how to solve one decision that has to seperate into several ones - if there are more enemies to go to - we need this decision to seperate into x decisions. where x is the number of enemies

    //also has its own codestate initialised on start and passed to the decision layer, if decision was selected

    public AIStateEnum correspondingAIState;

    AIController aiController;
    //AIState state;
    //several decisions can have the same state?
    /*public AIState GetCorrespondingState()
    {
        return state;
    }*/

    public void SetUpDecision(AIController aiController)
    {
        this.aiController = aiController;

        for (int i = 0; i < considerations.Length; i++)
        {
            considerations[i].SetUpConsideration(aiController);
        }
    }

    public AIState OnDecisionWasSelected()
    {
        AIState aIState = new AIst_HumSol_MovingToZeroPoint();
        aIState.SetUpState(aiController);
        return aIState;
    }

    public float RateDecision()
    {
        return 0;
    }


    /*void Start()
    {
        //instantiate the corresponding state
       // if(correspondingAIState == AIStateEnum.TestState)
        //{
         //   state = new AITestState();
       // }
    }

    void Update()
    {
        
    }*/
}
