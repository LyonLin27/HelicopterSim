using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class BossBlobController : MonoBehaviour
{
    [Header("Physics Settings")]
    public float spring = 75;
    public float dampening = 7;
    public float tolerance = 10;

    [Header("Agent Settings")]
    public float speed = 5.5f;
    public float acceleration = 35;
    public float obstacleAvoidance = 2.0f;
    

    [Header("References")]
    public Transform centerRigidPivot;
    public GameObject explodeParticle;

    private Color color;
    private Vector3 currentDestination;
    private List<GameObject> nodes = new List<GameObject>();
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();
    private NodeMeshGenerator nodeMesh;
    private Building targetBuilding;


    private bool editingNodes = false;

    // Start is called before the first frame update
    void Awake()
    {
        nodeMesh = GetComponent<NodeMeshGenerator>();
        nodes = nodeMesh.Nodes();
    }

    // Update is called once per frame
    void Update()
    {
        nodes = nodeMesh.Nodes();
        if (editingNodes == false)
        {
            
            ColliderUpdate();
        }
        
    }

    public void Start()
    {
        SetColor();

        InitializeSprings();
        InitializeNavAgents();

        GoToClosestBuilding();
    }

    public void SetColor()
    {
        color = new Color(Random.value, Random.value, Random.value);
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", color);
        renderer.SetPropertyBlock(block);
    }

    public Color GetColor()
    {
        return color;
    }

    public void DestroyNode(GameObject node)
    {
        editingNodes = true;
        for(int i = 0; i < 4; i++)
        {
            SpawnHitParticle(node);
        }
        nodeMesh.RemoveNode(node);
        InitializeSprings();
        AssertAgentsList();
        SetDestination(GetCurrentDestination(), 5.0f);

        if(nodes.Count <= 2)
        {
            foreach(GameObject n in nodes)
            {
                for (int i = 0; i < 4; i++)
                {
                    SpawnHitParticle(n);
                }
                    
            }
            GameManager.instance.bossBlobCount--;
            Destroy(transform.parent.gameObject);
        }
        editingNodes = false;
    }

    public void DamageNode(GameObject node, float dmg)
    {
        Node nodeCtrl = node.GetComponent<Node>();
        SpawnHitParticle(node);
        if (nodeCtrl.TakeDamage(dmg))
        {
            DestroyNode(node);
        }
        
    }

    public void SpawnHitParticle(GameObject node)
    {
        GameObject particle = node.GetComponent<Node>().SpawnObjectInTrigger(explodeParticle);
        ParticleSystem.MainModule main = particle.GetComponent<ParticleSystem>().main;
        main.startColor = GetColor();
    }

    public void InitializeSprings()
    {
        
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].GetComponent<SpringJoint>() != null)
            {
                Destroy(nodes[i].GetComponent<SpringJoint>());
            }
            int nextNodeIndex = i + 1;
            if (nextNodeIndex >= nodes.Count)
            {
                nextNodeIndex = 0;
            }
            GameObject neighborNode = nodes[nextNodeIndex];

            SpringJoint springJoint = nodes[i].gameObject.AddComponent<SpringJoint>();
            springJoint.connectedBody = neighborNode.GetComponent<Rigidbody>();
            springJoint.spring = spring; // lol
            springJoint.damper = dampening;
            springJoint.tolerance = tolerance;
        }

        /*
        foreach (SpringJoint spJoint in centerRigidPivot.GetComponents<SpringJoint>())
        {
            Destroy(spJoint);
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            SpringJoint springJoint = centerRigidPivot.gameObject.AddComponent<SpringJoint>();
            springJoint.connectedBody = nodes[i].gameObject.GetComponent<Rigidbody>();
            springJoint.spring = spring;
            springJoint.damper = dampening;
            springJoint.tolerance = tolerance;
        }
       */

    }

    public void InitializeNavAgents()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            NavMeshAgent agent = nodes[i].AddComponent<NavMeshAgent>();
            agent.radius = obstacleAvoidance;
            agent.baseOffset = -0.5f;
            agent.speed = speed;
            agent.acceleration = acceleration;
            agent.updateRotation = false;
            agents.Add(agent);
            nodes[i].GetComponent<Node>().SetAgent(agent);
        }
    }

    public void AssertAgentsList()
    {
        agents = new List<NavMeshAgent>();
        for (int i = 0; i < nodes.Count; i++)
        {
            agents.Add(nodes[i].GetComponent<NavMeshAgent>());
        }
    }

    public void GoToClosestBuilding()
    {
        SetDestination(GameManager.instance.GetClosestBuilding(centerRigidPivot.position).transform.position, 5);
    }

    public void SetDestination(Vector3 destination, float radius) // sends blob to a destination in circle formation with radius around destination
    {
        currentDestination = destination;
        for (int i = 0; i < agents.Count; i++)
        {
            float angle = -(i * Mathf.PI * 2f / agents.Count);
            Vector3 newPos = destination - (new Vector3(Mathf.Cos(angle) * radius, -2, Mathf.Sin(angle) * radius));
            agents[i].SetDestination(newPos);
        }
    }

    public void ColliderUpdate() // adjusts size and orientation of collider under each node to always face and be long enough to reach center rigid pivot point
    {
        foreach(GameObject node in nodes)
        {
            BoxCollider collider = node.GetComponent<Node>().collider.GetComponent<BoxCollider>();
            BoxCollider trigger = node.GetComponent<Node>().trigger.GetComponent<BoxCollider>();

            collider.transform.LookAt(centerRigidPivot);
            collider.transform.eulerAngles = new Vector3(0, collider.transform.eulerAngles.y, 0);
            trigger.transform.LookAt(centerRigidPivot);
            trigger.transform.eulerAngles = new Vector3(0, collider.transform.eulerAngles.y, 0);

            float distance = Vector3.Distance(collider.transform.position, centerRigidPivot.position) - (centerRigidPivot.GetComponent<SphereCollider>().radius * 1.25f);
            if(distance <= 2.0f)
            {
                distance = 2.0f;
            }
            collider.size = new Vector3(0.15f, 1.0f, distance);
            collider.center = new Vector3(0, 0, distance / 2);

            trigger.size = new Vector3(3.0f, nodeMesh.height, distance);
            trigger.center = new Vector3(0, nodeMesh.height / 2, (distance / 2));
        }
    }

    public Vector3 GetCurrentDestination()
    {
        return currentDestination;
    }

    public void RemoveNodeFromList(GameObject node)
    {
        nodes.Remove(node);
    }
}
