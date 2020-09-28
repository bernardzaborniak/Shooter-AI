using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform[] team1Spawns;
    public Transform[] team2Spawns;
    public GameObject team1Soldier;
    public GameObject team2Soldier;

    public Transform targetPosition;

    [Header("Timing & Scaling")]
    public float spawnIntervalMin;
    public float spawnIntervalMax;
    float nextTeam1SpawnTime;
    float nextTeam2SpawnTime;

   

    void Update()
    {
        if(Time.time > nextTeam1SpawnTime)
        {
            nextTeam1SpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
            SpawnTeam1Soldier();
        }
        if (Time.time > nextTeam2SpawnTime)
        {
            nextTeam2SpawnTime = Time.time + Random.Range(spawnIntervalMin, spawnIntervalMax);
            SpawnTeam2Soldier();
        }
    }

    void SpawnTeam1Soldier()
    {
        Transform currentSpawn = team1Spawns[Random.Range(0, team1Spawns.Length )];

        GameObject soldier = Instantiate(team1Soldier, currentSpawn.position, currentSpawn.rotation);

        //set the target
        AIController aiController = soldier.transform.GetChild(0).GetComponent<AIController>();
        aiController.targetPosition = targetPosition;
    }

    void SpawnTeam2Soldier()
    {
        Transform currentSpawn = team2Spawns[Random.Range(0, team2Spawns.Length )];

        GameObject soldier = Instantiate(team2Soldier, currentSpawn.position, currentSpawn.rotation);

        //set the target
        AIController aiController = soldier.transform.GetChild(0).GetComponent<AIController>();
        aiController.targetPosition = targetPosition;
    }
}
