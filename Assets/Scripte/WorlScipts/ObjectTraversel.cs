using UnityEngine;

public class ObjectTraversel : MonoBehaviour
{

    public PatrolArt patrolart;
    public Transform[] Travelpoints;
    public Transform[] Waitingpoints;
    private int curPoint = 0;
    private bool backwards = false;
    public float TravSpeed = 3f;
    public float WaitTime = 0f;

    private float timer;
    private int interval = 10;

    public enum PatrolArt{
        PingPong,
        Loop,
        Teleport,
    }


    void Start()
    {
        transform.position = Travelpoints[0].position;
        transform.rotation = Travelpoints[0].rotation;
    }

    void Update()
    {
        if (Time.frameCount % interval == 0)
        {
            if(Vector3.Distance(transform.position, Travelpoints[curPoint].position) == 0f)
            {
                transform.rotation = Travelpoints[(backwards? Mathf.Clamp(curPoint -1, 0, Travelpoints.Length): curPoint)].rotation;
                curPoint = curPoint + (backwards? -1: 1);
                

                if(patrolart == PatrolArt.PingPong)
                {
                    if(curPoint >= Travelpoints.Length){
                        backwards = true;
                        curPoint = Travelpoints.Length - 1;
                    }
                    if(curPoint < 0)
                    {
                        backwards = false;
                        curPoint = 0;
                    }
                }

                if(patrolart == PatrolArt.Loop)
                {
                    if(curPoint >= Travelpoints.Length)
                    {
                        curPoint = 0;
                    }
                }

                if(patrolart == PatrolArt.Teleport)
                {
                    if(curPoint >= Travelpoints.Length)
                    {
                        transform.position = Travelpoints[0].position;
                        transform.rotation = Travelpoints[0].rotation;
                        curPoint = 1;
                    }
                }
                
                if(Waitingpoints.Length != 0)
                {
                    for(int i=0; i< Waitingpoints.Length; i++)
                    {
                        if(Waitingpoints[i] == Travelpoints[curPoint])
                        {
                            timer = WaitTime;
                        }
                    }
                }

            }
        }

        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                timer = 0;
            }
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, Travelpoints[curPoint].position, TravSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < Travelpoints.Length; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Travelpoints[i].position, 1f);
        }

    }

}
