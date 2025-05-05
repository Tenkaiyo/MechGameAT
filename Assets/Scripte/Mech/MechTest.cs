using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MechTest : MonoBehaviour
{
    [Header("Movement")]
    #region
    public Rigidbody rb;
    public float Speed;
    public Transform GroundRay;
    public LayerMask layerMaskGround;
    public Vector3 groundNormal;
    bool isGrounded;
    #endregion


    [Header("Dash")]
    #region
    public float DashSpeed;
    public float maxSpeed;
    bool dashing = false;
    #endregion

    
    [Header("Hover")]
    #region
    public float HoverJump;
    #endregion


    [Header("Dodge")]
    #region 
    public float DodgePower = 24f;
    public float DodgeTime = .2f;
    public float DodgeCooldown = 1f;
    public float DodgeCost = 0.8f;
    bool candodge = true , dodging = false;
    #endregion


    [Header("Fuel")]
    #region 
    public Slider FuelSlider;
    float Currfuel; 
    public float MaxFuel;
    public float fuelresetSpeed;
    private float resetfueltimer, fueldelay = 0.6f, fueldelayMax = 1f;
    #endregion

    [Header("Other")]
    #region 
    public GameObject DeathUI;
    private bool Dead = false;
    public GameObject MechModel;
    public GameObject Cam;
    public MechCam mechCam;
    Quaternion CameraplanarRot;
    Vector3 movement;
    Vector2 Axis;
    public ShootScript Shootscr;
    public Animator Anim;
    #endregion

    [Header("Player Actions")]
    #region
    public InputAction PlayerMovement;
    public InputAction DashButton;
    public InputAction DodgeButton;
    public InputAction ShootButton;
    private bool shooting;
    public InputAction AimButton;
    #endregion


    void OnEnable()
    {
        PlayerMovement.Enable();
        DashButton.Enable();
        DodgeButton.Enable();
        ShootButton.Enable();
        AimButton.Enable();
    }

    void OnDisable()
    {
        PlayerMovement.Disable();
        DashButton.Disable();
        DodgeButton.Disable();
        ShootButton.Disable();
        AimButton.Disable();
    }

    void Start()
    {
        SupplyFuel();
    }

    void Update()
    {
        if(Dead)
        {
            return;
        }
        Movement();
        FuelRefill();
        Shooting();
    }


    private void LateUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(GroundRay.transform.position, Vector3.down, out hit, 0.25f, layerMaskGround))
        {
            groundNormal = hit.normal;
            isGrounded = true;
        }else{
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        if(!isGrounded && !dashing)
        {
            rb.AddForce(Vector3.down* 8, ForceMode.Acceleration);
        }

        if(Dead)
        {
            return;
        }


        #region Hover
        //Hover
        /*if(DashButton.ReadValue<float>() == 1f && Currfuel > 0f)
        {
            if(!dashing && isGrounded)
            {
                Debug.Log("Hover");
                rb.AddForce(Vector3.up * HoverJump, ForceMode.Impulse);
                dashing = true;
                rb.useGravity = false;
            }

            if(dashing)
            {
                Debug.Log("Hovering");
                Currfuel -= Time.deltaTime;
                UpdateFuelUI();

                if(movement != Vector3.zero)
                {
                    Vector3 groundedBoostVelocity = Vector3.ProjectOnPlane(movement,Vector3.up);
                    rb.AddForce(groundedBoostVelocity * DashSpeed, ForceMode.Acceleration);
                }
                if (rb.linearVelocity.magnitude > maxSpeed)
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).normalized * maxSpeed;
            }
        }

        if(dashing && (DashButton.ReadValue<float>() == 0f || Currfuel <= 0f))
        {
            Debug.Log("No More Hover");
            dashing = false;
            rb.useGravity = true;
        }*/
        #endregion


        #region Dash
        //Dash
        if(DashButton.ReadValue<float>() == 1f && Currfuel > 0f)
        {
            if(movement != Vector3.zero)
            {
                Currfuel -= Time.deltaTime;
                resetfueltimer = fueldelay;
                if(Currfuel <= 0)
                    resetfueltimer = fueldelayMax;
                UpdateFuelUI();
                Vector3 groundedBoostVelocity = Vector3.ProjectOnPlane(movement,groundNormal.normalized);
                rb.AddForce(groundedBoostVelocity * DashSpeed, ForceMode.Acceleration);
            }
            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
        #endregion
    }

    void Movement()
    {
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(Cam.transform.rotation * Vector3.forward, transform.up).normalized;
        CameraplanarRot = Quaternion.LookRotation(cameraPlanarDirection, transform.up);
        Axis = PlayerMovement.ReadValue<Vector2>();
        movement = CameraplanarRot * Vector3.ClampMagnitude(new Vector3(Axis.x, 0, Axis.y), 1f);
        RotateModel();

        //Dodge
        if(DodgeButton.ReadValue<float>() == 1f && candodge && Currfuel > 0f)
        {
            StartCoroutine(Dodge());
            return;
        }


        //Dash
        if(dashing)
        {
            return;
        }

        //Normal Walk
        if(rb.linearVelocity.magnitude > Speed)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity.normalized, Time.deltaTime);
        }
        if(rb.linearVelocity.magnitude <= Speed)
        {
            Vector3 groundedBoostVelocity = Vector3.ProjectOnPlane(movement,groundNormal);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, groundedBoostVelocity * Speed, Time.deltaTime * Speed);
            //!isgrounded -> Gravitation reinballern
        }
    }

    void RotateModel()
    {
        if (movement != Vector3.zero && (AimButton.ReadValue<float>() < 1f || ShootButton.ReadValue<float>() < 1f))
        {
            MechModel.transform.rotation = Quaternion.Slerp(MechModel.transform.rotation, Quaternion.LookRotation(new Vector3(movement.x, 0f, movement.z)), 5f * Time.deltaTime);
        }
        if(AimButton.ReadValue<float>() == 1f || ShootButton.ReadValue<float>() == 1f || dodging)
        {
            //*if(MechModel.transform.rotation != CameraplanarRot)
            {
                //MechModel.transform.rotation = Quaternion.Slerp(MechModel.transform.rotation, CameraplanarRot, 90f * Time.deltaTime);
            }
            //else
            {
                MechModel.transform.rotation = CameraplanarRot;
            }
        }
    }

    void Shooting()
    {
        if(ShootButton.ReadValue<float>() == 1f && !dodging){
            shooting = true;
            Shootscr.Shoot();
        }
        if(ShootButton.ReadValue<float>() == 0f && shooting)
        {
            shooting = false;
            Shootscr.StopShooting();
        }
    }

    public IEnumerator Dodge()
    {
        candodge = false;
        dodging = true;
        mechCam.CamDelay();
        Currfuel -= DodgeCost;
        resetfueltimer = fueldelay;
        if(Currfuel <= 0)
            resetfueltimer = fueldelayMax;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        float damping = rb.linearDamping;
        rb.linearDamping = 2f;
        UpdateFuelUI();
        if(movement != Vector3.zero){
            rb.linearVelocity = movement * DodgePower;            
            Anim.SetFloat("DodgeH", Axis.x);
            Anim.SetFloat("DodgeV", Axis.y);
        }else{
            rb.linearVelocity = CameraplanarRot * new Vector3(0, 0, 1f) * DodgePower;
            Anim.SetFloat("DodgeV", 1f);
        }

        yield return new WaitForSeconds(DodgeTime);

        rb.useGravity = true;
        Anim.SetFloat("DodgeH", 0f);
        Anim.SetFloat("DodgeV", 0f);
        rb.linearDamping = damping;
        dodging = false;

        yield return new WaitForSeconds(DodgeCooldown);

        candodge = true;
    }

    public void Die()
    {
        if(DeathUI != null)
            Dead = true;
            DeathUI.SetActive(true);
    }

    void FuelRefill()
    {
        if(resetfueltimer > 0)
        {
            resetfueltimer -= Time.deltaTime;
        }

        if(resetfueltimer <= 0 && Currfuel < MaxFuel)
        {
            Currfuel += Time.deltaTime * fuelresetSpeed;
            UpdateFuelUI();
        }
    }
    public void SupplyFuel()
    {
        FuelSlider.maxValue = MaxFuel;
        Currfuel = MaxFuel;
        UpdateFuelUI();
    }

    public void UpdateFuelUI()
    {
        FuelSlider.value = Currfuel;
    }
}
