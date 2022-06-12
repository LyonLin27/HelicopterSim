using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserController : MonoBehaviour
{
    [HideInInspector] public Transform _navTarget;

    [SerializeField] private float _maxSpd;
    [SerializeField] private float _maxForce;
    [SerializeField] private float _maxTorque;
    [SerializeField] private float _maxAngularSpd = 10f;
    [SerializeField] private float _maxTorqueAngle = 30f;
    [SerializeField] private float _balanceTorque = 300f;

    [SerializeField] private float _navReachRange = 30f;
    [SerializeField] private LayerMask _navValidCheckMask;

    private float _vertDecelerateRange = 30f;

    private Rigidbody _rb;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _navTarget = transform.parent.Find("NavTarget");
    }

	private void Update()
	{
        if (Vector3.Distance(transform.position, _navTarget.position) < _navReachRange)
        {
            // randomly set _navTarget
            Vector3 newNavPos = _navTarget.position + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * Random.Range(50f, 100f);

            bool newNavPosValid = false;
            Ray ray = new Ray(transform.position, newNavPos - transform.position);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Vector3.Distance(newNavPos, transform.position), _navValidCheckMask);
            if (!hit.collider)
            {
                newNavPosValid = true;
            }

            if(newNavPosValid)
                _navTarget.position = newNavPos;

            Debug.DrawRay(ray.origin, ray.direction * Vector3.Distance(newNavPos, transform.position), newNavPosValid ? Color.green : Color.red, 1f);
        }
    }


	private void FixedUpdate()
	{
        HandleSteering();

        Stablize();
	}

    private void HandleSteering()
    {
        // calc hori and verti steering
        Vector3 horiNavPos = _navTarget.position;
        horiNavPos.y = transform.position.y;

        float vertNavOffset = _navTarget.position.y - transform.position.y;

        // apply steering to rb
        Vector3 eularRot = Quaternion.FromToRotation(horiNavPos - transform.position, transform.forward).eulerAngles;
        if (eularRot.y > 180f)
            eularRot.y -= 360f;

        float yTorqueDir = -1f * Mathf.Sign(eularRot.y);
        float yTorqueRate = Mathf.Clamp(Mathf.Abs(eularRot.y), 0f, _maxTorqueAngle) / _maxTorqueAngle;
        float yTorque = _maxTorque * yTorqueDir * yTorqueRate;
        float angularSpdCap = Mathf.Deg2Rad * _maxAngularSpd * yTorqueRate;

        if (_rb.angularVelocity.magnitude > angularSpdCap)
        {
            _rb.angularVelocity = _rb.angularVelocity.normalized * angularSpdCap;
        }
        _rb.AddTorque(0f, yTorque * Time.fixedDeltaTime, 0f, ForceMode.Acceleration);

        // hori force
        float forceRate = 1f; // decrease by dist
        Vector3 forwardForce = transform.forward * _maxForce * forceRate;
        _rb.AddForce(forwardForce * Time.fixedDeltaTime, ForceMode.Acceleration);

        // vert force
        float vertForceRate = Mathf.Clamp01(Mathf.Abs(vertNavOffset) / _vertDecelerateRange);
        float vertForce = Mathf.Sign(vertNavOffset) * _maxForce * 0.5f * vertForceRate;
        _rb.AddForce(0f, vertForce * Time.fixedDeltaTime, 0f);

        if (_rb.velocity.magnitude > _maxSpd)
        {
            _rb.velocity = _rb.velocity.normalized * _maxSpd;
        }
    }

    private void Stablize(float stabMult = 1f)
    {
        Vector3 currTlt = _rb.rotation.eulerAngles;
        currTlt = NormalizeEulerAngle(currTlt);

        if (Mathf.Abs(currTlt.x) < 90f && Mathf.Abs(currTlt.z) < 90f)
        {
            float angleDiff = Vector3.Angle(transform.up, Vector3.up);
            Vector3 axis = Vector3.Cross(transform.up, Vector3.up);

            transform.Rotate(axis, angleDiff * Time.fixedDeltaTime * _balanceTorque * stabMult, Space.World);
        }
    }

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
}
