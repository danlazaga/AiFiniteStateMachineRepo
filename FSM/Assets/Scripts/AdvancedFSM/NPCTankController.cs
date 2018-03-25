using System.Collections;
using UnityEngine;

public class NPCTankController : AdvancedFSM
{
    MeshRenderer meshRenderer;
    public GameObject Bullet;
    private int health;

    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        health = 100;

        elapsedTime = 0.0f;
        shootRate = 2.0f;

        meshRenderer = GetComponent<MeshRenderer>();

        //Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

        //Get the turret of the tank
        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;

        //Start Doing the Finite State Machine
        ConstructFSM();
    }

    //Update each frame
    protected override void FSMUpdate()
    {
        //Check for health
        elapsedTime += Time.deltaTime;

        //Make the AI reason and act based on its current state
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);

    }

    public void SetTransition(Transition t)
    {
        PerformTransition(t);
    }

    private void ConstructFSM()
    {
        //Get the list of points
        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");

        Transform[] waypoints = new Transform[pointList.Length];
        int i = 0;
        foreach (GameObject obj in pointList)
        {
            waypoints[i] = obj.transform;
            i++;
        }

        //Add Transition and State Pairs
        PatrolState patrol = new PatrolState(this, waypoints);
        //If the tank sees the player while patrolling, move to chasing state
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        //If the tank loses health while patrolling, move to dead state
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        // patrol.AddTransition(Transition.Buff, FSMStateID.Rage);

        ChaseState chase = new ChaseState(this, waypoints);
        //If the tank loses the player while chasing, move to patrol state
        chase.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        //If the tank reaches the player while attacking, move to attack state
        chase.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        //If the tank loses health while patrolling, move to dead state
        chase.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        // chase.AddTransition(Transition.Buff, FSMStateID.Rage);

        AttackState attack = new AttackState(this, waypoints);
        //If the tank loses the player while attacking, move to patrol state
        attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        //If the player is within sight while attacking, move to chase state
        attack.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        //If the tank loses health while attacking, move to dead state
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        // attack.AddTransition(Transition.Buff, FSMStateID.Rage);

        // RageState rage = new RageState(this, meshRenderer, waypoints);

        // rage.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        // //If the tank loses health while attacking, move to dead state
        // rage.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        DeadState dead = new DeadState();
        //When there is no health, go to dead state
        dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        //Add states to the state list
        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        //AddFSMState(rage);
        AddFSMState(dead);
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        //Reduce health
        if (collision.gameObject.tag == "Bullet")
        {
            health -= 50;

            if (health <= 50)
            {
                SetTransition(Transition.Buff);
            }

            else if (health <= 0)
            {
                Debug.Log("Switch to Dead State");
                SetTransition(Transition.NoHealth);
                Explode();
            }
        }
    }

    protected void Explode()
    {
        float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            GetComponent<Rigidbody>().AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        }

        Destroy(gameObject, 1.5f);
    }

    /// <summary>
    /// Shoot the bullet from the turret
    /// </summary>
    public void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            elapsedTime = 0.0f;
        }
    }
}