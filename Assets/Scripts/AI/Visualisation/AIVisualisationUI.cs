﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BenitosAI
{

    // UI corresponding to the Visualisation Manager
    public class AIVisualisationUI : MonoBehaviour
    {
        #region Fields

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

        public UIExpandCollapsePanel sensingEnemiesPanel;
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

        #region Variables cached to reduce garbage

        // ----- UpdateSensingUIItems-------

        HashSet<GameObject> objectsToDestroy = new HashSet<GameObject>();


        //--------------------

        #endregion

        #endregion


        void Update()
        {
            showOpenFieldPointsButton.SetActiveExternally(manager.settings.showOpenFieldPoints);
            showCoverPointsButton.SetActiveExternally(manager.settings.showCoverPoints);
            showCoverShootPointsButton.SetActiveExternally(manager.settings.showCoverShootPoints);

            showCoverDistanceRatingButton.SetActiveExternally(manager.settings.showCoverDistanceRating);
            showCoverDistanceRatingNumbersButton.SetActiveExternally(manager.settings.showCoverDistanceRatingNumbers);

            showCoverQualityRatingButton.SetActiveExternally(manager.settings.showCoverQualityRating);
            showCoverQualityRatingNumbersButton.SetActiveExternally(manager.settings.showCoverQualityRatingNumbers);

            if (manager.currentSelectedSoldier)
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
            if (detailedMenuState == DetailedMenuState.SensingMenuOpen)
            {
                sensingMenu.SetActive(false);
                detailedMenuState = DetailedMenuState.NoMenu;
            }
            else if (detailedMenuState == DetailedMenuState.DecisionmakerMenuOpen)
            {
                sensingMenu.SetActive(true);
                decisionmakerMenu.SetActive(false);

                detailedMenuState = DetailedMenuState.SensingMenuOpen;
            }
            else if (detailedMenuState == DetailedMenuState.NoMenu)
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

        public void UpdateSensingUIItems(SensingInfo sensingInfo)
        {
            //Reset the UI - delete all enemies & friendlies etc

            if (sensingInfo != null)
            {
                // Destroy old items immediately to prevent UI visuali bugs due to vertuical group & content size fitter
                objectsToDestroy.Clear();
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

                AI_VIS_UI_SensingItem topicPanel;

                // Update Enemies Panel --------------------------------
                foreach (SensedEntityInfo enemy in sensingInfo.enemyInfos.Values)
                {
                    if (enemy.IsAlive())
                    {
                        topicPanel = Instantiate(sensingUIItemPrefab, sensingEnemiesPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                        topicPanel.SetUp((enemy.entity.name + enemy.entity.GetHashCode()), enemy.lastSquaredDistanceMeasured, enemy.timeWhenLastSeen, enemy.frameCountWhenLastSeen, enemy.entity.transform, manager);
                    }
                }
                sensingEnemiesPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.enemyInfos.Count);

                // Update Friendlies Panel --------------------------------
                foreach (SensedEntityInfo friendly in sensingInfo.friendlyInfos.Values)
                {
                    if (friendly.IsAlive())
                    {
                        topicPanel = Instantiate(sensingUIItemPrefab, sensingFriendliesPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                        topicPanel.SetUp((friendly.entity.name + friendly.entity.GetHashCode()), friendly.lastSquaredDistanceMeasured, friendly.timeWhenLastSeen, friendly.frameCountWhenLastSeen, friendly.entity.transform, manager);
                    }

                }
                sensingFriendliesPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.friendlyInfos.Count);

                // Update TPoints Cover Panel --------------------------------
                foreach (SensedTacticalPointInfo tPoint in sensingInfo.tPointsCoverInfos.Values)
                {
                    topicPanel = Instantiate(sensingUIItemPrefab, sensingTPointsCoverPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                    topicPanel.SetUp((tPoint.tacticalPoint.tacticalPointType.ToString() + tPoint.tacticalPoint.GetHashCode()), tPoint.lastSquaredDistanceMeasured, tPoint.timeWhenLastSeen, tPoint.frameCountWhenLastSeen, tPoint.tacticalPoint.transform, manager);
                }
                sensingTPointsCoverPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.tPointsCoverInfos.Count);

                // Update TPoints OpenField Panel --------------------------------
                foreach (SensedTacticalPointInfo tPoint in sensingInfo.tPointsOpenFieldInfos.Values)
                {
                    topicPanel = Instantiate(sensingUIItemPrefab, sensingTPointsOpenFieldPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                    topicPanel.SetUp((tPoint.tacticalPoint.tacticalPointType.ToString() + tPoint.tacticalPoint.GetHashCode()), tPoint.lastSquaredDistanceMeasured, tPoint.timeWhenLastSeen, tPoint.frameCountWhenLastSeen, tPoint.tacticalPoint.transform, manager);
                }
                sensingTPointsOpenFieldPanel.UpdateNumberOfItemsInsidePanel(sensingInfo.tPointsCoverInfos.Count);
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

}
