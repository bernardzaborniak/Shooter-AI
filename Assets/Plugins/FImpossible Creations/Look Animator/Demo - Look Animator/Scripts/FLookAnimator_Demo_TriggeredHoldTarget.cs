using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FM: Class for demo, when player enters trigger he will look at this object until he leaves it's range, look at order is prioritized
    /// </summary>
    public class FLookAnimator_Demo_TriggeredHoldTarget : MonoBehaviour
    {
        public Vector3 offset = Vector3.up;
        public GameObject generatedFollow { get; private set; }
        private Renderer rend;

        private void Start()
        {
            rend = GetComponent<Renderer>();
        }

        private static List<FLookAnimator_Demo_TriggeredHoldTarget> priorityList;
        private static void RefreshArray()
        {
            if (priorityList == null) priorityList = new List<FLookAnimator_Demo_TriggeredHoldTarget>();
            for (int i = priorityList.Count - 1; i >= 0; i--) if (priorityList[i] == null) priorityList.RemoveAt(i);
        }

        private static void AddToPriority(FLookAnimator_Demo_TriggeredHoldTarget source)
        {
            if (priorityList == null) priorityList = new List<FLookAnimator_Demo_TriggeredHoldTarget>();
            if (!priorityList.Contains(source)) priorityList.Insert(0, source);
        }

        private static void RemoveFromPriority(FLookAnimator_Demo_TriggeredHoldTarget source)
        {
            if (priorityList == null) priorityList = new List<FLookAnimator_Demo_TriggeredHoldTarget>();
            if (priorityList.Contains(source)) priorityList.Remove(source);
        }

        public void AssignToLookAnimator(FLookAnimator lookAnim)
        {
            if (!generatedFollow)
            {
                generatedFollow = new GameObject(transform.gameObject.name + "-MomentLookTarget");
                generatedFollow.transform.SetParent(transform);
                generatedFollow.transform.localPosition = offset;
            }

            lookAnim.SetMomentLookTransform(generatedFollow.transform);
            //lookAnim.SetLookTarget(generatedFollow.transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                FLookAnimator lookAnimator = other.gameObject.GetComponent<FLookAnimator>();

                if (lookAnimator)
                {
                    AddToPriority(this);
                    RefreshArray();

                    if (priorityList.Count > 0)
                    {
                        if (priorityList[0] == this)
                        {
                            AssignToLookAnimator(lookAnimator);
                        }
                    }
                }
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                FLookAnimator lookAnimator = other.gameObject.GetComponent<FLookAnimator>();

                if (lookAnimator)
                {
                    if (generatedFollow) Destroy(generatedFollow);
                    RemoveFromPriority(this);
                    RefreshArray();

                    if (priorityList.Count > 0)
                    {
                        priorityList[0].AssignToLookAnimator(lookAnimator);
                    }
                }
            }
        }


        private void Update()
        {
            if (!rend) return;
                 
            if (generatedFollow)
                rend.material.color = Color.Lerp(rend.material.color, Color.green, Time.deltaTime * 6f);
            else
                rend.material.color = Color.Lerp(rend.material.color, Color.gray, Time.deltaTime * 6f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 0.2f, 1f, 0.6f);
            Gizmos.DrawLine(transform.position, transform.position + offset);
        }
    }
}