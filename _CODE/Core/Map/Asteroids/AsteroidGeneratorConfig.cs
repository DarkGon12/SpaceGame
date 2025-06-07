using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Asteroid_Generator_Config", menuName = "Config/Asteroid")]
public class AsteroidGeneratorConfig : ConfigBase
{
    [Header("Asteroid Settings")]
    public List<GameObject> AsteroidPrefabCollection;
    [Range(1, 100000)] public int AsteroidCount = 1000;
    [Range(1f, 100f)] public float AsteroidSpacing = 10f;

    [Header("Generation Zone")]
    public Vector3 ZoneSize = new Vector3(500, 500, 500);
    public int RegenerateTime = 20;
}