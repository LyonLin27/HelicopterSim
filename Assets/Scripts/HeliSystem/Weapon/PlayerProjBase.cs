using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Try to keep parameters in weapons class
 * On shoot, init parameters are init by weapon
 * 
 * 
 * On hit, player projectiles play vfxs and dispawns
 * Damage calculation is handled by hit object, using projInfo
 * 
 */
public class PlayerProjBase : MonoBehaviour
{
    protected float _startTime;

    protected PlayerProjInfo _projInfo;

    // overload this method for more complex init
    // but it should setup _projInfo
    protected void InitProjInfo(int dmg)
    {
        _projInfo = new PlayerProjInfo();
        _projInfo.damage = dmg;
        _startTime = Time.time;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        DespawnSequence();
    }

    public virtual void DespawnSequence()
    { }

    public PlayerProjInfo GetProjInfo()
    {
        _projInfo.pos = transform.position;
        return _projInfo;
    }
}

// class to pass damage info to Enemy
// for a new parameter, simply add to the class instead of deriving another one
public class PlayerProjInfo
{
    public int damage;
    public Vector3 pos;
}
