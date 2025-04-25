using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MechCam : MonoBehaviour
{


    [Header("ThirdPersonStuff")]
    #region
    public Transform CamPar;
    public MechTest MechMove;
    [System.NonSerialized] public float mouseX, mouseY;
    [System.NonSerialized] public float smallestDistance;
    public float CamDistance;
    public float CameraRotationSpeed = 1;
    public LayerMask collisionMask;
    Ray camray;
    RaycastHit camrayhit;
    float collisionCusion = 0.15f, adjustdistance, adjustspeed = 5f; 
    Vector3 oldPos;
    bool delay;
    float delaytimer, delaytime = .5f;
    #endregion

    [Header("Cam rotation")]
    #region
    public InputAction MouseMove;
    #endregion


    void OnEnable()
    {
        MouseMove.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void OnDisable()
    {
        MouseMove.Disable();
    }


    void LateUpdate()
    {
        CameraLook();
    }


    void CameraLook()
    {
        Vector2 Axis = MouseMove.ReadValue<Vector2>();
        mouseX += Axis.x * CameraRotationSpeed * Time.timeScale;
        mouseY -= Axis.y * CameraRotationSpeed * Time.timeScale;
        mouseY = Mathf.Clamp(mouseY, -40, 60);
        CamPar.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        #region CamCollison
        camray.origin = CamPar.position;
        camray.direction = CamPar.forward * -1f;

        if (Physics.Raycast(camray, out camrayhit, CamDistance + collisionCusion, collisionMask))
        {
            adjustdistance = Vector3.Distance(camray.origin, camrayhit.point) - collisionCusion;
        }
        else
        {
            adjustdistance = CamDistance;
        }
        #endregion

        if(adjustdistance < smallestDistance)
        {
            smallestDistance = adjustdistance;
        }
        if(adjustdistance > smallestDistance)
        {
            smallestDistance += adjustspeed * Time.deltaTime;
        }

        if(!delay)
        {
            transform.localPosition = new Vector3(0, 0, -smallestDistance);
        }
        if(delay)
        {
            delaytimer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, CamPar.position + transform.rotation * new Vector3(0,0, -smallestDistance), delaytimer * 2f);
            if(delaytimer >= delaytime){
                delay = false;
                transform.localPosition = new Vector3(0, 0, -smallestDistance);
            }
        }
    }

    public void CamDelay()
    {
        oldPos = transform.position;
        delay = true;
        delaytimer = 0f;
    }
}
