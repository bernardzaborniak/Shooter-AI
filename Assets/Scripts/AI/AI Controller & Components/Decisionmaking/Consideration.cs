using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{

    public class Consideration : ScriptableObject
    {

        public string description;

        // Example Values to make setting up the curves easier for more complex relationships like squared distances
        public float exampleInput1 = 0;
        public float exampleInput2 = 0.25f;
        public float exampleInput3 = 0.5f;
        public float exampleInput4 = 0.75f;
        public float exampleInput5 = 1f;

        public float exampleOutput1;
        public float exampleOutput2;
        public float exampleOutput3;
        public float exampleOutput4;
        public float exampleOutput5;

        //Inputs needed:

        //character data
        //mine & targets

        //Game Engine Data
        //distance between 
        //elapsed time

        //other systems in the world

        //each of this is predefined -> put them in a dropdown list

        //public ConsiderationInput_HumanoidSoldier considerationInput;
        public ConsiderationInput considerationInput;
        float input;

        //for ConsiderationInput_HumanoidSoldier_DistanceToClosestEnemy
        public float min;
        [HideInInspector]
        public float minSquared;

        public float max;
        [HideInInspector]
        public float maxSquared;


        public CustomCurve considerationCurve;


        private void OnValidate()
        {
            considerationCurve.UpdateCurveVisualisationKeyframes();
        }

        //private void Awake()
        private void OnEnable()
        {
            minSquared = min * min;
            maxSquared = max * max;
        }

        public float GetConsiderationRating(DecisionContext context)
        {
            //Get Input, already normalized by ConsideraionInput
            input = considerationInput.GetConsiderationInput(context, this);

            //Debug.Log("Consideration: Ijnput: " + input);

            //TODo Refactor, dont save input into another variable, write it directly into the return statement

            //Debug.Log("Consideration: Remapped Curve: " + considerationCurve.GetRemappedValue(input));

            //Modify input by curve, curve also normalizes automaticly
            return considerationCurve.GetRemappedValue(input);
        }



    }

}
