﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AIVisualisationUI : MonoBehaviour
{
    public AIVisualisationManager manager;

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

    [Header("Selected Soldier Info")]
    public GameObject soldierSelectionUI;
    public TextMeshProUGUI tmp_EntityName;
    public TextMeshProUGUI tmp_EntityTeamID;

    [Header("Detailed Info Menus")]
    public GameObject sensingMenu;
    public GameObject decisionmakerMenu;

    public enum DetailedMenuState
    {
        NoMenu,
        SensingMenuOpen,
        DecisionmakerMenuOpen
    }
    public DetailedMenuState detailedMenuState = DetailedMenuState.NoMenu;


    void Update()
    {
        showOpenFieldPointsButton.SetActiveExternally(manager.settings.showOpenFieldPoints);
        showCoverPointsButton.SetActiveExternally(manager.settings.showCoverPoints);
        showCoverShootPointsButton.SetActiveExternally(manager.settings.showCoverShootPoints);

        showCoverDistanceRatingButton.SetActiveExternally(manager.settings.showCoverDistanceRating);
        showCoverDistanceRatingNumbersButton.SetActiveExternally(manager.settings.showCoverDistanceRatingNumbers);

        showCoverQualityRatingButton.SetActiveExternally(manager.settings.showCoverQualityRating);
        showCoverQualityRatingNumbersButton.SetActiveExternally(manager.settings.showCoverQualityRatingNumbers);

        if(manager.currentSelectedSoldier)
        {
            tmp_EntityName.text = manager.currentSelectedSoldier.name + " " + manager.currentSelectedSoldier.GetHashCode();
            tmp_EntityTeamID.text = manager.currentSelectedSoldier.teamID.ToString();
        }
        else
        {
            tmp_EntityName.text = "";
            tmp_EntityTeamID.text = "";
        }

    }

    public void OnShowOpenFieldPointsButtonClicked(ToogleableButton button)
    {
        Debug.Log("open points clickd");
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

    public void OnHideSoldierSelectionInfoButtonClicked(ToogleableButton button)
    {
        if (button.active)
        {
            soldierSelectionUI.SetActive(false);
        }
        else
        {
            soldierSelectionUI.SetActive(true);
        }
    }

    public void OnOpenSensingMenuButtonCLicked()
    {
        if(detailedMenuState == DetailedMenuState.SensingMenuOpen)
        {
            sensingMenu.SetActive(false);
            detailedMenuState = DetailedMenuState.NoMenu;
        }
        else if(detailedMenuState == DetailedMenuState.DecisionmakerMenuOpen)
        {
            sensingMenu.SetActive(true);
            decisionmakerMenu.SetActive(false);

            detailedMenuState = DetailedMenuState.SensingMenuOpen;
        }
        else if(detailedMenuState == DetailedMenuState.NoMenu)
        {
            sensingMenu.SetActive(true);
            detailedMenuState = DetailedMenuState.SensingMenuOpen;

        }
    }

    public void OnOpenDecisionMakingMenuButtonClicked()
    {
        if (detailedMenuState == DetailedMenuState.DecisionmakerMenuOpen)
        {
            decisionmakerMenu.SetActive(false);
            detailedMenuState = DetailedMenuState.NoMenu;
        }
        else if (detailedMenuState == DetailedMenuState.SensingMenuOpen)
        {
            sensingMenu.SetActive(false);
            decisionmakerMenu.SetActive(true);

            detailedMenuState = DetailedMenuState.DecisionmakerMenuOpen;
        }
        else if (detailedMenuState == DetailedMenuState.NoMenu)
        {
            decisionmakerMenu.SetActive(true);
            detailedMenuState = DetailedMenuState.DecisionmakerMenuOpen;

        }
    }

}
