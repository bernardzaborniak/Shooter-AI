﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScenarioManager : MonoBehaviour
{
    [Header("Orders")]
    public BenitosAI.Decision team1Order;
    public BenitosAI.Decision team2Order;


    [Header("References")]
    public Transform[] team1Spawns;
    public Transform[] team2Spawns;
    public GameObject team1Soldier;
    public GameObject team2Soldier;

    public Transform team1TargetPosition;
    public Transform team2TargetPosition;

    public List<GameObject> team1Soldiers = new List<GameObject>();
    public List<GameObject> team2Soldiers = new List<GameObject>();


    [Header("Timing & Scaling")]
    public float spawnIntervalMin;
    public float spawnIntervalMax;
    float nextTeam1SpawnTime;
    float nextTeam2SpawnTime;

    //TODO convert them to properties to change UI when thoose are changed
    [SerializeField] int maxSoldiersTeam1;

    [SerializeField] int maxSoldiersTeam2;

    [SerializeField] bool spawnSoldiersTeam1;

    [SerializeField] bool spawnSoldiersTeam2;


    //public int maxSoldiersTeam1;
    //public int maxSoldiersTeam2;
    //public bool spawnSoldiersTeam1;
    //public bool spawnSoldiersTeam2;

    [Header("UI")]
    public TMP_InputField tmp_maxSoldiersTeam1;
    public TMP_InputField tmp_maxSoldiersTeam2;
    public ToogleableButton button_spawnSoldiersTeam1;
    public ToogleableButton button_spawnSoldiersTeam2;
    public TMP_Text tmp_numOfSoldiersSpawnedTeam1;
    public TMP_Text tmp_numOfSoldiersSpawnedTeam2;



    void Update()
    {
        tmp_maxSoldiersTeam1.text = maxSoldiersTeam1.ToString();
        tmp_maxSoldiersTeam2.text = maxSoldiersTeam2.ToString();

        button_spawnSoldiersTeam1.SetActiveExternally(spawnSoldiersTeam1);
        button_spawnSoldiersTeam2.SetActiveExternally(spawnSoldiersTeam2);

        tmp_numOfSoldiersSpawnedTeam1.text = team1Soldiers.Count.ToString();
        tmp_numOfSoldiersSpawnedTeam2.text = team2Soldiers.Count.ToString();


        if (Time.time > nextTeam1SpawnTime)
        {
            //check for null values
            List<GameObject> team1SoldiersToRemove = new List<GameObject>();

            for (int i = 0; i < team1Soldiers.Count; i++)
            {
                if (team1Soldiers[i] == null) team1SoldiersToRemove.Add(team1Soldiers[i]);
            }

            for (int i = 0; i < team1SoldiersToRemove.Count; i++)
            {
                team1Soldiers.Remove(team1SoldiersToRemove[i]);
            }


            //Remove soldiers if too many
            while (maxSoldiersTeam1 < team1Soldiers.Count)
            {
                GameObject removedSoldier = team1Soldiers[team1Soldiers.Count - 1];
                team1Soldiers.RemoveAt(team1Soldiers.Count - 1);
                Destroy(removedSoldier);
            }

            if (spawnSoldiersTeam1)
            {
                if (team1Soldiers.Count < maxSoldiersTeam1)
                {
                    team1Soldiers.Add(SpawnTeam1Soldier());

                }
            }
            nextTeam1SpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
        }



        if (Time.time > nextTeam2SpawnTime)
        {
            //check for null values
            List<GameObject> team2SoldiersToRemove = new List<GameObject>();

            for (int i = 0; i < team2Soldiers.Count; i++)
            {
                if (team2Soldiers[i] == null) team2SoldiersToRemove.Add(team2Soldiers[i]);
            }

            for (int i = 0; i < team2SoldiersToRemove.Count; i++)
            {
                team2Soldiers.Remove(team2SoldiersToRemove[i]);
            }


            //Remove soldiers if too many
            while (maxSoldiersTeam2 < team2Soldiers.Count)
            {
                GameObject removedSoldier = team2Soldiers[team2Soldiers.Count - 1];
                team2Soldiers.RemoveAt(team2Soldiers.Count - 1);
                Destroy(removedSoldier);
            }

            if (spawnSoldiersTeam2)
            {
                if (team2Soldiers.Count < maxSoldiersTeam2)
                {
                    team2Soldiers.Add(SpawnTeam2Soldier());

                }
            }
            nextTeam2SpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
        }


    }

    GameObject SpawnTeam1Soldier()
    {
        Transform currentSpawn = team1Spawns[Random.Range(0, team1Spawns.Length)];

        return Instantiate(team1Soldier, currentSpawn.position, currentSpawn.rotation);

        //set the target
        //BenitosAI.AIControllerOld aiController = soldier.transform.GetChild(1).GetComponent<BenitosAI.AIControllerOld>();
        // aiController.targetPosition = team1TargetPosition;
        //aiController.SetFinalTargetPosition(team1TargetPosition.position);

    }

    GameObject SpawnTeam2Soldier()
    {
        Transform currentSpawn = team2Spawns[Random.Range(0, team2Spawns.Length)];

        return Instantiate(team2Soldier, currentSpawn.position, currentSpawn.rotation);

        //set the target
        //BenitosAI.AIControllerOld aiController = soldier.transform.GetChild(1).GetComponent<BenitosAI.AIControllerOld>();
        //aiController.targetPosition = team2TargetPosition;
        //aiController.SetFinalTargetPosition(team2TargetPosition.position);
    }

    #region UI Interaction

    public void OnSetMaxSoldiersTeam1(TMP_InputField tmp_inputField)
    {
        int.TryParse(tmp_inputField.text, out maxSoldiersTeam1);
    }
    public void OnSetMaxSoldiersTeam2(TMP_InputField tmp_inputField)
    {
        int.TryParse(tmp_inputField.text, out maxSoldiersTeam2);
    }

    public void OnSetSpawnSoldiersTeam1(ToogleableButton button)
    {
        spawnSoldiersTeam1 = button.active;
    }

    public void OnSetSpawnSoldiersTeam2(ToogleableButton button)
    {
        spawnSoldiersTeam2 = button.active;
    }


    #endregion
}