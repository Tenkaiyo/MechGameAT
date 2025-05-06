using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class MechCam : MonoBehaviour
{


    [Header("ThirdPersonStuff")]
    #region
    public Transform CamParRot, CamParX;
    public Camera cam;
    [System.NonSerialized] public float mouseX, mouseY;
    [System.NonSerialized] public float smallestDistanceX, smallestDistanceR;
    public float CamDistance, CamDistanceAim, CamSideDist;
    public float CameraRotationSpeed = 1;
    public LayerMask collisionMask;
    Ray camrayx, camrayRot;
    RaycastHit camrayhit;
    float collisionCusion = 0.15f, adjustdistanceX, adjustdistanceR, adjustspeed = 5f; 
    Vector3 oldPos;
    #endregion

    #region CamDelay
    bool delay;
    float delaytimer, delaytime = 1f;
    public float maxDelay;
    bool ExtendFov;
    float normFov, currFov, maxFov = 90f, additionalFov = 5f;
    #endregion

    [Header("Cam rotation")]
    #region
    public InputAction MouseMove;
    public InputAction AimButton;
    #endregion


    void OnEnable()
    {
        MouseMove.Enable();
        AimButton.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }
    void OnDisable()
    {
        MouseMove.Disable();
        AimButton.Disable();
    }

    void Start()
    {
        normFov = cam.fieldOfView;
        currFov = normFov;
        maxFov = normFov + additionalFov;
    }

    public void SetMouseSens(float sens)
    {
        CameraRotationSpeed = sens;
    }

    void LateUpdate()
    {
        CameraLook();
    }


    void CameraLook()
    {
        float camdist;
        Vector2 Axis = MouseMove.ReadValue<Vector2>();
        mouseX += Axis.x * CameraRotationSpeed * Time.timeScale;
        mouseY -= Axis.y * CameraRotationSpeed * Time.timeScale;
        mouseY = Mathf.Clamp(mouseY, -40, 60);
        CamParRot.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);

        if(AimButton.ReadValue<float>() == 1f)
        {
            CamParX.localPosition = new Vector3(CamSideDist, 0, 0);
            camdist = CamDistanceAim;
        }else{
             CamParX.localPosition = Vector3.zero;
             camdist = CamDistance;
        }


        #region CamCollison
        /*
        camrayRot.origin = CamParRot.position;
        camrayRot.direction = CamParRot.right * 1f;

        if(Physics.Raycast(camrayRot, out camrayhit, CamSideDist + collisionCusion, collisionMask))
        {
            adjustdistanceR = Vector3.Distance(camrayRot.origin, camrayhit.point) - collisionCusion;
        }
        else
        {
            adjustdistanceR = CamSideDist;
        }

        if(adjustdistanceR < smallestDistanceR)
        {
            smallestDistanceR = adjustdistanceR;
        }
        if(adjustdistanceR > smallestDistanceR)
        {
            smallestDistanceR += adjustspeed * Time.deltaTime;
        }
        */



        camrayx.origin = CamParX.position;
        camrayx.direction = CamParX.forward * -1f;

        if (Physics.Raycast(camrayx, out camrayhit, camdist + collisionCusion, collisionMask))
        {
            adjustdistanceX = Vector3.Distance(camrayx.origin, camrayhit.point) - collisionCusion;
        }
        else
        {
            adjustdistanceX = camdist;
        }

        if(adjustdistanceX < smallestDistanceX)
        {
            smallestDistanceX = adjustdistanceX;
        }
        if(adjustdistanceX > smallestDistanceX)
        {
            smallestDistanceX += adjustspeed * Time.deltaTime;
        }
        #endregion



        if(ExtendFov && currFov > cam.fieldOfView)
        {
            cam.fieldOfView += Time.deltaTime * 40f;
        }
        if(cam.fieldOfView > normFov && !ExtendFov)
        {
            cam.fieldOfView -= Time.deltaTime * 10f;
        }



        if(!delay)
        {
            transform.localPosition = new Vector3(0, 0, -smallestDistanceX);
        }
        if(delay)
        {
            delaytimer += Time.deltaTime;
            Vector3 newcampos = CamParX.position + transform.rotation * new Vector3(0,0, -smallestDistanceX);
            oldPos = Vector3.ClampMagnitude(oldPos - newcampos, maxDelay) + newcampos;
            transform.position = Vector3.Lerp(oldPos, newcampos, delaytimer);
            if(delaytimer >= delaytime){
                delay = false;
                transform.localPosition = new Vector3(0, 0, -smallestDistanceX);
            }
        }
    }


    public IEnumerator FOVSpeed()
    {
        currFov = cam.fieldOfView + additionalFov;
        if(currFov > maxFov)
        {
            currFov = maxFov;
        }
        ExtendFov = true;
        yield return new WaitForSeconds(.4f); 
        ExtendFov = false;
    }

    public void CamDelay()
    {
        //StartCoroutine(FOVSpeed());
        oldPos = transform.position;
        delay = true;
        delaytimer = 0f;
    }
}
