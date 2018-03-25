using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
	protected Transform playerTransform;
	//The next destination of our NPC tank
	protected Vector3 destPos;
	protected GameObject[] pointList;
	[HideInInspector] public float shootRate;
	protected float elapsedTime;

	public Transform turret { get; set; }
	public Transform bulletSpawnPoint { get; set; }

	protected virtual void Initialize() { }
	protected virtual void FSMUpdate() { }

	void Start()
	{
		Initialize();
	}

	void Update()
	{
		FSMUpdate();
	}

}
