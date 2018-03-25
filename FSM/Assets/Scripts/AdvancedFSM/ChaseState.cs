using System.Collections;
using UnityEngine;

public class ChaseState : FSMState
{

    NPCTankController _npcTankController;
    public ChaseState(NPCTankController npcTankController, Transform[] wp)
    {
        waypoints = wp;
        stateID = FSMStateID.Chasing;
        _npcTankController = npcTankController;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPoint();
    }

    public override void Reason(Transform player, Transform npc)
    {
        //TO IMPLEMENT
        //Set the target position as the player position
        //Check the distance with player tank
        var playerDistance = Vector3.Distance(npc.position, player.position);
        //When the distance is near, call the appropriate transition using SetTransition(Transition t) from NPCTankController 
        if (playerDistance >= 600f)
        {
            _npcTankController.SetTransition(Transition.LostPlayer);
        }

        //Also check when the player becomes too far, call the appropriate transition using SetTransition(Transition t) from NPCTankController 
        else if (playerDistance <= 300f)
        {
            _npcTankController.SetTransition(Transition.ReachPlayer);
        }

    }

    public override void Act(Transform player, Transform npc)
    {
        //TO IMPLEMENT
        AvoidObstacles(player.position, npc);
        //Rotate to the target point
        SetRotation(AvoidObstacles(player.position, npc), npc);
        //Rotate turret
        TurretLookAt(player.position, _npcTankController.turret);
        //Go Forward
        SetDestination(npc);

    }

}