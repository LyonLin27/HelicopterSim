using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLine : MonoBehaviour
{
    private ParticleSystem _ps;
    private PlayerController _player;

    public float _startThreshold = 20f;
    public float _maxEmissionRate = 20f;

    void Start()
    {
        _player = GameManager.instance.player;
        _ps = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = _player.GetVelocity();

        var psMain = _ps.main;
        psMain.startSpeed = new ParticleSystem.MinMaxCurve(-1f * velocity.magnitude);

        var psEmission = _ps.emission;
        if (velocity.magnitude < _startThreshold)
        {
            psEmission.rateOverTime = 0;
        }
        else
        {
            psEmission.rateOverTime = _maxEmissionRate * (velocity.magnitude / _startThreshold);
            transform.forward = velocity.normalized;
        }
    }
}
