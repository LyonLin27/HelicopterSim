using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Building : MonoBehaviour
{
    public float health = 100;

    public GameObject rubble;
    public int bossBlobCount = 0;
    public int minionBlobCount = 0;

    private float damageCurrFrame = 0f;
    private Animator animator;

    private BoxCollider collider;
    private BoxCollider trigger;
    private NavMeshObstacle obstacle;

    public bool Destroyed { get { return buildingDestroyed; } }
    private bool buildingDestroyed = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        collider = transform.GetChild(0).GetComponentInChildren<BoxCollider>();
        trigger = GetComponent<BoxCollider>();
        obstacle = GetComponent<NavMeshObstacle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingDestroyed == false)
        {
            if (damageCurrFrame > 0)
            {
                health -= damageCurrFrame * Time.deltaTime;
                damageCurrFrame = 0;
            }

            if (health <= 0)
            {
                DestroyBuilding();
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        damageCurrFrame += dmg;
    }

    private void DestroyBuilding()
    {
        Instantiate(rubble, transform.position, Quaternion.identity);
        collider.enabled = false;
        trigger.enabled = false;
        obstacle.enabled = false;
        buildingDestroyed = true;
        animator.SetTrigger("Destroy");
        this.enabled = false;
    }
}
