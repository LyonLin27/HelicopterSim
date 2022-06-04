using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : EnemyBase
{
    [Header("Refs")]
    [SerializeField] private ParticleSystem explodsion1;
    [SerializeField] private ParticleSystem explodsion2;
    [SerializeField] private GameObject shipModel;
    [SerializeField] private GameObject chargeSphere;
    [SerializeField] private GameObject beamPrefab;
    [SerializeField] private AudioSource laserShotSound;

    [Header("Settings")]
    public float health = 100f;
    public float speed = 50f;
    public float approachDistMin = 20f;
    public float approachDistMax = 200f;
    public float minHeight = 60;
    public float maxHeight = 120;
    public float chargeTime = 10f;
    public float shootTime = 1f;
    public float moveDistMin = 10f;
    public float moveDistMax = 70f;
    
    [HideInInspector]
    public Transform target = null;
    
    private bool isDead = false;

    private enum UFOState {
        Approach,
        Attack,
        Move
    }
    private UFOState state = UFOState.Approach;
    private Rigidbody rb;
    private float randFloat;
    private Vector3 randVec3 = Vector3.zero;
    private float stateStartTime = 0f;
    private Vector3 stateStartPos = Vector3.zero;
    private bool stateFirstFrame = true;
    private Vector3 dmgHitPoint = Vector3.zero;
    private bool interupted = false;
    private int bulletShot = 0;
    private bool hasShoot = false;
    private bool targetingPlayer = false;

    private float forceMoveTime = 0f;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        randFloat = Random.value;
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchState(UFOState.Approach);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) {
            return;
        }

        if (target == null) {
            PickTarget();
            SwitchState(UFOState.Approach);
        }
        else if (target.GetComponent<Building>() && target.GetComponent<Building>().health <= 0) {
            PickTarget();
            SwitchState(UFOState.Approach);
        }

        // handle movement
        switch (state) {
            case UFOState.Approach:
                Vector3 offset = Vector3.zero;
                if (!targetingPlayer) {
                    offset = new Vector3(0f, 110 + randFloat * 10f, 0f);
                }

                if (Vector3.Distance(target.position + offset, transform.position) < approachDistMin + (approachDistMax-approachDistMin)*randFloat) {
                    SwitchState(UFOState.Attack);
                    break;
                }
                rb.velocity = (target.position + offset - transform.position).normalized * speed*5f;

                break;
            case UFOState.Attack:
                if (stateFirstFrame) {
                    bulletShot = 0;
                    hasShoot = false;
                    interupted = false;
                    stateFirstFrame = false;
                }

                // charge
                if (!interupted) {
                    if (Time.time - stateStartTime < chargeTime) {
                        chargeSphere.transform.localScale += Vector3.one * 0.2f * Time.deltaTime;
                    }
                    else if (Time.time - stateStartTime < chargeTime +shootTime) {
                        chargeSphere.transform.localScale = Vector3.zero;
                        if (Time.time - stateStartTime > chargeTime + 0.2f * bulletShot && !hasShoot) {
                            bulletShot++;
                            if(bulletShot > 5)
                                hasShoot = true;
                            GameObject beam = Instantiate(beamPrefab, chargeSphere.transform.position, Quaternion.identity);
                            beam.transform.up = target.position - beam.transform.position;
                            laserShotSound.Play();
                        }
                    }
                    else {
                        chargeSphere.transform.localScale = Vector3.zero;
                    }
                }
                else {
                    chargeSphere.transform.localScale = Vector3.zero;
                }

                if (Time.time - stateStartTime > chargeTime + shootTime) {
                    chargeSphere.transform.localScale = Vector3.zero;
                    SwitchState(UFOState.Move);
                }
                break;
            case UFOState.Move:
                if (stateFirstFrame) {
                    Ray ray = new Ray(transform.position, randVec3);
                    if (Physics.Raycast(ray, moveDistMin + (moveDistMax - moveDistMin) * randFloat)) {
                        SwitchState(UFOState.Move); // find another direction
                        break;
                    }
                    // if already too close to earth and try to get even closer
                    if (transform.position.y < minHeight && Vector3.Dot(-transform.up, randVec3) > 0f) {
                        SwitchState(UFOState.Move); // find another direction
                        break;
                    }
                    else if (transform.position.y > maxHeight && Vector3.Dot(-transform.up, randVec3) < 0f) {
                        SwitchState(UFOState.Move); // find another direction
                        break;
                    }
                    stateFirstFrame = false;
                }
                Vector3 targetPos = stateStartPos + randVec3 * (moveDistMin + (moveDistMax - moveDistMin) * randFloat);
                rb.velocity = (targetPos - transform.position).normalized * speed;

                if (Vector3.Distance(targetPos, transform.position) < 2f) {
                    SwitchState(UFOState.Attack);
                }
                break;
            default:
                break;

        }

    }

    private void PickTarget() {
        if (Random.value < 0.2f) {
            target = GameManager.instance.player.transform;
            targetingPlayer = true;
        }
        else {
            target = GameManager.instance.GetRandBuilding().transform;
            targetingPlayer = false;
        }
    }

    private void SwitchState(UFOState newState) {
        randFloat = Random.value;
        randVec3 = new Vector3(Random.value-0.5f, Random.value - 0.5f, Random.value - 0.5f).normalized;
        state = newState;
        stateStartTime = Time.time;
        stateStartPos = transform.position;
        stateFirstFrame = true;
    }

    public override void TakeDamage(PlayerProjInfo projInfo)
    {
        dmgHitPoint = projInfo.pos;
        health -= projInfo.damage;
        interupted = true;
        Invoke("ApplyImpact", 0.2f);
        if (health <= 0) {
            DestroyUFO();
        }
    }

    private void ApplyImpact() {
        Vector3 forceDir = (transform.position - dmgHitPoint).normalized;
        rb.AddForce((transform.position - dmgHitPoint).normalized * 100f, ForceMode.Force);
        GetComponent<Rigidbody>().AddForce(forceDir * 100f);
    }

    private void DestroyUFO() {
        if (isDead)
            return;
        // play particle
        // add force
        // play particle
        explodsion1.Play();
        //GetComponentInParent<EnemyManager>().targetList.Remove(this.transform);
        Vector3 forceDir = (transform.position - dmgHitPoint).normalized;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        GetComponent<Rigidbody>().AddForce(forceDir * 100f, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddTorque((Random.value-0.5f) * 1500f, (Random.value - 0.5f) * 500f, (Random.value - 0.5f) * 1500f, ForceMode.Impulse);
        GetComponent<Rigidbody>().useGravity = true;

        Invoke("DestroyUFOEnd", 2f);

        GameManager.instance.AddScore(50);
        isDead = true;
    }

    private void DestroyUFOEnd() {
        explodsion2.transform.position = shipModel.transform.position;
        explodsion2.Play();
        shipModel.SetActive(false);
        GameManager.instance.UFOcount--;
        Invoke("CleanUp", 4f);
    }

    private void CleanUp() {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.GetComponent<PlayerProjBase>()) {
            TakeDamage(collision.collider.GetComponent<PlayerProjBase>().GetProjInfo());
        }
        else {
            SwitchState(UFOState.Move);
            forceMoveTime = Time.time;
        }
    }

    private void OnCollisionStay(Collision collision){
        if (!collision.collider.GetComponent<Bullet>()) {
            if(Time.time - forceMoveTime > 1f){
                SwitchState(UFOState.Move);
                forceMoveTime = Time.time;
            }
        }
    }
}
