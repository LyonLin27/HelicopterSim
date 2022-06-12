using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Type")]
    public HeliType heliType;

    [Header("Ref")]
    public AudioSource bladesAudio;
    public List<GameObject> hideWhenDistroy;
    public GameObject blades;
    public GameObject rearBlades;
    public HUDManager hud;
    public Transform camP1;
    public Transform camP2;
    public Transform camPoint;
    public Transform camAimPoint;
    public Transform cam;
    public ParticleSystem dustParticle;
    public GameObject explosion;
    public Transform balanceTarget;
    private Rigidbody rb;
    private PlayerInput pi;

    [Header("Weapons")]
    public List<MachineGun> machineGuns;
    public List<MissileLauncher> missileLaunchers;
    public float mgInterval = 0.1f;
    public float msInterval = 0.5f;
    public float msWaveInterval = 2f;
    public int msWaveCount = 0;
    public int msWaveMax = 4;
    public float machineGunBulletSpd = 200f;

    // control type
    public enum ControlType
    {
        Manual,
        AutoHover
    }
    private ControlType controlType;

    // key variables
    [HideInInspector] public int HP;
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

    private float spawnTime = 0f;

    private void Awake() {

        HP = heliType.HP;

        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0f, -30f, 0f);
        rb.mass = heliType.mass;

        controlType = ControlType.Manual;
        camType = CameraType.locked;
        hud.SetDebugInfo(5, camType.ToString());

        foreach (MachineGun mg in machineGuns) {
            mg.heliRB = rb;
        }

        foreach (MissileLauncher ms in missileLaunchers) {
            ms.heliRB = rb;
        }
        spawnTime = Time.time;
    }

    private void Start() {
		if (GameManager.instance)
			GameManager.instance.player = this;

		if (GameManager.instance)
            pi = GameManager.instance.playerInput;
        if (pi == null)
        {
            pi = new PlayerInput();
        }
        pi.actions.HeliController.MenuAction.performed += ctx => HandleMenuInput();

        foreach (var gun in machineGuns)
            gun.SetupWeapon(machineGunBulletSpd);
    }

	private void OnDestroy()
	{
        pi.actions.HeliController.MenuAction.performed -= ctx => HandleMenuInput();
    }

	private void Update() {
        HandleCameraUpdate();

        pi.Tick();

        HandleWeaponUpdate();

        HandleDustParticle();

        HandleAudioUpdate();
    }

    private void FixedUpdate() {
        HandlePhysicsFixedUpdate();

        HandleCameraFixedUpdate();
    }

	#region Update Helpers

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
                float targetRotX = 90f * pi.cam_in.x;
                float lerpResultX = Mathf.LerpAngle(camP1.transform.localEulerAngles.y, targetRotX, 0.1f);
                camP1.localEulerAngles = new Vector3(0f, lerpResultX, 0f);
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

    private void HandleWeaponUpdate()
    {
        if (pi.mg_in && Time.time - lastFireTime > mgInterval)
        {
            foreach (MachineGun mg in machineGuns)
            {
                mg.Fire();
            }
            lastFireTime = Time.time;
        }
        if (pi.ms_in && Time.time - lastMissileFireTime > msInterval && msWaveCount < msWaveMax)
        {
            MissileLauncher ms = missileLaunchers[msWaveCount % 2];
            ms.Fire(hud.lockOnSystem.GetLockedTarget(msWaveCount));
            msWaveCount++;
            lastMissileFireTime = Time.time;
        }
        if (Time.time - lastMissileFireTime > msWaveInterval)
        {
            msWaveCount = 0;
        }
    }

    private void HandleDustParticle()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit);
        if (hit.collider)
        {
            // play particle
            dustParticle.transform.position = hit.point;
            float dustStr = (currBladeSpd / heliType.highForceThreshold) / (hit.distance / 20f);
            ParticleSystem.ShapeModule shape = dustParticle.shape;
            shape.radius = dustStr / 4f;
            dustParticle.startSize = dustStr / 30f;
            if (!dustParticle.isPlaying)
                dustParticle.Play();
        }
        else
        {
            dustParticle.Stop();
        }
    }

    private void HandleAudioUpdate()
    {
        bladesAudio.pitch = Mathf.Clamp(currBladeSpd / heliType.highForceThreshold * 0.7f, 0, 2f);
    }

    #endregion Update Helpers

    #region Fixed Update Helpers

    private void HandlePhysicsFixedUpdate()
    {
        switch (controlType)
        {
            case ControlType.Manual:
                ModifyAccPow();
                ApplyBladesSpd(CalcDeltaBladeSpd());
                ApplyTilting();
                if (pi.isStablized)
                    Stablize();
                break;
            case ControlType.AutoHover:
                pi.isStablized = true;
                ApplyBladesSpd(CalcDeltaBladeSpd());
                ApplyTilting();
                Stablize(2f);
                break;
        }
        ApplyGroundEffect();
    }

    private void ModifyAccPow()
    {
        if (currBladeSpd > heliType.highForceThreshold)
        {
            acc_pow = heliType.acc * (1 - (currBladeSpd - heliType.highForceThreshold) / heliType.highForceDropRange);
            dcc_pow = heliType.dcc * (1 + (currBladeSpd - heliType.highForceThreshold) / heliType.highForceDropRange);
            res_pow = heliType.res * (1 + (currBladeSpd - heliType.highForceThreshold) / heliType.highForceDropRange);
        }
        else if (currBladeSpd <= heliType.highForceThreshold && currBladeSpd > heliType.lowForceThreshold)
        {
            acc_pow = heliType.acc;
            dcc_pow = heliType.dcc;
            res_pow = heliType.res;
        }
        else if (currBladeSpd < heliType.lowForceThreshold)
        {
            acc_pow = heliType.acc * 1.5f;
            dcc_pow = heliType.dcc;
            res_pow = heliType.res / 1.5f;
        }
    }

    private float CalcDeltaBladeSpd()
    {
        if (controlType == ControlType.AutoHover)
        {
            float maxDeltaSpd = heliType.acc - heliType.res;
            float ratio = Mathf.Clamp(rb.velocity.y / -5f, -1f, 1f);
            float controlledRatio = 0.3f;
            float controlledSpd = pi.acc_parsed * acc_pow - pi.dcc_parsed * dcc_pow;
            return maxDeltaSpd * ratio + pi.acc_parsed + controlledSpd * controlledRatio;
        }
        else 
        {
            float accSpd = pi.acc_parsed * acc_pow - pi.dcc_parsed * dcc_pow;
            float resSpd = currBladeSpd > 0 ? -res_pow : 0;
            return accSpd + resSpd;
        }
    }

    private void ApplyBladesSpd(float deltaBladeSpd)
    {
        currBladeSpd += deltaBladeSpd * Time.fixedDeltaTime;

        if (currBladeSpd < 0)
            currBladeSpd = 0;

        Vector3 currRot = blades.transform.localEulerAngles;
        Vector3 currRearRot = rearBlades.transform.localEulerAngles;
        float deltaBladeAngle = currBladeSpd * 2000f / heliType.highForceThreshold;
        blades.transform.localEulerAngles = Vector3.up * (currRot.y + deltaBladeAngle * Time.fixedDeltaTime);
        rearBlades.transform.localEulerAngles = new Vector3(0f, 0f, currRearRot.z + deltaBladeAngle * 0.5f * Time.fixedDeltaTime);
        rb.AddForce(blades.transform.up * currBladeSpd * Time.fixedDeltaTime, heliType.forceMode);

        hud.SetDebugInfo(0, deltaBladeSpd.ToString("0"));
        hud.SetDebugInfo(1, currBladeSpd.ToString("0"));
        hud.SetDebugInfo(2, string.Format("{0} / {1}",
            new Vector2(rb.velocity.x, rb.velocity.z).magnitude.ToString("0.0"), rb.velocity.y.ToString("0.0")));
        hud.SetDebugInfo(3, transform.position.y.ToString("0"));
        hud.SetDebugInfo(4, HP.ToString("0"));
    }

    private void ApplyTilting()
    {
        float xTorque = pi.tlt_parsed.y * heliType.tltAcc;
        float yTorque = pi.rot_parsed * heliType.rotAcc;
        float zTorque = -pi.tlt_parsed.x * heliType.tltAcc;

        Vector3 torque = new Vector3(xTorque, yTorque, zTorque);
        rb.AddRelativeTorque(torque);
    }

    private void Stablize(float stabMult = 1f)
    {
        Vector3 currTlt = rb.rotation.eulerAngles;
        currTlt = NormalizeEulerAngle(currTlt);

        if (Mathf.Abs(currTlt.x) < 90f && Mathf.Abs(currTlt.z) < 90f)
        {
            float angleDiff = Vector3.Angle(transform.up, Vector3.up);
            Vector3 axis = Vector3.Cross(transform.up, Vector3.up);

            transform.Rotate(axis, angleDiff * Time.fixedDeltaTime * heliType.balanceTorque * stabMult, Space.World);
        }
    }

    private void ApplyGroundEffect()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit);
        if (hit.collider)
        {
            float groundEffectMag = currBladeSpd * 0.1f / (hit.distance / 20f);
            groundEffectMag = Mathf.Clamp(groundEffectMag, 0f, currBladeSpd * 0.1f);
            rb.AddForce(blades.transform.up * groundEffectMag * Time.fixedDeltaTime, heliType.forceMode);
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

	#endregion Fixed Update Helpers

	#region Input Callbacks

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

            hud.SetDebugInfo(5, camType.ToString());
        }
        if (pi.menu_in.y < -0.5f)
        {
            if (controlType == ControlType.Manual)
                controlType = ControlType.AutoHover;
            else
                controlType = ControlType.Manual;
        }
    }

	#endregion Input Callbacks

	#region public getters

    public float GetBladeSpd(){
        return currBladeSpd;
    }

    public float GetVerticalSpd()
    {
        return rb.velocity.y;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

	#endregion public getters

	#region public modifider

	public void TakeDamage(int dmg){
        // bug collision happens in the beginning
        if(Time.time - spawnTime < 5f){
            return;
        }
        if(HP <= 0)
            return;
        HP -= dmg;
        hud.SetDebugInfo(4, HP.ToString("0"));
        if (HP <= 0){
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

    public void TakeImpact(Vector3 impactPoint, Vector3 force)
    {
        force.y *= 0.2f;
        impactPoint.y = rb.position.y * 0.8f + impactPoint.y * 0.2f;
        rb.AddForceAtPosition(force, impactPoint, ForceMode.Impulse);
    }

	#endregion public modifider

	#region Collisions

	private void OnCollisionEnter(Collision coll){
        float impulse = coll.impulse.magnitude;
        print("impulse: " + impulse);
        if(Vector3.Dot(transform.up, Vector3.up) <=0){
            TakeDamage((int)(impulse/10));

        }else{
            impulse -= (4f * heliType.mass);
            if(impulse > 0)
                TakeDamage((int)(impulse/10));
        }
    }

    private void OnCollisionStay(Collision coll){
        float impulse = coll.impulse.magnitude;
        if(Vector3.Dot(transform.up, Vector3.up) <=0){
            TakeDamage((int)(impulse));
        }else{
            impulse -= 100;
            if(impulse > 0)
                TakeDamage((int)(impulse));
        }
    }

	#endregion Collisions

	#region Helpers

	private Vector3 NormalizeEulerAngle(Vector3 angle)
    {
        while (angle.x > 180) angle.x -= 360f;
        while (angle.y > 180) angle.y -= 360f;
        while (angle.z > 180) angle.z -= 360f;

        while (angle.x < -180) angle.x += 360f;
        while (angle.y < -180) angle.y += 360f;
        while (angle.z < -180) angle.z += 360f;

        return angle;
    }

	#endregion Helpers
}
