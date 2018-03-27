using System.Collections;
using UnityEngine;

public class AttackState : FSMState
{
    NPCTankController _npcController;

    public AttackState(NPCTankController npcController, Transform[] wp)
    {
        waypoints = wp;
        stateID = FSMStateID.Attacking;
        _npcController = npcController;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPoint();
    }

    public override void Reason(Transform player, Transform npc)
    {
        //TO IMPLEMENT
        //Check the distance with the player tank
        //When the distance is near, call the appropriate transition using SetTransition(Transition t) from NPCTankController 
        if (CheckPlayerDistance(player, npc) >= 300f)
        {
            _npcController.SetTransition(Transition.SawPlayer);
        }
        //Also check when the player becomes too far, call the appropriate transition using SetTransition(Transition t) from NPCTankController 
        if (CheckPlayerDistance(player, npc) >= 600f)
        {
            _npcController.SetTransition(Transition.LostPlayer);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //TO IMPLEMENT
        //Set the target position as the player position
        //Rotate to the target point
        SetRotation(player.position, npc);

        //Rotate turret
        TurretLookAt(player.position, _npcController.turret);

        //Shoot bullet towards the player
        _npcController.ShootBullet();
    }
}