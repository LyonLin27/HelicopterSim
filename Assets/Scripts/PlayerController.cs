﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Ref")]
    public AudioSource bladesAudio;
    public List<GameObject> hideWhenDistroy;
    public GameObject blades;
    public GameObject rearBlades;
    public Transform camP1;
    public Transform camP2;
    public Transform camPoint;
    public Transform camAimPoint;
    public Transform cam;
    public ParticleSystem dustParticle;
    public GameObject explosion;
    private Rigidbody rb;
    private PlayerInput pi;

    // debug
    public TextMeshProUGUI field1;
    public TextMeshProUGUI field2;
    public TextMeshProUGUI field3;
    public TextMeshProUGUI field4;
    public TextMeshProUGUI field5;
    public TextMeshProUGUI field6;

    [Header("Weapons")]
    public LockOnSystem lockOnSystem;
    public List<MachineGun> machineGuns;
    public List<MissileLauncher> missileLaunchers;
    public float mgInterval = 0.1f;
    public float msInterval = 0.5f;
    public float msWaveInterval = 2f;
    public int msWaveCount = 0;
    public int msWaveMax = 4;

    [Header("Settings")]
    public float acc = 10f;
    public float dcc = 10f;
    public float res = 1f;
    public float tltAcc = 5f;
    public float tltMax = 60f;
    public float rotAcc = 5f;
    public int HP = 100;

    // key variables
    private float currBladeSpd = 0f;
    private float acc_pow;
    private float dcc_pow;
    private float res_pow;

    // weapon related
    private float lastFireTime = 0f;
    private float lastMissileFireTime = 0f;

    // camera control
    private enum CameraType
    {
        locked,
        free,
        forward,
        aim
    }

    private CameraType camType;

    private bool controllable = false;
    private float spawnTime = 0f;

    private void Awake() {

        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0f, -30f, 0f);

        camType = CameraType.locked;
        field5.text = camType.ToString();

        foreach (MachineGun mg in machineGuns) {
            mg.heliRB = rb;
        }

        foreach (MissileLauncher ms in missileLaunchers) {
            ms.heliRB = rb;
        }
        spawnTime = Time.time;
    }

    private void Start() {
        if(GameManager.instance)
            GameManager.instance.player = this;
        //Controllable(false);

        if(GameManager.instance)
            pi = GameManager.instance.playerInput;
        if (pi == null)
        {
            pi = new PlayerInput();
        }
        pi.actions.HeliController.MenuAction.performed += ctx => HandleMenuInput();
    }

	private void OnDestroy()
	{
        pi.actions.HeliController.MenuAction.performed -= ctx => HandleMenuInput();
    }

	private void Update() {
        HandleCameraUpdate();

        pi.Tick();

        //if(controllable)
        //{
        // weapons
        if (pi.mg_in && Time.time - lastFireTime > mgInterval)
            {
                foreach (MachineGun mg in machineGuns)
                {
                    mg.Fire();
                }
                lastFireTime = Time.time;
            }
            if(pi.ms_in && Time.time - lastMissileFireTime > msInterval && msWaveCount < msWaveMax){
                MissileLauncher ms = missileLaunchers[msWaveCount % 2];
                ms.Fire(lockOnSystem.GetLockedTarget(msWaveCount));
                msWaveCount++;
                lastMissileFireTime = Time.time;
            }
            if(Time.time - lastMissileFireTime > msWaveInterval){
                msWaveCount = 0;
            }
        //}

        // dust particle
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);
        if (hit.collider) {
            // play particle
            dustParticle.transform.position = hit.point;
            float dustStr = (currBladeSpd / 1500) / (hit.distance / 20f);
            ParticleSystem.ShapeModule shape = dustParticle.shape;
            shape.radius = dustStr/4f;
            dustParticle.startSize = dustStr/30f;
            if(!dustParticle.isPlaying)
                dustParticle.Play();
        }
        else {
            dustParticle.Stop();
        }

    }

    private void FixedUpdate() {
        //if(controllable)
        //{
            ModifyAccPow();
            currBladeSpd += (pi.acc_parsed * acc_pow - pi.dcc_parsed * dcc_pow) * Time.fixedDeltaTime;
            currBladeSpd += (currBladeSpd > 0 ? -res_pow : 0) * Time.fixedDeltaTime;
            if (currBladeSpd < 0)
                currBladeSpd = 0;
            field1.text = ((int)(pi.acc_parsed * acc_pow - pi.dcc_parsed * dcc_pow + (currBladeSpd > 0 ? -res_pow : 0))).ToString();
            field2.text = ((int)currBladeSpd).ToString();
            field3.text = ((int)transform.position.y).ToString();
            field4.text = HP.ToString();

            Vector3 currRot = blades.transform.localEulerAngles;
            Vector3 currRearRot = rearBlades.transform.localEulerAngles;
            blades.transform.localEulerAngles = new Vector3(0f, 0f, currRot.z + currBladeSpd * Time.fixedDeltaTime);
            rearBlades.transform.localEulerAngles = new Vector3(0f, 0f, currRearRot.z + currBladeSpd * 0.5f * Time.fixedDeltaTime);
            rb.AddForce(-blades.transform.forward * currBladeSpd * Time.fixedDeltaTime, ForceMode.Acceleration);

            Vector3 torque = new Vector3(pi.tlt_parsed.y * tltAcc, -pi.tlt_parsed.x * tltAcc, 0f);
            rb.AddRelativeTorque(torque);
            rb.AddTorque(0f, pi.rot_parsed * rotAcc, 0f);

            // stablizer
            if (pi.isStablized)
            {
                Vector3 currTlt = rb.rotation.eulerAngles;
                if (currTlt.x > 270f)
                {

                }
                else
                {
                    currTlt.x = Mathf.Lerp(currTlt.x, 90f, Time.fixedDeltaTime * 2f);
                }
                rb.rotation = Quaternion.Euler(currTlt);
            }
        //}

        HandleCameraFixedUpdate();

        bladesAudio.pitch = Mathf.Clamp(currBladeSpd / 1500, 0, 2f);
    }

    private void HandleCameraUpdate() {
        if (pi.aim_in || camType == CameraType.aim)
        {
            camType = pi.aim_in ? CameraType.aim : CameraType.locked;
        }

        switch (camType)
        {
            case CameraType.locked:

                break;

            case CameraType.free:
                float targetRotX = -90f * pi.cam_in.x;
                float lerpResultX = Mathf.LerpAngle(camP1.transform.localEulerAngles.z, targetRotX, 0.1f);
                camP1.localEulerAngles = new Vector3(0f, 0f, lerpResultX);
                float targetRotY = -90f * pi.cam_in.y;
                float lerpResultY = Mathf.LerpAngle(camP2.transform.localEulerAngles.x, targetRotY, 0.1f);
                camP2.localEulerAngles = new Vector3(lerpResultY, 0f, 0f);

                pi.rot_in = 0f;

                break;

            case CameraType.forward:
                Vector3 moveDir = rb.velocity.normalized;
                camP1.up = moveDir;
                break;

            case CameraType.aim:
                break;
        }
    }

    private void HandleMenuInput()
    {
        if (!GameManager.instance.playerTakeOff)
            return;

        if (pi.menu_in.y > 0.5f)
        {
            if (camType != CameraType.aim)
            {
                if (camType != CameraType.free)
                    camType = CameraType.free;
                else
                    camType = CameraType.locked; // default
            }

            field5.text = camType.ToString();
        }
    }

    private void HandleCameraFixedUpdate()
    {
        if (camType == CameraType.aim)
        {
            cam.position = camAimPoint.position;
            cam.rotation = camAimPoint.rotation;
        }
        else
        {
            cam.position = Vector3.Lerp(cam.position, camPoint.position, Time.fixedDeltaTime * 10f);
            cam.rotation = Quaternion.Lerp(cam.rotation, camPoint.rotation, Time.fixedDeltaTime * 10f);
        }

    }

    private void ModifyAccPow() {
        if (currBladeSpd > 2000) {
            acc_pow = acc * (1 - (currBladeSpd - 2000) / 1000);
            dcc_pow = dcc * (1 + (currBladeSpd - 2000) / 1000);
            res_pow = res * (1 + (currBladeSpd - 2000) / 1000);
        }
        else if (currBladeSpd <= 2000 && currBladeSpd > 1000) {
            acc_pow = acc;
            dcc_pow = dcc;
            res_pow = res;
        }
        else if (currBladeSpd < 1000) {
            acc_pow = acc * 1.5f;
            dcc_pow = dcc;
            res_pow = res / 1.5f;
        }
    }

    public void Controllable(bool condition)
    {
        controllable = condition;

        if (condition)
        {
            rb.useGravity = true;
        }
        else
        {
            rb.useGravity = false;
        }
    }

    public float GetBladeSpd(){
        return currBladeSpd;

    }

    public void TakeDamage(int dmg){
        // bug collision happens in the beginning
        if(Time.time - spawnTime < 5f){
            return;
        }
        if(HP <= 0)
            return;
        HP -= dmg;
        field4.text = HP.ToString();
        if(HP <= 0){
            controllable = false;
            camPoint.parent = transform.parent;
            // ins explo
            Instantiate(explosion, transform.position, transform.rotation);
            // destory self
            foreach(GameObject obj in hideWhenDistroy){
                obj.SetActive(false);
            }
            rb.isKinematic = true;
            bladesAudio.Stop();
            dustParticle.Stop();
            GameManager.instance.HandlePlayerDeath();
        }
    }

    private void OnCollisionEnter(Collision coll){
        float impulse = coll.impulse.magnitude;
        print("impulse: " + impulse);
        if(Vector3.Dot(-transform.forward, Vector3.up) <=0){
            TakeDamage((int)(impulse/10));

        }else{
            impulse -= 500;
            if(impulse > 0)
                TakeDamage((int)(impulse/10));
        }
    }

    private void OnCollisionStay(Collision coll){
        float impulse = coll.impulse.magnitude;
        if(Vector3.Dot(-transform.forward, Vector3.up) <=0){
            TakeDamage((int)(impulse));
        }else{
            impulse -= 100;
            if(impulse > 0)
                TakeDamage((int)(impulse));
        }
    }

    public void TakeImpact(Vector3 impactPoint, Vector3 force)
    {
        force.y *= 0.2f;
        impactPoint.y = rb.position.y * 0.8f + impactPoint.y * 0.2f;
        rb.AddForceAtPosition(force, impactPoint, ForceMode.Impulse);
    }
}
