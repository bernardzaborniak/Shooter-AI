using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIExpandCollapsePanel : MonoBehaviour
{
    public Transform panelToExpand;
    [Tooltip("Content Size Fitters inside layout groups need to be updated per script to layout properly - unity bug? or I am using it wrong, but it makes sense")]
    public ContentSizeFitter contentSizeFitterToUpdate;

    public TextMeshProUGUI tmp_numberOfItemsInsidePanel;

    public void OnExpandOrHideToogleableButtonClicked(ToogleableButton button)
    {
        if (panelToExpand)
        {
            if (button.active)
            {
                panelToExpand.gameObject.SetActive(true);
            }
            else
            {
                panelToExpand.gameObject.SetActive(false);
            }
        }
        
        //UpdateContentSizeFitter();
        // Invoke("UpdateContentSizeFitterDelayed", 0.02f);  // a delay of 0.1f does not work, the UI System seems to update delayed. If it isnt working, increase the delay to 0.5f or smth.

        //LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        //LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());


        /*Canvas.ForceUpdateCanvases();  // *
        GetComponent<VerticalLayoutGroup>().enabled = false; // **
        GetComponent<VerticalLayoutGroup>().enabled = true;
        transform.parent.GetComponent<VerticalLayoutGroup>().enabled = false; // **
        transform.parent.GetComponent<VerticalLayoutGroup>().enabled = true;*/
    }

    public void UpdateNumberOfItemsInsidePanel(int itemsInsidePanel)
    {
        if(tmp_numberOfItemsInsidePanel) tmp_numberOfItemsInsidePanel.text = itemsInsidePanel.ToString();
    }

   /* void UpdateContentSizeFitterDelayed()
    {
        //this
        contentSizeFitterToUpdate.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentSizeFitterToUpdate.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        //or this
        //contentSizeFitterToUpdate.enabled = false;
        //contentSizeFitterToUpdate.enabled = true;

        // This seemingly meaningless code forses the contentFitter to "Update". Otherwise it doesnt update when expanded or collapsed
    }*/

}
