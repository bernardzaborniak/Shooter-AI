using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TacticalPointsManager : MonoBehaviour
{
    public GameObject openFieldPointPrefab;
    public float standingCoverQualityHeight;
    public float crouchedCoverQualityHeight;
    public float maxSnapDistanceToNavmesh;

    public HashSet<TacticalPointsGeneratorBox> tacticalPointGenerators = new HashSet<TacticalPointsGeneratorBox>();
    public static TacticalPointsManager Instance;

    void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
    {
        Instance = this;
    }

    public void AddTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Add(generator);
    }

    public void RemoveTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Remove(generator);
    }

    public void GenerateAll()
    {
        foreach (TacticalPointsGeneratorBox generator in tacticalPointGenerators)
        {
            generator.Generate();
        }
    }

    public void BakeAllCoverDistanceRatings()
    {
        foreach (TacticalPointsGeneratorBox generator in tacticalPointGenerators)
        {
            generator.BakeCoverDistanceRating();
        }
    }

    public void BakeAllCoverQualityRatings()
    {
        foreach (TacticalPointsGeneratorBox generator in tacticalPointGenerators)
        {
            generator.BakeCoverQualityRating();
        }
    }
}
