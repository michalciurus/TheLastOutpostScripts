using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using BehaviorDesigner.Runtime.Tasks;
using Lightbug.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public enum SpawnChance
{
    VeryRare = 10,
    Rare = 30,
    Normal = 40,
    High = 75,
    Guaranteed = 100
}

public class Spawner : MonoBehaviour
{
    public SpawnerTag[] spawnTags;
    public String prefabName;
    public SpawnChance spawnChance = SpawnChance.Normal;
    public DropChance maximumDropChance = DropChance.Common;
    public bool useDimensionFilter = false;
    public Dimensions dimensionsFilter;
    public SpawnerTag tags;

    private GameObject player;
    private GameObject spawned;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.Find("MainPlayer");

        var rand = Random.Range(0.0f, 1.0f);
        
        if (rand > ( (int)spawnChance / 100.0f ))
        {
            Debug.Log("Bad luck spawn");
            return;
        }
        
        var items = useDimensionFilter ? SpawnSettings.spawnRandomItem(tags,dimensionsFilter) : SpawnSettings.spawnRandomItem(tags);

        var name = items.prefabName;
        if (!prefabName.IsNullOrEmpty())
        {
            name = prefabName;
        }

        var k = Resources.Load("Prefabs/" + name);

        if (k == null)
        {
            Debug.Log("OBJECT MISSING IN PREFABS " + name);
            
        }
        
        Debug.Log("TRYING: " + items.prefabName);
        
        spawned = Instantiate(k, transform.position, transform.rotation) as GameObject;

        Debug.Log("SPAWNED: " + items.prefabName);
    }

    private void OnDrawGizmos()
    {
        var c = Color.yellow;
        c.a = 0.2f;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, 0.15f);
        Gizmos.DrawLine(transform.position, ( transform.position + (transform.rotation *  (Vector3.forward * 0.1f))));
    }

    // Update is called once per frame
    void Update()
    {
        if(spawned)
        {
            spawned.SetActive((spawned.transform.position - player.transform.position).magnitude < 15.0f);
        }

    }
}
