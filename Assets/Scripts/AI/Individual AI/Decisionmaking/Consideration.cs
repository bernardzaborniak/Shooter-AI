using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    [CreateAssetMenu(menuName = "AI/Consideration", fileName = "new Consideration")]
    public class Consideration : ScriptableObject
    {

        public string description;

        #region For Input Visalisation Editor Only
#if UNITY_EDITOR
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
#endif
        #endregion

        //Inputs needed:

        //character data
        //mine & targets

        //Game Engine Data
        //distance between 
        //elapsed time

        //other systems in the world

        //each of this is predefined -> put them in a dropdown list

        //public ConsiderationInput_HumanoidSoldier considerationInput;
        [SerializeField] ConsiderationInput considerationInput;
        [SerializeField] ConsiderationInputParams considerationInputParams;
        //float input;

        //for Range
        //public float min;
       // public float max;
        // for desired range
       // public float desiredFloatValue;
        //for WeaponID params Type
       // public int weaponID;
        //for TPointQualityEvaluationParams params type
       // public QualityOfCoverEvaluationType tPointEvaluationType;
       // public int tPointEvaluationMaxEnemiesToAcknowledgeWhileRating;
       // public int tPointEvaluationMaxFriendliesToAcknowledgeWhileRating;

        public CustomCurve considerationCurve;


        private void OnValidate()
        {
            considerationCurve.UpdateCurveVisualisationKeyframes();
        }

        private void OnEnable()
        {

        }

        public float GetConsiderationRating(DecisionContext context)
        {
            

            //Get Input, already normalized by ConsideraionInput
            //float input = considerationInput.GetConsiderationInput(context, this, considerationInputParams);
            float input = considerationInput.GetConsiderationInput(context, considerationInputParams);

            //Debug.Log("Consideration: Ijnput: " + input);

            //TODo Refactor, dont save input into another variable, write it directly into the return statement

            //Debug.Log("Consideration: Remapped Curve: " + considerationCurve.GetRemappedValue(input));

            //Modify input by curve, curve also normalizes automaticly

            return considerationCurve.GetRemappedValue(input);

        }

        //Only for Visualisations
        public float GetConsiderationInput(DecisionContext context)
        {
            return considerationInput.GetConsiderationInput(context, considerationInputParams);
        }
    }

}
