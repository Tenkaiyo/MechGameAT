using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent NavAgent;
    public Transform[] patrolPoints;
    private int curPoint = 0;
    private Transform player;
    public ShootScript ShootScr;
    private int interval = 5;


    void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if(patrolPoints.Length > 0)
        {
            NavAgent.stoppingDistance = 0f;
            NavAgent.SetDestination(patrolPoints[curPoint].position);
        }

    }

    void Update()
    {
        if (Time.frameCount % interval == 0)
        {
            Attack();

            if(patrolPoints.Length > 0)
            {
                PointDistanceCheck();
                return;
            }

            NavAgent.SetDestination(player.position);
        }
    }


    void Attack()
    {
        if(ShootScr != null)
        {
            ShootScr.ShootEnemy(player.position);
        }
    }


    void PointDistanceCheck()
    {
        if (!NavAgent.pathPending && NavAgent.remainingDistance <= NavAgent.stoppingDistance)
        {
            if (!NavAgent.hasPath || NavAgent.velocity.sqrMagnitude == 0f)
            {
                SetNextPatrolPoint();
            }
        }
    }

    void SetNextPatrolPoint()
    {
        curPoint = (curPoint + 1) % patrolPoints.Length;
        NavAgent.SetDestination(patrolPoints[curPoint].position);
    }
}
