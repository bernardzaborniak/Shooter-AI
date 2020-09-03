using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FM: Class for demo, when player enters trigger he will look at this object for few seconds then return looking at main look target object
    /// </summary>
    public class FLookAnimator_Demo_TriggeredMomentTarget : MonoBehaviour
    {
        public float timeOfLooking = 3f;
        public Vector3 offset = Vector3.up;
        public GameObject generatedFollow { get; private set; }
        private Renderer rend;

        private void Start()
        {
            rend = GetComponent<Renderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                FLookAnimator lookAnimator = other.gameObject.GetComponent<FLookAnimator>();

                if (lookAnimator)
                {
                    generatedFollow = lookAnimator.SetMomentLookTarget(transform, offset, timeOfLooking);
                }
            }
        }

        private void Update()
        {
            if (generatedFollow)
                rend.material.color = Color.Lerp(rend.material.color, Color.green, Time.deltaTime * 6f);
            else
                rend.material.color = Color.Lerp(rend.material.color, Color.gray, Time.deltaTime * 6f);
        }
    }
}