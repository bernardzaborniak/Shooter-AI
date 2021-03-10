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


        [SerializeField] ConsiderationInput considerationInput;
        [SerializeField] ConsiderationInputParams considerationInputParams;

        public CustomCurve considerationCurve; //needs to be public because of the ConisderationEditor


        private void OnValidate()
        {
            considerationCurve.UpdateCurveVisualisationKeyframes();
        }

        public float GetConsiderationRating(DecisionContext context)
        {            
            float input = considerationInput.GetConsiderationInput(context, considerationInputParams);

            return considerationCurve.GetRemappedValue(input);
        }

        //Used By DecisionMaker Memory 
        public float GetConsiderationInput(DecisionContext context)
        {
            return considerationInput.GetConsiderationInput(context, considerationInputParams);
        }
    }

}
