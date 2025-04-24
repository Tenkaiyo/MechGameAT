using UnityEngine;
using UnityEngine.Animations;

public class CameraTrigger : MonoBehaviour
{
    public Transform NewCameraPos;
    public CamType Types;
    public float FOV = 60;
    public UnityEngine.Vector3 Offset;

    public enum CamType{
        ThirdPerson,
        LookAtPlayer,
        FollowPlayer,
        Static
    }


    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerMove playr = other.GetComponent<PlayerMove>();
            PlayerCam playcam = playr.Cam.GetComponent<PlayerCam>();
            if(playr.oldCamPerspective == false)
            {
                playr.OldCameraplanarRot = Vector3.ProjectOnPlane(playcam.transform.rotation * Vector3.forward, playr.transform.up).normalized;
                playr.OldAxis = playr.Axis;
                playr.oldCamPerspective = true;
            }
            playr.Cam.GetComponent<Camera>().fieldOfView = FOV;
            playcam.Offset = Offset;


            if(Types == CamType.ThirdPerson){
                playcam.SetParent();
                if(playcam.Types != PlayerCam.CamTypes.ThirdPerson){
                    playcam.transform.localRotation = new Quaternion(0,0,0,0);
                    playcam.mouseX = NewCameraPos.localRotation.eulerAngles.y;
                    Debug.Log(NewCameraPos.localRotation.eulerAngles.y);
                    playcam.mouseY = NewCameraPos.localRotation.eulerAngles.x;
                    Debug.Log(NewCameraPos.localRotation.eulerAngles.x);
                    playcam.smallestDistance = Offset.z;
                }
                playcam.Types = PlayerCam.CamTypes.ThirdPerson;
                Debug.Log("Third Person Baby");
            }
            if(Types == CamType.LookAtPlayer){
                playcam.UnParent();
                playcam.Types = PlayerCam.CamTypes.LookAtPlayer;
                playcam.transform.position = NewCameraPos.position;
                Debug.Log("Im Looking");
            }
            if(Types == CamType.FollowPlayer){
                playcam.UnParent();
                playcam.Types = PlayerCam.CamTypes.FollowPlayer;
                playcam.transform.rotation = NewCameraPos.rotation;
                Debug.Log("I will follow!");
            }
            if(Types == CamType.Static){
                playcam.UnParent();
                playcam.Types = PlayerCam.CamTypes.Static;
                playcam.transform.position = NewCameraPos.position;
                playcam.transform.rotation = NewCameraPos.rotation;
                Debug.Log("Static");
            }
        }
    }
}
