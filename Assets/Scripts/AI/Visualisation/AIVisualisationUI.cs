using System.Collections;
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

    [Header("Sensing Menu")]
    public GameObject sensingMenu;
    public GameObject sensingUIItemPrefab;

    /*public Transform sensingEnemiesPanelParent;  //parents containing all the spawned items
    public Transform sensingFriendliesPanelParent;
    public Transform sensingTPointsCoverPanelParent;
    public Transform sensingTPointsOpenFieldPanelParent;*/

    public UIExpandCollapsePanel sensingEnemiesPanel;  //parents containing all the spawned items
    public UIExpandCollapsePanel sensingFriendliesPanel;
    public UIExpandCollapsePanel sensingTPointsCoverPanel;
    public UIExpandCollapsePanel sensingTPointsOpenFieldPanel;



    [Header("Decisionmaking Menu")]
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

    public void UpdateSensingUI(SensingInfo sensingInfo)
    {
        //Reset the UI - delete all enemies & friendlies etc

        if (sensingInfo != null)
        {
            // Update Enemies Panel --------------------------------

            HashSet<GameObject> objectsToDestroy = new HashSet<GameObject>();
            for (int i = 0; i < sensingEnemiesPanel.panelToExpand.childCount; i++)
            {
                objectsToDestroy.Add(sensingEnemiesPanel.panelToExpand.GetChild(i).gameObject);
            }
            for (int i = 0; i < sensingFriendliesPanel.panelToExpand.childCount; i++)
            {
                objectsToDestroy.Add(sensingFriendliesPanel.panelToExpand.GetChild(i).gameObject);
            }
            for (int i = 0; i < sensingTPointsCoverPanel.panelToExpand.childCount; i++)
            {
                objectsToDestroy.Add(sensingTPointsCoverPanel.panelToExpand.GetChild(i).gameObject);
            }
            for (int i = 0; i < sensingTPointsOpenFieldPanel.panelToExpand.childCount; i++)
            {
                objectsToDestroy.Add(sensingTPointsOpenFieldPanel.panelToExpand.GetChild(i).gameObject);
            }

            foreach (GameObject obj in objectsToDestroy)
            {
                DestroyImmediate(obj);
            }


            //delete old
            /* for (int i = 0; i < sensingEnemiesPanelParent.childCount; i++)
             {
                 Destroy(sensingEnemiesPanelParent.GetChild(i).gameObject);
             }*/

            //create new
            foreach (AIC_S_EntityVisibilityInfo enemy in sensingInfo.enemiesInSensingRadius)
            {
                AI_VIS_UI_SensingItem topicPanel = Instantiate(sensingUIItemPrefab, sensingEnemiesPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                topicPanel.SetUp((enemy.entity.name + enemy.entity.GetHashCode()), enemy.lastSquaredDistanceMeasured, enemy.timeWhenLastSeen, enemy.frameCountWhenLastSeen, enemy.entity.transform, manager);
            }
            sensingEnemiesPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.enemiesInSensingRadius.Count);


            // Update Friendlies Panel --------------------------------

            //delete old
            /*for (int i = 0; i < sensingFriendliesPanelParent.childCount; i++)
            {
                Destroy(sensingFriendliesPanelParent.GetChild(i).gameObject);
            }*/

            //create new
            foreach (AIC_S_EntityVisibilityInfo friendly in sensingInfo.friendliesInSensingRadius)
            {
                AI_VIS_UI_SensingItem topicPanel = Instantiate(sensingUIItemPrefab, sensingFriendliesPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                topicPanel.SetUp((friendly.entity.name + friendly.entity.GetHashCode()), friendly.lastSquaredDistanceMeasured, friendly.timeWhenLastSeen, friendly.frameCountWhenLastSeen, friendly.entity.transform, manager);
            }
            sensingFriendliesPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.friendliesInSensingRadius.Count);



            // Update TPoints Cover Panel --------------------------------

            //delete old
            /*for (int i = 0; i < sensingTPointsCoverPanelParent.childCount; i++)
            {
                Destroy(sensingTPointsCoverPanelParent.GetChild(i).gameObject);
            }*/

            //create new
            foreach (AIC_S_TacticalPointVisibilityInfo tPoint in sensingInfo.tPointsCoverInSensingRadius)
            {
                AI_VIS_UI_SensingItem topicPanel = Instantiate(sensingUIItemPrefab, sensingTPointsCoverPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                topicPanel.SetUp((tPoint.point.tacticalPointType.ToString() + tPoint.point.GetHashCode()), tPoint.lastSquaredDistanceMeasured, tPoint.timeWhenLastSeen, tPoint.frameCountWhenLastSeen, tPoint.point.transform, manager);
            }
            sensingTPointsCoverPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.tPointsCoverInSensingRadius.Count);



            // Update TPoints OpenField Panel --------------------------------

            //delete old
            /*for (int i = 0; i < sensingTPointsopenFieldPanelParent.childCount; i++)
            {
                Destroy(sensingTPointsopenFieldPanelParent.GetChild(i).gameObject);
            }*/

            //create new
            foreach (AIC_S_TacticalPointVisibilityInfo tPoint in sensingInfo.tPointsOpenFieldInSensingRadius)
            {
                AI_VIS_UI_SensingItem topicPanel = Instantiate(sensingUIItemPrefab, sensingTPointsOpenFieldPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                topicPanel.SetUp((tPoint.point.tacticalPointType.ToString() + tPoint.point.GetHashCode()), tPoint.lastSquaredDistanceMeasured, tPoint.timeWhenLastSeen, tPoint.frameCountWhenLastSeen, tPoint.point.transform, manager);
            }
            sensingTPointsOpenFieldPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.tPointsOpenFieldInSensingRadius.Count);



        }
        else
        {
            // Delete Old

            for (int i = 0; i < sensingEnemiesPanel.panelToExpand.childCount; i++)
            {
                Destroy(sensingEnemiesPanel.panelToExpand.GetChild(i).gameObject);
            }

            for (int i = 0; i < sensingFriendliesPanel.panelToExpand.childCount; i++)
            {
                Destroy(sensingFriendliesPanel.panelToExpand.GetChild(i).gameObject);
            }

            for (int i = 0; i < sensingTPointsCoverPanel.panelToExpand.childCount; i++)
            {
                Destroy(sensingTPointsCoverPanel.panelToExpand.GetChild(i).gameObject);
            }

            for (int i = 0; i < sensingTPointsOpenFieldPanel.panelToExpand.childCount; i++)
            {
                Destroy(sensingTPointsOpenFieldPanel.panelToExpand.GetChild(i).gameObject);
            }
        }
    }

}
