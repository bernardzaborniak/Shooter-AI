using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BenitosAI
{
    public class EntityTags : MonoBehaviour
    {
        //[Header("Tagging System")]
  
        //action tags
        public HashSet<EntityActionTag> actionTags = new HashSet<EntityActionTag>();
        public EntityActionTag[] actionTagsVisualised;

        //threat tags
        [Tooltip("What kind of threats does this unit pose?")]
        public EntityThreatTag[] threatTags;
        [Tooltip("a rough represantation of how strong this unit is - a normal soldier would be like 1, a mech something like 10-40")]
        public float strengthLevel;

        public void AddEntityActionTags(EntityActionTag[] tagsToAdd)
        {

            if (tagsToAdd != null)
            {
                for (int i = 0; i < tagsToAdd.Length; i++)
                {
                    actionTags.Add(tagsToAdd[i]);
                }
            }

            //debug only, vill visualiser
            actionTagsVisualised = new EntityActionTag[actionTags.Count];
            actionTags.CopyTo(actionTagsVisualised);
        }

        public void RemoveEntityActionTags(EntityActionTag[] tagsToRemove)
        {
            if (tagsToRemove != null)
            {
                for (int i = 0; i < tagsToRemove.Length; i++)
                {
                    actionTags.Remove(tagsToRemove[i]);
                }
            }

            //debug only, vill visualiser
            actionTagsVisualised = new EntityActionTag[actionTags.Count];
            actionTags.CopyTo(actionTagsVisualised);
        }
    }
}