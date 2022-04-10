using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    public Vector3 spawnDestination;

    private GameObject enemyPrefab;
    private int enemyCount;
    private float radius = 5.0f;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies()
    {
        for(int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }

        animator.SetTrigger("Retract");
    }

    public void SpawnEnemy()
    {
        Vector3 position = RandomPositionAroundDestination();
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity, null);
        GameManager.instance.enemyList.Add(enemy.transform);
        if(enemy.GetComponent<NavMeshAgent>() != null)
        {
            enemy.GetComponent<NavMeshAgent>().Warp(position);
        }
        else
        {
            enemy.transform.position = position;
        }
       
    }

    public Vector3 RandomPositionAroundDestination()
    {
        return new Vector3(spawnDestination.x + Random.Range(-radius, radius), 0, spawnDestination.z + Random.Range(-radius, radius));
    }
    public void SetSpawnDestination(Vector3 pos)
    {
        spawnDestination = pos;
    }

    public void SetPrefab(GameObject prefab)
    {
        enemyPrefab = prefab;
    }
    public void SetEnemyCount(int count)
    {
        enemyCount = count;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
