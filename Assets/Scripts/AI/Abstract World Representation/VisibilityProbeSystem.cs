using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityProbeSystem : MonoBehaviour
{
    // This class will be expanded and used sometime, but not today :)
    // A System similar to unitys lightprobe system, but containing information about how dark a place is (enemies not visible in darkness) and how far one can see (enemies not visibile through smote & mist)

    public static VisibilityProbeSystem Instance;

    private void Awake()
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

}
