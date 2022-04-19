using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathPlacer : MonoBehaviour
{
    public Transform pathParent;
	public GameObject checkPointPrefab;
	private float lastPlacedTime;

	private void Start()
	{
		GameManager.instance.playerInput.actions.HeliController.MenuAction.performed += PlaceCheckPoint;
	}

	private void PlaceCheckPoint(InputAction.CallbackContext ctx)
	{
		if (ctx.ReadValue<Vector2>().x > 0.5f && Time.time - lastPlacedTime > 1f)
		{
			var checkPoint = Instantiate(checkPointPrefab, pathParent);
			checkPoint.transform.position = transform.position;
			lastPlacedTime = Time.time;
		}
	}
}
