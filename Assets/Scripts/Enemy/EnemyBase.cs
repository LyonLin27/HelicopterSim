using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

	// Damage is handled by enemy instead of Bullet
	// When enemy detect collision, take projectile info to react and calc damage
	public virtual void TakeDamage(PlayerProjInfo projInfo)
    {}
}
