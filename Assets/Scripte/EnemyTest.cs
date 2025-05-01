using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent NavAgent;
    public float radius, angle, distance;
    public Transform[] patrolPoints;
    private int curPoint = 0;
    public LayerMask TargetMask, ObstructionMask;
    public Transform RaycastTrans;
    private Transform player;
    public ShootScript ShootScr;
    private int interval = 10;


    void Awake()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        //player = GameObject.FindGameObjectWithTag("Player").transform;

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
            CheckForTarget();
        }
    }

    void CheckForTarget()
    {
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, TargetMask);
        if(rangeCheck.Length != 0)
        {
            player = rangeCheck[0].transform;
            Vector3 directionToTarget = ((player.position + new Vector3(0,1f,0)) - RaycastTrans.position).normalized;
            float distanceToTarget = Vector3.Distance(RaycastTrans.position, player.position + new Vector3(0,1f,0));

            /*if(Vector3.Angle(transform.position, directionToTarget) < angle /2)
            {

            }*/

            if(!Physics.Raycast(RaycastTrans.position, directionToTarget, distanceToTarget, ObstructionMask))
            {
                Attack();
            }
            else
            {
                NavAgent.isStopped = false;
                NavAgent.SetDestination(player.position);
                return;
            }
            if(distanceToTarget > distance)
            {
                NavAgent.isStopped = false;
                NavAgent.SetDestination(player.position);
                return;
            }
            else
            {
                NavAgent.isStopped = true;
                return;
            }
        }

        if(patrolPoints.Length > 0)
        {
            PointDistanceCheck();
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

    public void GotHit()
    {
        Debug.Log("I got hit from somewhere?");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        NavAgent.SetDestination(player.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);

    }
}
