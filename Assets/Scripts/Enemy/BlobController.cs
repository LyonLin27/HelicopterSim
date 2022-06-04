using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class BlobController : EnemyBase
{
    [Header("Settings")]
    public float health = 10;
    public int atkDamage = 1;
    [Header("References")]
    public SkinnedMeshRenderer mesh;
    public GameObject hitParticle;

    private bool walking;
    private NavMeshAgent agentController;
    private Animator animator;
    private Color color;

    private Transform currentDestination;

    private bool isDead = false;

    private void Awake()
    {
        agentController = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetRandomColor();

        Invoke("GoToClosestFeature", 1.0f);
        InvokeRepeating("CheckFoodViable", 5.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!walking && agentController.hasPath == true)
        {
            walking = true;
        }
        else if (walking && agentController.hasPath == false)
        {
            walking = false;
        }

        if (currentDestination == null)
        {
            GoToClosestFeature();
        }

        animator.SetBool("Walking", walking);
    }

    public void SetRandomColor()
    {
        color = new Color(Random.value, Random.value, Random.value);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", color);
        mesh.SetPropertyBlock(block);
    }

    public void SpawnHitParticle()
    {
        Vector3 pos = transform.position;
        GameObject spawnedObj = Instantiate(hitParticle, pos, Quaternion.identity, transform);
        spawnedObj.transform.SetParent(transform);

        spawnedObj.transform.localPosition = new Vector3(0, 0, 0);
        spawnedObj.transform.eulerAngles = new Vector3(-90, 0, 0);
        spawnedObj.transform.SetParent(null);

        ParticleSystem.MainModule main = spawnedObj.GetComponent<ParticleSystem>().main;
        main.startColor = GetColor();
    }

    public void GoToClosestFeature()
    {
        Building closestBuilding = GameManager.instance.GetClosestBuilding(transform.position);
        BlobFood closestFood = GameManager.instance.GetClosestBlobFood(transform.position);

        if (transform.localScale.x <= 20 && Vector3.Distance(transform.position, closestFood.transform.position) <= Vector3.Distance(transform.position, closestBuilding.transform.position))
        {
            SetDestination(closestFood.transform.position);
            currentDestination = closestFood.transform;
        }
        else
        {
            SetDestination(closestBuilding.transform.position);
            currentDestination = closestBuilding.transform;
        }
    }

    public void CheckFoodViable()
    {
        if(currentDestination != null && currentDestination.GetComponent<BlobFood>() != null)
        {
            BlobFood food = currentDestination.GetComponent<BlobFood>();
            if (food.currentMinionBlob == this || food.currentMinionBlob == null)
            {
                return;
            }
            else
            {
                GoToClosestFeature();
            }
        }
    }

    public void SetDestination(Vector3 position)
    {
        agentController.SetDestination(position);
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
        health = scale * 50f;
    }

    public override void TakeDamage(PlayerProjInfo projInfo)
    {
        SpawnHitParticle();
        if(health - projInfo.damage <= 0)
        {
            health = 0;
            Die();
        }
        else
        {
            health -= projInfo.damage;
        }
    }

    public void Die()
    {
        if (isDead)
            return;
        Destroy(gameObject);
        GameManager.instance.AddScore(10 * Mathf.CeilToInt(transform.localScale.x));

        isDead = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10) // if bullet layer
        {
            Bullet bullet = other.GetComponent<Bullet>();
            TakeDamage(bullet.GetProjInfo());
            bullet.OnCollisionEnter(null);
        }

        Missile ms = other.GetComponent<Missile>();
        if(ms){
            TakeDamage(ms.GetProjInfo());
            ms.OnCollisionEnter(null);
        }
    }

	private void OnTriggerStay(Collider other)
	{
        if (other.GetComponent<Building>())
        {
            Building building = other.GetComponent<Building>();
            if (building.Destroyed)
            {
                if (building.transform == currentDestination)
                    GoToClosestFeature();
            }
            else
            {
                building.TakeContDamage((int)transform.localScale.x * atkDamage);
            }
        }
    }
}
