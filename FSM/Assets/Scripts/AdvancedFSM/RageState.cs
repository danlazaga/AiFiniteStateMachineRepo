using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RageState : FSMState
{
	NPCTankController _npcController;
	MeshRenderer _meshRenderer;

	public RageState(NPCTankController npcController, MeshRenderer meshRenderer, Transform[] wp)
	{
		waypoints = wp;
		stateID = FSMStateID.Rage;
		_npcController = npcController;
		curRotSpeed = 1.0f;

		_meshRenderer = meshRenderer;
	}

	public override void Reason(Transform player, Transform npc)
	{
		
		
	}

	public override void Act(Transform player, Transform npc)
	{
		
	}

}