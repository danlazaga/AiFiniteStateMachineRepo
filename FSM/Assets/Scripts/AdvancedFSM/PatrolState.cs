using System.Collections;
using UnityEngine;

public class PatrolState : FSMState
{
    NPCTankController _npcTankController;

    public PatrolState(NPCTankController npcTankController, Transform[] wp)
    {
        waypoints = wp;
        stateID = FSMStateID.Patrolling;
        _npcTankController = npcTankController;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //TO IMPLEMENT
        //Check the distance with player tank
        var distancePlayer = Vector3.Distance(npc.position, player.position);

        //When the distance is near, call the appropriate transition using SetTransition(Transition t) from NPCTankController 
        if (distancePlayer <= 500)
        {
            _npcTankController.SetTransition(Transition.SawPlayer);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //TO IMPLEMENT
        //Find another random patrol point if the current destination point is reached
        if (IsInCurrentRange(npc, destPos)== true)
        {
            FindNextPoint();
        }

        AvoidObstacles(destPos, npc);

        //Rotate the tank and the turret
        SetRotation(AvoidObstacles(destPos,npc), npc);
        TurretLookAt(destPos, _npcTankController.turret);

        //Go Forward
        SetDestination(npc);

    }



}