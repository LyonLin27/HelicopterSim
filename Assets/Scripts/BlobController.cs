using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class BlobController : MonoBehaviour
{
    [Header("Settings")]
    public float health = 10;
    [Header("References")]
    public SkinnedMeshRenderer mesh;
    public GameObject hitParticle;
    private bool walking;
    private NavMeshAgent agentController;
    private Animator animator;
    private Color color;

    private BlobFood destinationFood;

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

        if (transform.localScale.x <= 15 && Vector3.Distance(transform.position, closestFood.transform.position) <= Vector3.Distance(transform.position, closestBuilding.transform.position))
        {
            SetDestination(closestFood.transform.position);
            destinationFood = closestFood;
        }
        else
        {
            SetDestination(closestBuilding.transform.position);
        }
        
    }

    public void CheckFoodViable()
    {
        if(destinationFood != null)
        {
            if(destinationFood.currentMinionBlob == this || destinationFood.currentMinionBlob == null)
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
        health = scale * 20f;
    }

    public void Damage(float amount)
    {
        SpawnHitParticle();
        if(health - amount <= 0)
        {
            health = 0;
            Die();
        }
        else
        {
            health -= amount;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10) // if bullet layer
        {
            Bullet bullet = other.GetComponent<Bullet>();
            Damage(bullet.damage);
            bullet.OnCollisionEnter(null);
        }

        Missile ms = other.GetComponent<Missile>();
        if(ms){
            Damage(ms.damage);
            ms.OnCollisionEnter(null);
        }
    }
}
