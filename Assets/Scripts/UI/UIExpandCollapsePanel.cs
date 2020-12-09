using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExpandCollapsePanel : MonoBehaviour
{
    public GameObject panelToExpand;
    [Tooltip("Content Size Fitters inside layout groups need to be updated per script to layout properly - unity bug? or I am using it wrong, but it makes sense")]
    public ContentSizeFitter contentSizeFitterToUpdate;

    public void OnExpandOrHideToogleableButtonClicked(ToogleableButton button)
    {
        if (button.active)
        {
            panelToExpand.SetActive(true);
        }
        else
        {
            panelToExpand.SetActive(false);
        }
        //UpdateContentSizeFitter();
        Invoke("UpdateContentSizeFitterDelayed", 0.02f);  // a delay of 0.1f does not work, the UI System seems to update delayed. If it isnt working, increase the delay to 0.5f or smth.
    }

    void UpdateContentSizeFitterDelayed()
    {
        //this
        contentSizeFitterToUpdate.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentSizeFitterToUpdate.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        //or this
        //contentSizeFitterToUpdate.enabled = false;
        //contentSizeFitterToUpdate.enabled = true;

        // This seemingly meaningless code forses the contentFitter to "Update". Otherwise it doesnt update when expanded or collapsed
    }

}
