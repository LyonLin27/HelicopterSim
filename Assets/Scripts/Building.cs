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

    private float timer = 0.0f;
    private float damageInterval = 0.5f;
    private Animator animator;

    private BoxCollider collider;
    private BoxCollider trigger;
    private NavMeshObstacle obstacle;

    private List<NodeTriggerRelay> overlappedTriggers = new List<NodeTriggerRelay>();
    private List<BlobController> overlappedBlobs = new List<BlobController>();

    private BossBlobController beingAttackedBy = null;

    private bool buildingDestroyed = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        collider = transform.GetChild(0).GetComponentInChildren<BoxCollider>();
        trigger = GetComponent<BoxCollider>();
        obstacle = GetComponent<NavMeshObstacle>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingDestroyed == false)
        {
            if (bossBlobCount > 0 || minionBlobCount > 0)
            {
                timer += Time.deltaTime;
                if (timer >= damageInterval)
                {
                    TakeBossDamage(bossBlobCount);
                    TakeMinionDamage(minionBlobCount);
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
    }

    public void TakeLaserDamage(int dmg){
        if(health - dmg <= 0)
        {
            health = 0;
            DestroyBuilding();
        }
        else
        {
            health -= dmg;
        }
    }

    public void TakeBossDamage(int blobCount)
    {
        float dmg = blobCount;
        if(health - dmg <= 0)
        {
            health = 0;
            DestroyBuilding();
        }
        else
        {
            health -= dmg;
        }
    }

    public void TakeMinionDamage(int minionCount)
    {
        float dmg = 0;
        foreach(BlobController blob in overlappedBlobs)
        {
            if(blob != null)
            {
                dmg += blob.transform.localScale.x;
            }
            
        }
        if(health - dmg <= 0 && buildingDestroyed == false)
        {
            health = 0;
            DestroyBuilding();
        }
        else
        {
            health -= dmg;
        }
    }

    public void DestroyBuilding()
    {
        if(overlappedBlobs.Count > 0)
        {
            foreach(BlobController blob in overlappedBlobs)
            {
                if(blob != null)
                {
                    blob.GoToClosestFeature();
                }
                
            }
            overlappedBlobs.Clear();
        }
        if(beingAttackedBy != null)
        {
            beingAttackedBy.GoToClosestBuilding();
            beingAttackedBy = null;
        }
        
        Instantiate(rubble, transform.position, Quaternion.identity);
        collider.enabled = false;
        trigger.enabled = false;
        obstacle.enabled = false;
        buildingDestroyed = true;
        animator.SetTrigger("Destroy");
        this.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 11) // boss blob trigger layer
        {
            NodeTriggerRelay otherTrigger = other.gameObject.GetComponent<NodeTriggerRelay>();
            if(!overlappedTriggers.Contains(otherTrigger))
            {
                bossBlobCount++;
                overlappedTriggers.Add(otherTrigger);
                if (bossBlobCount > 0)
                {
                    beingAttackedBy = otherTrigger.node.BlobController();
                }
            }
            
        }
        if(other.gameObject.layer == 12)
        {
            BlobController blobCtrl = other.gameObject.GetComponent<BlobController>();
            if (!overlappedBlobs.Contains(blobCtrl))
            {
                minionBlobCount++;
                overlappedBlobs.Add(blobCtrl);
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
