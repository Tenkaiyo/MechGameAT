using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MechTest : MonoBehaviour
{
    [Header("Movement")]
    #region
    public Rigidbody rb;
    public float Speed, DashSpeed, maxSpeed;
    bool dashing = false;


    public Transform GroundRay;
    public LayerMask layerMaskGround;
    public Vector3 groundNormal;
    bool isGrounded;
    #endregion

    [Header("Fuel")]
    #region 
    public Slider FuelSlider;
    float Currfuel; 
    public float MaxFuel;
    #endregion

    [Header("Dodge")]
    #region 
    public float DodgePower = 24f;
    public float DodgeTime = .2f;
    public float DodgeCooldown = 1f;
    public float DodgeCost = 0.8f;
    public float DodgeKeyPress = .1f, DodgeKeyTime;
    bool candodge = true;
    #endregion

    public GameObject MechModel;
    public GameObject Cam;
    Quaternion CameraplanarRot;
    Vector3 movement;
    Vector2 Axis;
    public ShootScript Shootscr;
    public Animator Anim;

    [Header("Player Actions")]
    #region
    public InputAction PlayerMovement;
    public InputAction DashButton;
    public InputAction DodgeButton;
    public InputAction ShootButton;
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
        Movement();
        Shooting();
    }


    private void LateUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(GroundRay.transform.position, Vector3.down, out hit, 0.75f, layerMaskGround))
        {
            groundNormal = hit.normal;
            isGrounded = true;
        }else{
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        if(!isGrounded)
        {
            rb.AddForce(Vector3.down* 8, ForceMode.Acceleration);
        }

        if(DashButton.ReadValue<float>() == 1f && Currfuel > 0f)
        {
            DodgeKeyTime += Time.deltaTime;
            if(movement != Vector3.zero)
            {
                dashing = true;
                Currfuel -= Time.deltaTime;
                UpdateFuelUI();
                Vector3 groundedBoostVelocity = Vector3.ProjectOnPlane(movement,groundNormal.normalized);
                rb.AddForce(groundedBoostVelocity * DashSpeed, ForceMode.Acceleration);
            }else{
                dashing = false;
            }

            if (rb.linearVelocity.magnitude > maxSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }else{
            dashing = false;
        }
        if(DashButton.ReadValue<float>() == 0f && DodgeKeyTime > 0 ){
            if(DodgeKeyTime < DodgeKeyPress)
            {
                StartCoroutine(Dodge());
            }

            DodgeKeyTime = 0f;
        }
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
        if(rb.linearVelocity.magnitude > Speed){
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, rb.linearVelocity.normalized, Time.deltaTime);
        }
        if(rb.linearVelocity.magnitude <= Speed){
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
        if(AimButton.ReadValue<float>() == 1f || ShootButton.ReadValue<float>() == 1f)
        {
            MechModel.transform.rotation = Quaternion.Slerp(MechModel.transform.rotation, CameraplanarRot, 90f * Time.deltaTime);
        }
    }

    void Shooting()
    {
        if(ShootButton.ReadValue<float>() == 1f){
            Shootscr.Shoot();
        }
    }

    public IEnumerator Dodge()
    {
        candodge = false;
        Currfuel -= DodgeCost;
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
        yield return new WaitForSeconds(DodgeCooldown);
        candodge = true;
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
