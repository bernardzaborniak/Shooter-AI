using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//workaround to update content size fitters if their children change
public class ContentSizeFitterUpdater : MonoBehaviour
{
    public RectTransform[] rectTransformsToUpdate;
    

    void Update()
    {
        for (int i = 0; i < rectTransformsToUpdate.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformsToUpdate[i]);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformsToUpdate[i]);
        }
       
    }
}
