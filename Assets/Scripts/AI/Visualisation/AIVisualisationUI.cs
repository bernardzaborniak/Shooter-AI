using System.Collections;
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
        public UIExpandCollapsePanel sensingTPointsCoverPeekPanel;
        public UIExpandCollapsePanel sensingTPointCurrentlyUsedPanel;



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

        public void UpdateSensingUIItems(AIController_Blackboard blackboard)
        {
            //Reset the UI - delete all enemies & friendlies etc

            if (blackboard != null)
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
                for (int i = 0; i < sensingTPointsCoverPeekPanel.panelToExpand.childCount; i++)
                {
                    objectsToDestroy.Add(sensingTPointsCoverPeekPanel.panelToExpand.GetChild(i).gameObject);
                }
                for (int i = 0; i < sensingTPointCurrentlyUsedPanel.panelToExpand.childCount; i++)
                {
                    objectsToDestroy.Add(sensingTPointCurrentlyUsedPanel.panelToExpand.GetChild(i).gameObject);
                }


                foreach (GameObject obj in objectsToDestroy)
                {
                    DestroyImmediate(obj);
                }

                AI_VIS_UI_SensingItem newSensingItem;

                // Update Enemies Panel --------------------------------
                foreach (SensedEntityInfo enemy in blackboard.enemyInfos)
                {
                    if (enemy.IsAlive())
                    {
                        newSensingItem = Instantiate(sensingUIItemPrefab, sensingEnemiesPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                        //topicPanel.SetUp((enemy.entity.name + enemy.entity.GetHashCode()), enemy.lastDistanceMeasured, enemy.timeWhenLastSeen, enemy.frameCountWhenLastSeen, enemy.entity.transform, manager);
                        newSensingItem.SetUp((enemy.entity.name + enemy.GetHashCode()), enemy.lastDistanceMeasured, enemy.timeWhenLastSeen, enemy.frameCountWhenLastSeen, enemy.entity.transform, manager);
                    }
                }
                sensingEnemiesPanel.UpdateNumberOfItemsInsidePanel(blackboard.enemyInfos.Length);

                // Update Friendlies Panel --------------------------------
                foreach (SensedEntityInfo friendly in blackboard.friendlyInfos)
                {
                    if (friendly.IsAlive())
                    {
                        newSensingItem = Instantiate(sensingUIItemPrefab, sensingFriendliesPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                        newSensingItem.SetUp((friendly.entity.name + friendly.entity.GetHashCode()), friendly.lastDistanceMeasured, friendly.timeWhenLastSeen, friendly.frameCountWhenLastSeen, friendly.entity.transform, manager);
                    }

                }
                sensingFriendliesPanel.UpdateNumberOfItemsInsidePanel(blackboard.friendlyInfos.Length);

                // Update TPoints Cover Panel --------------------------------
                foreach (SensedTacticalPointInfo tPoint in blackboard.tPCoverInfos)
                {
                    newSensingItem = Instantiate(sensingUIItemPrefab, sensingTPointsCoverPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                    newSensingItem.SetUp((tPoint.tacticalPoint.tacticalPointType.ToString() + tPoint.tacticalPoint.GetHashCode()), tPoint.lastDistanceMeasured, tPoint.timeWhenLastSeen, tPoint.frameCountWhenLastSeen, tPoint.tacticalPoint.transform, manager);
                }
                sensingTPointsCoverPanel.UpdateNumberOfItemsInsidePanel(blackboard.tPCoverInfos.Length);

                // Update TPoints OpenField Panel --------------------------------
                foreach (SensedTacticalPointInfo tPoint in blackboard.tPOpenFieldInfos)
                {
                    newSensingItem = Instantiate(sensingUIItemPrefab, sensingTPointsOpenFieldPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                    newSensingItem.SetUp((tPoint.tacticalPoint.tacticalPointType.ToString() + tPoint.tacticalPoint.GetHashCode()), tPoint.lastDistanceMeasured, tPoint.timeWhenLastSeen, tPoint.frameCountWhenLastSeen, tPoint.tacticalPoint.transform, manager);
                }
                sensingTPointsOpenFieldPanel.UpdateNumberOfItemsInsidePanel(blackboard.tPOpenFieldInfos.Length);

                // Update TPoints CoverPeek Panel --------------------------------
                foreach (SensedTacticalPointInfo tPoint in blackboard.tPCoverPeekInfos)
                {
                    newSensingItem = Instantiate(sensingUIItemPrefab, sensingTPointsCoverPeekPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                    newSensingItem.SetUp((tPoint.tacticalPoint.tacticalPointType.ToString() + tPoint.tacticalPoint.GetHashCode()), tPoint.lastDistanceMeasured, tPoint.timeWhenLastSeen, tPoint.frameCountWhenLastSeen, tPoint.tacticalPoint.transform, manager);
                }
                sensingTPointsCoverPeekPanel.UpdateNumberOfItemsInsidePanel(blackboard.tPCoverPeekInfos.Length);

                // Update TPoints CurrentlyUsed Panel --------------------------------
                if(blackboard.GetCurrentlyUsedTacticalPoint())
                {
                    TacticalPoint tPoint = blackboard.GetCurrentlyUsedTacticalPoint();
                    newSensingItem = Instantiate(sensingUIItemPrefab, sensingTPointCurrentlyUsedPanel.panelToExpand).GetComponent<AI_VIS_UI_SensingItem>();
                    newSensingItem.SetUp((tPoint.tacticalPointType.ToString() + tPoint.GetHashCode()), 0, 0, 0, tPoint.transform, manager);
                }
                //sensingTPointsCoverPeekPanel.UpdateNumberOfItemsInsidePanel(blackboard.tPCoverPeekInfos.Length);

                
            }
            else
            {
                // Delete Old

                for (int i = 0; i < sensingEnemiesPanel.panelToExpand.childCount; i++)
                {
                    Destroy(sensingEnemiesPanel.panelToExpand.GetChild(i).gameObject);
                }
                sensingEnemiesPanel.UpdateNumberOfItemsInsidePanel(0);

                for (int i = 0; i < sensingFriendliesPanel.panelToExpand.childCount; i++)
                {
                    Destroy(sensingFriendliesPanel.panelToExpand.GetChild(i).gameObject);
                }
                sensingFriendliesPanel.UpdateNumberOfItemsInsidePanel(0);


                for (int i = 0; i < sensingTPointsCoverPanel.panelToExpand.childCount; i++)
                {
                    Destroy(sensingTPointsCoverPanel.panelToExpand.GetChild(i).gameObject);
                }
                sensingTPointsCoverPanel.UpdateNumberOfItemsInsidePanel(0);


                for (int i = 0; i < sensingTPointsOpenFieldPanel.panelToExpand.childCount; i++)
                {
                    Destroy(sensingTPointsOpenFieldPanel.panelToExpand.GetChild(i).gameObject);
                }
                sensingTPointsOpenFieldPanel.UpdateNumberOfItemsInsidePanel(0);

                for (int i = 0; i < sensingTPointsCoverPeekPanel.panelToExpand.childCount; i++)
                {
                    Destroy(sensingTPointsCoverPeekPanel.panelToExpand.GetChild(i).gameObject);
                }
                sensingTPointsCoverPeekPanel.UpdateNumberOfItemsInsidePanel(0);


                for (int i = 0; i < sensingTPointCurrentlyUsedPanel.panelToExpand.childCount; i++)
                {
                    Destroy(sensingTPointCurrentlyUsedPanel.panelToExpand.GetChild(i).gameObject);
                }
                sensingTPointCurrentlyUsedPanel.UpdateNumberOfItemsInsidePanel(0);


            }
        }

        #region Show Sensing Info in World

        public void OnShowEnemyInfosInWorldButtonClicked(ToogleableButton button)
        {
            manager.settings.ShowEnemiesInWorld = button.active;
        }

        public void OnShowFriendlyInfosInWorldButtonClicked(ToogleableButton button)
        {
            manager.settings.ShowFriendliesInWorld = button.active;
        }

        public void OnShowTPCoverInfosInWorldButtonClicked(ToogleableButton button)
        {
            manager.settings.ShowTPCoverInWorld = button.active;
        }

        public void OnShowTPOpenFieldInfosInWorldButtonClicked(ToogleableButton button)
        {
            manager.settings.ShowTPOpenFieldInWorld = button.active;
        }

        public void OnShowTPCoverPeekInfosInWorldButtonClicked(ToogleableButton button)
        {
            manager.settings.ShowTPCoverPeekInWorld = button.active;
        }

        public void OnShowCurrentlyUsedTPointInWorldButtonClicked(ToogleableButton button)
        {
            manager.settings.ShowTPCurrentlyUsedInWorld = button.active;
        }

        #endregion
    }

}
