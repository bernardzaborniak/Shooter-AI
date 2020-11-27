using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualisationUI : MonoBehaviour
{
    public VisualisationManager manager;

    [Header("Tactical Points Options")]
    public ToogleableButton showOpenFieldPointsButton;
    public ToogleableButton showCoverPointsButton;
    public ToogleableButton showCoverShootPointsButton;
    [Space(5)]
    public ToogleableButton showCoverDistanceRatingButton;
    public ToogleableButton showCoverDistanceRatingNumbersButton;
    [Space(5)]
    public ToogleableButton showCoverQualityRatingButton;
    public ToogleableButton showCoverQualityRatingNumbersButton;


    void Update()
    {
        showOpenFieldPointsButton.SetActiveExternally(manager.settings.showOpenFieldPoints);
        showCoverPointsButton.SetActiveExternally(manager.settings.showCoverPoints);
        showCoverShootPointsButton.SetActiveExternally(manager.settings.showCoverShootPoints);

        showCoverDistanceRatingButton.SetActiveExternally(manager.settings.showCoverDistanceRating);
        showCoverDistanceRatingNumbersButton.SetActiveExternally(manager.settings.showCoverDistanceRatingNumbers);

        showCoverQualityRatingButton.SetActiveExternally(manager.settings.showCoverQualityRating);
        showCoverQualityRatingNumbersButton.SetActiveExternally(manager.settings.showCoverQualityRatingNumbers);

    }

    public void OnShowOpenFieldPointsButtonClicked(ToogleableButton button)
    {
        manager.settings.showOpenFieldPoints = button.active;
    }

    public void OnShowCoverPointsButtonClicked(ToogleableButton button)
    {
        manager.settings.showCoverPoints = button.active;
    }

    public void OnShowCoverShootPointsButtonClicked(ToogleableButton button)
    {
        manager.settings.showCoverShootPoints = button.active;
    }


    public void OnShowCoverDistanceRatingButtonClicked(ToogleableButton button)
    {
        manager.settings.showCoverDistanceRating = button.active;
    }
    public void OnShowCoverDistanceRatingNumbersButtonClicked(ToogleableButton button)
    {
        manager.settings.showCoverDistanceRatingNumbers = button.active;
    }

    public void OnShowCoverQualityRatingButtonClicked(ToogleableButton button)
    {
        manager.settings.showCoverQualityRating = button.active;
    }
    public void OnShowCoverQualityRatingNumbersButtonClicked(ToogleableButton button)
    {
        manager.settings.showCoverQualityRatingNumbers = button.active;
    }

}
