using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFSM : FSM
{
    public enum FSMState { NONE, PATROL, CHASE, ATTACK, DEAD, FlEE }
    public FSMState currentState;
    public GameObject bullet;
    float currentSpeed;
    float currentRotationSpeed;
    float turretRotSpeed;
    bool isDead;
    [SerializeField] int health;
    float distancePlayer;
    float distance;
    Rigidbody myRigidBody;

    protected override void Initialize()
    {
        currentState = FSMState.PATROL;

        currentSpeed = 150.0f;
        currentRotationSpeed = 2.0f;
        turretRotSpeed = 10.0f;
        isDead = false;
        shootRate = 3.0f;

        pointList = GameObject.FindGameObjectsWithTag("WandarPoint");
        myRigidBody = GetComponent<Rigidbody>();

        FindNextPoint();

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.transform.GetChild(0).transform;
    }

    void FindNextPoint()
    {
        int randomIndex = Random.Range(0, pointList.Length);

        destPos = pointList[randomIndex].transform.position;
    }

    protected override void FSMUpdate()
    {
        distancePlayer = Vector3.Distance(transform.position, playerTransform.position);
        distance = Vector3.Distance(transform.position, destPos);

        switch (currentState)
        {
            case FSMState.PATROL:
                UpdatePatrolState();
                break;

            case FSMState.CHASE:
                UpdateChaseState();
                break;

            case FSMState.ATTACK:
                UpdateAttackState();
                break;

            case FSMState.DEAD:
                UpdateDeadState();
                break;

            case FSMState.FlEE:
                UpdateFleeState();
                break;
        }

        elapsedTime += Time.deltaTime;

        if (health <= 0)
        {
            currentState = FSMState.DEAD;
        }

        if(health < 70)
        {
            UpdateRageState();
        }
    }

    void UpdatePatrolState()
    {
        TurretLookAt(destPos);
        SetDestination(destPos);

        if (distancePlayer < 500f)
        {
            currentState = FSMState.CHASE;
        }

        if (distance < 100f)
        {
            FindNextPoint();
        }

    }

    void UpdateChaseState()
    {
        SetDestination(playerTransform.position);
        TurretLookAt(playerTransform.position);

        if (distance >= 600f)
        {
            currentState = FSMState.PATROL;
        }
        else if (distance <= 300f)
        {
            currentState = FSMState.ATTACK;
        }
    }

    void UpdateFleeState()
    {
        float fleeSpeed = 100f;

        transform.rotation = Quaternion.LookRotation(transform.position - playerTransform.position);
        //Move
        transform.position += (transform.forward * fleeSpeed * Time.deltaTime);
    }

    void UpdateAttackState()
    {
        TurretLookAt(playerTransform.position);
        ShootBullet();

        if (distance >= 300f)
        {
            currentState = FSMState.CHASE;
        }
    }

    void UpdateDeadState()
    {
        if (!isDead)
        {
            isDead = true;
            Explode();
        }
    }

    void UpdateRageState()
    {
        currentSpeed = 300f;
        shootRate = 6f;
        var meshRender = GetComponent<MeshRenderer>();
        meshRender.material.color = Color.black;
    }

    void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            elapsedTime = 0;
        }
    }

    void SetDestination(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.Normalize();
        transform.position += direction * currentSpeed * Time.deltaTime;

        Quaternion newRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * currentRotationSpeed);
    }

    void TurretLookAt(Vector3 target)
    {
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
    }

    void Explode()
    {
        float randomX = Random.Range(10.0f, 30.0f);
        float randomZ = Random.Range(10.0f, 30.0f);

        myRigidBody.AddExplosionForce(10000.0f, this.transform.position - new Vector3(randomX, 10.0f, randomZ), 40.0f, 10.0f);

        myRigidBody.velocity = transform.TransformDirection(new Vector3(randomX, 20.0f, randomZ));

        Destroy(this.gameObject, 1.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= collision.gameObject.GetComponent<Bullet>().damage;
        }
    }

}