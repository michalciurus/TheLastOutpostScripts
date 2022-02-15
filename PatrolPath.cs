using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PatrolPath : MonoBehaviour
{
    public List<Vector3> flockPatrolPoints = new List<Vector3>();
    public bool canSpawnHorde = false;
    public int spawnChance = 100;
    
    private List<GameObject> allManaged;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        allManaged = new List<GameObject>();
        player = GameObject.Find("MainPlayer");

        var roll = Random.Range(0, 100);

        if (roll >= spawnChance) return;
        
        for (int i = 0; i < transform.childCount; i++) {
            flockPatrolPoints.Add(transform.GetChild(i).transform.position);
        }

        if (!DEBUG.SPAWN_ENEMIES) return;
        
        var go = UnitySugar.instantiatePrefab("ZombieEnemy");

        var randomEnemy = SpawnSettings.SpawnRandomEnemy();
        var randomModel = UnitySugar.instantiateMesh(randomEnemy.meshName);

        go.gameObject.transform.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = randomModel;
        
        go.gameObject.transform.FindDeepChildComponent<AI>().patrolPath = this;
        go.gameObject.transform.FindDeepChildComponent<AI>().isAFlockLeader = true;
        if (flockPatrolPoints.Count > 0)
        {
            go.transform.position = flockPatrolPoints[0];
        }
        else
        {
            go.gameObject.transform.FindDeepChildComponent<AI>().isAFlockLeader = false;
            go.transform.position = transform.position;
        }
        
        go.transform.parent = transform;

        go.transform.FindDeepChildComponent<AI>().gameObject.GetComponent<NavMeshAgent>().avoidancePriority = 30;

        allManaged.Add(go);

        if (canSpawnHorde && false)
        {
            Transform last = null;
            for (int i = 0; i < 2; i++)
            {
                var follower = UnitySugar.instantiatePrefab("ZombieEnemy");
                
                follower.transform.position = go.transform.position;
                follower.gameObject.transform.FindDeepChildComponent<AI>().followTarget =
                    go.gameObject.transform.FindDeepChildComponent<AI>().transform;

                if (last != null && i > 2)
                {
                    follower.gameObject.transform.FindDeepChildComponent<AI>().followTarget =
                        last;
                }
                
                follower.transform.parent = transform;
                follower.gameObject.transform.FindDeepChildComponent<AI>().isAFlockLeader = false;
                follower.gameObject.transform.FindDeepChildComponent<AI>().followsAFlockMember = true;
                follower.gameObject.transform.FindDeepChildComponent<AI>().gameObject.GetComponent<NavMeshAgent>().avoidancePriority = 50 + i;
                
                last = follower.gameObject.transform.FindDeepChildComponent<AI>().transform;

            }
        }
    }

    private void OnDrawGizmos()
    {
        Transform previous = null;
        if(transform.childCount > 1)
        {
            foreach (Transform child in transform)
            {
                if (previous != null)
                {
                    Gizmos.DrawLine(previous.position, child.position);
                }

                previous = child;
            }
        }
        else
        {
            Gizmos.DrawSphere(transform.position,0.2f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.back * 0.1f);
        }

    }

    // Update is called once per frame
    void Update()
    {

        foreach(GameObject managed in allManaged)
        {

            managed.SetActive((managed.transform.position - player.transform.position).magnitude < 30.0f);

        }

    }
}
