using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualisationManager : MonoBehaviour
{
    public Transform camTransform;
    //public HashSet<TextToCameraAligner> managedObjects = new HashSet<TextToCameraAligner>();
    public HashSet<TacticalPointVisualiser> tacticalPointVisualisers = new HashSet<TacticalPointVisualiser>();

    //public bool enableTacticalPointVisualisers;


    #region Singleton Code
    public static VisualisationManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion


    void Update()
    {
        Vector3 camForward = camTransform.forward;

        /* #region 1. Update Text aligned To Camera

         

         foreach (TextToCameraAligner obj in managedObjects)
         {
             obj.UpdateTextAligner(camForward);
         }

         #endregion*/

        #region 1. Update Tactical Point Visualisers

        foreach (TacticalPointVisualiser visualiser in tacticalPointVisualisers)
        {
            visualiser.UpdateVisualiser(camForward);
        }

        #endregion

    }

    public void AddTacticalPointVisualiser(TacticalPointVisualiser visualiser)
    {
        tacticalPointVisualisers.Add(visualiser);
    }

    public void RemoveTacticalPointVisualise(TacticalPointVisualiser visualiser)
    {
        tacticalPointVisualisers.Remove(visualiser);
    }

    /*public void AddTextAlignedToCamera(TextToCameraAligner obj)
    {
        managedObjects.Add(obj);
    }

    public void RemoveTextAlignedToCamera(TextToCameraAligner obj)
    {
        managedObjects.Remove(obj);
    }*/
}
