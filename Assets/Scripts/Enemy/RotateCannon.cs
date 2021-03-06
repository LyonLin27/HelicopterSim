using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCannon : EnemyBase
{
    [Header("Ref")]
    [SerializeField] private Transform _canonBase;
    [SerializeField] private Transform _canonBarrel;
    [SerializeField] private Transform _shootTrans;
    [SerializeField] private ParticleSystem _explodsion;
    [SerializeField] private ParticleSystem _smoke;
    [SerializeField] private GameObject _bulletPrefab;

    [Header("Param")]
    [SerializeField] private float _maxHP = 100f;

    [SerializeField] private float _turningAglSpd = 60f;
    [SerializeField] private float _attackDist = 100f;
    [SerializeField] private float _attackAglDot = 0.0f;
    [SerializeField] private float _aimedAglDot = 0.9f;
    [SerializeField] private float _fireItv = 1f;
    [SerializeField] [Tooltip("estimate hit range")]private float _accuracy = 5f;
    [SerializeField] [Tooltip("makes canon continue fire")]private float _fireAccDebuff = 3f;
    [SerializeField] [Tooltip("further target, less accurate")]private float _distAccDebuff = 1f;

    [Header("Operation Mode")]
    public CannonMode _mode = CannonMode.AutoAttack;
    public enum CannonMode {
        AutoAttack,
        Volley,
        Idle
    }

    [Header("Status")]
    [SerializeField] private float _hp;
    [SerializeField] private bool _isDead;


    //private
    private Transform _targetTrans;
    private Vector3 _volleyTargetDir;
    private bool _targetInRange;
    private Vector3 _rootForward;

    private float _lastFireTime;

	private void Start()
	{
        _targetTrans = GameManager.instance.player.transform;
        _rootForward = transform.forward;
        _hp = _maxHP;
	}

	private void Update()
	{
        if (_isDead)
            return;

        _rootForward = transform.forward;

        if (_mode == CannonMode.Idle)
        {
            TurnToDefault();
            return;
        }

        switch (_mode)
        {
            case CannonMode.Idle:
                TurnToDefault();
                break;
            case CannonMode.AutoAttack:
                if (TargetInRange())
                {
                    TurnToTarget();

                    if (TargetAimed() && Time.time - _lastFireTime > _fireItv)
                    {
                        Fire();
                    }
                }
                break;
            case CannonMode.Volley:
                if (TargetInRange())
                {
                    TurnToDir(_volleyTargetDir);
                }
                break;
        }

	}

    #region public

    public void TriggerFire()
    {
        if (_isDead)
            return;
        Fire();
    }

    public void SetMode(CannonMode mode)
    {
        _mode = mode;
    }

    public void SetTargetDir(Vector3 targetDir)
    {
        _volleyTargetDir = targetDir;
    }

    public bool IsReadyToVolley()
    {
        return TargetDirAimed();
    }

    public bool IsDead()
    {
        return _isDead;
    }

	#endregion public

	private bool TargetInRange()
    {
        Vector3 posOffset = _targetTrans.position - transform.position;
        bool inDist = posOffset.magnitude < _attackDist;
        bool inAngle = Vector3.Dot(_rootForward, posOffset.normalized) > _attackAglDot;

        return inDist && inAngle;
    }

    private bool TargetAimed()
    {
        Vector3 posOffset = _targetTrans.position - transform.position;
        Vector3 hitPos = transform.position + _canonBarrel.forward * posOffset.magnitude;

        return Vector3.Distance(_targetTrans.position, hitPos) < _accuracy * AccuracyDebuff();
    }

    private bool TargetDirAimed()
    {
        return Vector3.Angle(_volleyTargetDir, _canonBarrel.forward) < 10f;
    }

    private float AccuracyDebuff()
    {
        float value = 1f;

        // when just fired, add debuff
        if (Time.time - _lastFireTime < 2f)
        {
            value += _fireAccDebuff;
        }

        // When player is far away, add debuff
        Vector3 posOffset = _targetTrans.position - transform.position;
        value += posOffset.magnitude / _attackDist * _distAccDebuff;

        return value;
    }

    private void TurnToTarget()
    {
        TurnToPos(_targetTrans.position);
    }

    private void TurnToDir(Vector3 targetDir)
    {
        TurnToPos(transform.position + targetDir * 10f);
    }

    private void TurnToPos(Vector3 targetPos)
    {
        // turn base
        Vector3 baseTarget = targetPos;
        baseTarget.y = _canonBase.position.y;
        _canonBase.forward = Vector3.RotateTowards(_canonBase.forward, baseTarget - _canonBase.position, Mathf.Deg2Rad * _turningAglSpd * Time.deltaTime, 0f);

        // turn barrel
        Vector3 posOffset = targetPos - transform.position;
        Vector3 aimPos = _canonBarrel.position + _canonBarrel.forward * posOffset.magnitude;
        Debug.DrawLine(_canonBarrel.position, aimPos);
        float yOffset = targetPos.y - aimPos.y;

        if (Mathf.Abs(yOffset) > 0.1f)
        {
            // turn up
            if (yOffset > 0)
            {
                _canonBarrel.Rotate(-1f * _turningAglSpd * Time.deltaTime, 0f, 0f);
            }
            else 
            {
                _canonBarrel.Rotate(_turningAglSpd * Time.deltaTime, 0f, 0f);
            }
        }
    
    }

    private void TurnToDefault()
    {
        _canonBase.forward = Vector3.RotateTowards(_canonBase.forward, _rootForward, Mathf.Deg2Rad * _turningAglSpd * Time.deltaTime, 0f);

        Vector3 aimPos = _canonBarrel.position + _canonBarrel.forward * 10f;
        float yOffset = _canonBarrel.position.y - aimPos.y;
        if (Mathf.Abs(yOffset) > 0.1f)
        {
            // turn up
            if (yOffset > 0)
            {
                _canonBarrel.Rotate(-1f * _turningAglSpd * Time.deltaTime, 0f, 0f);
            }
            else
            {
                _canonBarrel.Rotate(_turningAglSpd * Time.deltaTime, 0f, 0f);
            }
        }
    }

    private void Fire()
    {
        GameObject beam = Instantiate(_bulletPrefab, _shootTrans.transform.position, Quaternion.identity);
        beam.transform.up = _canonBarrel.forward;

        _lastFireTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isDead)
            return;
        if (collision.collider.GetComponent<PlayerProjBase>())
        {
            TakeDamage(collision.collider.GetComponent<PlayerProjBase>().GetProjInfo());
        }
    }

    public override void TakeDamage(PlayerProjInfo projInfo)
	{
        _hp -= projInfo.damage;

        if (_hp <= 0)
        {
            DestroySequence();
        }
	}

    private void DestroySequence()
    {
        _isDead = true;
        _explodsion.Play();
        _smoke.Play();
        _canonBase.gameObject.SetActive(false);
    }
}
