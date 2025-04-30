using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTest : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent NavAgent;
    public float radius;
    public LayerMask TargetMask;
    public Transform[] patrolPoints;
    private int curPoint = 0;
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
            Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, TargetMask);
            if(rangeCheck.Length != 0)
            {
                player = rangeCheck[0].transform;
                Attack();
                NavAgent.SetDestination(player.position);
                return;
            }

            if(patrolPoints.Length > 0)
            {
                PointDistanceCheck();
            }

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

   /* private void OnSceneGUI()
    {
        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
    }*/
}
