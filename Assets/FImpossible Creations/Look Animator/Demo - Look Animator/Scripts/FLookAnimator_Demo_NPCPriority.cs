using System.Collections.Generic;
using UnityEngine;

namespace FIMSpace.FLook
{
    /// <summary>
    /// FM: Class for demo, npc's are looking at each other if are in trigger range unless player entering visibility zone
    /// </summary>
    public class FLookAnimator_Demo_NPCPriority : MonoBehaviour
    {
        public string ImportantTargetTag = "Player";
        public string NPCTag;

        public GameObject generatedFollow { get; private set; }

        private FLookAnimator npcLookAnimator;
        private List<Transform> targetsInRange;
        private List<Transform> importantTargetsInRange;


        // When component is added inside editor, it reads object's tag to be putted in NPCTag variable
        private void Reset()
        {
            NPCTag = gameObject.tag;
        }

        private void Awake()
        {
            targetsInRange = new List<Transform>();
            importantTargetsInRange = new List<Transform>();
        }

        private void Start()
        {
            npcLookAnimator = GetComponent<FLookAnimator>();
        }

        /// <summary>
        /// Adding new target to objects in our sight range, last one entered will be observed (unless player enters range)
        /// </summary>
        private void NewTargetInRange(Transform source, List<Transform> list)
        {
            if (!list.Contains(source)) list.Insert(0, source);
            CheckTargets();
        }

        /// <summary>
        /// Removing target from visibility range for npc
        /// </summary>
        private void TargetLeftVisibilityRange(Transform source, List<Transform> list)
        {
            if (list.Contains(source)) list.Remove(source);
            CheckTargets();
        }

        private void CleanArrayFromNulls(List<Transform> list)
        {
            for (int i = list.Count - 1; i >= 0; i--) if (list[i] == null) list.RemoveAt(i);
        }

        private Transform GetNearest(List<Transform> list)
        {
            if (list.Count > 0)
            {
                Transform target = list[0];

                if ( list.Count > 1)
                {
                    float nearestDist = Vector3.Distance(list[0].position, transform.position);

                    for (int i = 1; i < list.Count; i++)
                    {
                        float distance = Vector3.Distance(list[i].position, transform.position);
                        if ( distance < nearestDist)
                        {
                            target = list[i];
                            nearestDist = distance;
                        }
                    }
                }

                return target;
            }

            return null;
        }

        // When someone steps in range
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == ImportantTargetTag)
            {
                NewTargetInRange(other.transform, importantTargetsInRange);
            }
            else
                if (other.tag == NPCTag)
                    NewTargetInRange(other.transform, targetsInRange);
        }


        // When someone left look at range
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == ImportantTargetTag)
            {
                TargetLeftVisibilityRange(other.transform, importantTargetsInRange);
            }
            else
                if (other.tag == NPCTag)
                TargetLeftVisibilityRange(other.transform, targetsInRange);
        }


        private void CheckTargets()
        {
            if (!generatedFollow) generatedFollow = new GameObject(transform.gameObject.name + "-MomentLookTarget");

            // Just for security, cleaning array if some objects to follow was deleted lately
            CleanArrayFromNulls(targetsInRange);
            CleanArrayFromNulls(importantTargetsInRange);

            // If important target is set, we looking at it (player)
            if (importantTargetsInRange.Count > 0)
            {
                //npcLookAnimator.ObjectToFollow = importantTarget;
                Transform target = GetNearest(importantTargetsInRange);
                generatedFollow.transform.SetParent(target, true);
                generatedFollow.transform.position = npcLookAnimator.TryFindHeadPositionInTarget(target);
                npcLookAnimator.SetMomentLookTransform(generatedFollow.transform);
            }
            else // In other situation, we will take nearest target
                if (targetsInRange.Count > 0)
            {
                //npcLookAnimator.ObjectToFollow = targetsInRange[0];
                Transform target = GetNearest(targetsInRange);
                generatedFollow.transform.SetParent(target, true);
                generatedFollow.transform.position = npcLookAnimator.TryFindHeadPositionInTarget(target);
                npcLookAnimator.SetMomentLookTransform(generatedFollow.transform);
            }
        }

        //private void Update()
        //{
        //    CheckTargets();
        //}
    }
}