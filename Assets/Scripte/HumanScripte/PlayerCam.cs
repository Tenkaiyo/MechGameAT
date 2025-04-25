using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{

    public CamTypes Types;
    public Transform Player;
    public Vector3 Offset;


    [Header("ThirdPersonStuff")]
    #region
    [System.NonSerialized] public float mouseX, mouseY;
    [System.NonSerialized] public float smallestDistance;
    public float CameraRotationSpeed = 1;
    public LayerMask collisionMask;
    Ray camray;
    RaycastHit camrayhit;
    float collisionCusion = 0.15f, adjustdistance, adjustspeed = 5f; 
    #endregion
    public enum CamTypes{
        ThirdPerson,
        LookAtPlayer,
        FollowPlayer,
        Static
    }


    [Header("Cam rotation")]
    #region
    public InputAction MouseMove;
    #endregion

    void OnEnable()
    {
        MouseMove.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        smallestDistance = Offset.z;
    }

    void OnDisable()
    {
        MouseMove.Disable();
    }


    void LateUpdate()
    {
        if(Types == CamTypes.ThirdPerson)
        {
            ThirdPersonCam();
        }
        if(Types == CamTypes.LookAtPlayer)
        {
             LookAtPlayerCam();
        }
        if(Types == CamTypes.FollowPlayer)
        {
            FollowPlayerCam();
        }
        if(Types == CamTypes.Static)
        {

        }
    }

    public void SetParent(){
        transform.parent = Player;
    }
    public void UnParent(){
        transform.parent = null;
    }

    void ThirdPersonCam()
    {
        Vector2 Axis = MouseMove.ReadValue<Vector2>();

        mouseX += Axis.x * CameraRotationSpeed * Time.timeScale;
        mouseY -= Axis.y * CameraRotationSpeed * Time.timeScale;
        
        mouseY = Mathf.Clamp(mouseY, -40, 60);

        Player.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        //CameraCollision
        camray.origin = Player.position;
        camray.direction = Player.forward * -1f;

        if (Physics.Raycast(camray, out camrayhit, Offset.z + collisionCusion, collisionMask))
        {
            adjustdistance = Vector3.Distance(camray.origin, camrayhit.point) - collisionCusion;
        }
        else
        {
            adjustdistance = Offset.z;
        }
        if(adjustdistance < smallestDistance)
        {
            smallestDistance = adjustdistance;
        }
        if(adjustdistance > smallestDistance)
        {
            smallestDistance += Time.deltaTime* adjustspeed;
        }

        transform.localPosition = new Vector3(0, 0, -smallestDistance);
    }

    void LookAtPlayerCam()
    {
        transform.LookAt(Player.position + Offset);
    }

    void FollowPlayerCam()
    {
        transform.position = Player.position + Offset;
    }
}
