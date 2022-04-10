using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VR;

public class Node : MonoBehaviour
{
    public float health = 100;
    private NodeMeshGenerator nodeMesh;
    private BossBlobController controller;
    public int index = 0;

    private NavMeshAgent agent;

    [Header("References")]
    public GameObject collider;
    public GameObject trigger;
    private void OnEnable()
    {
        nodeMesh = GetComponentInParent<NodeMeshGenerator>();
        controller = GetComponentInParent<BossBlobController>();
    }
    // Start is called before the first frame update
    void Awake()
    {
        nodeMesh = GetComponentInParent<NodeMeshGenerator>();
        controller = GetComponentInParent<BossBlobController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(agent != null)
        {
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
        }
        
    }

    public GameObject SpawnObjectInTrigger(GameObject obj)
    {
        Vector3 pos = trigger.transform.position;
        GameObject spawnedObj = Instantiate(obj, pos, Quaternion.identity, trigger.transform);
        spawnedObj.transform.SetParent(trigger.transform);
        
        spawnedObj.transform.localPosition = new Vector3(0, 0, 0);
        spawnedObj.transform.eulerAngles = new Vector3(-90, 0, 0);
        Vector3 size = trigger.GetComponent<BoxCollider>().size;
        Vector3 randPos = new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(size.y / 2, size.y), Random.Range(0, size.z));
        spawnedObj.transform.localPosition += randPos;
        spawnedObj.transform.SetParent(null);
        return spawnedObj;
    }

    public void DeleteNode()
    {
        nodeMesh.RemoveNode(gameObject);
    }

    public void SplitNode()
    {
        nodeMesh.SplitNode(index);
    }

    public void SetIndex(int i)
    {
        index = i;
    }

    public void SetAgent(NavMeshAgent a)
    {
        agent = a;
    }

    public BossBlobController BlobController()
    {
        return controller;
    }

    public bool TakeDamage(float damage) // returns true or false if health below 0.
    {
        if (health - damage <= 0)
        {
            health = 0;
            return true;
        }
        else
        {
            health -= damage;
            return false;
        }
    }

    public void OnNodeHit(Collider other)
    {
        if (other.gameObject.layer == 10) // if bullet layer
        {
            
            Bullet bullet = other.GetComponent<Bullet>();
            controller.DamageNode(gameObject, bullet.damage);
            bullet.OnCollisionEnter(null);
        }

        Missile ms = other.GetComponent<Missile>();
        if(ms){
            controller.DamageNode(gameObject, ms.damage);
            ms.OnCollisionEnter(null);
        }
    }

    public NodeMeshGenerator NodeMesh()
    {
        return nodeMesh;
    }

   
}
