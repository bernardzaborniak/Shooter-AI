using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ------- Workaround to unity UI system -------
// When using nestet Content size fitters togther with vertical layout groups, the Ui does not update correctly on adding and removing children from the layout groups.
// This script fixes this by forcing the UI to update - but it is very propably bad for performance :/
public class ContentSizeFitterUpdater : MonoBehaviour
{
    public RectTransform[] rectTransformsToUpdate; // We have an array here but updating only the parent rectTransform holding all the contentSize fitters and layout groups saeems enough. - attach this script to it and cache it here.
    
    void Update()
    {
        for (int i = 0; i < rectTransformsToUpdate.Length; i++)
        {
            // Apparently this funciton needs to be called twice to ensure smooth update without visual bugs in the UI.
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformsToUpdate[i]);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformsToUpdate[i]);
        }
       
    }
}
