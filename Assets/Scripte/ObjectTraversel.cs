using UnityEngine;

public class ObjectTraversel : MonoBehaviour
{

    public PatrolArt patrolart;
    public Transform[] Travelpoints;
    private int curPoint = 0;
    private bool backwards = false;
    public float TravSpeed = 3f;

    private int interval = 10;

    public enum PatrolArt{
        PingPong,
        Loop,
        Teleport,
    }


    void Start()
    {

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
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, Travelpoints[curPoint].position, TravSpeed * Time.deltaTime);
    }

}
