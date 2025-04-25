using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Chara Controller")]
    #region
    public CharacterController CharaCon;
    public GameObject PlayerModel;
    public GameObject Cam;
    public float NormalMovementSpeed = 4, SprintSpeed = 5;
    private float PlayerSpeed;

    private Vector3 moveDir, playerVelocity;
    private bool groundedPlayer;
    private float gravityValue = -9.81f;
    Quaternion CameraplanarRot;
    public Vector3 OldCameraplanarRot;
    public Vector2 Axis;
    public Vector2 OldAxis;
    public bool oldCamPerspective;
    #endregion

    public Animator Anim;

    [Header("Player Actions")]
    #region
    public InputAction PlayerMovement;
    public InputAction SprintMove;
    void OnEnable()
    {
        PlayerMovement.Enable();
        SprintMove.Enable();
    }
    void OnDisable()
    {
        PlayerMovement.Disable();
        SprintMove.Disable();
    }
    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        groundedPlayer = CharaCon.isGrounded;

        if (groundedPlayer)
        {
            playerVelocity.y = -4f;
        }
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(Cam.transform.rotation * Vector3.forward, transform.up).normalized;

        if(oldCamPerspective)
        CameraplanarRot = Quaternion.LookRotation(OldCameraplanarRot, transform.up);
        if(!oldCamPerspective)
        CameraplanarRot = Quaternion.LookRotation(cameraPlanarDirection, transform.up);


        Axis = PlayerMovement.ReadValue<Vector2>();

        #region Sprinting
        if(SprintMove.ReadValue<float>() == 1f)
        {
            PlayerSpeed = SprintSpeed;
            Anim.SetBool("Sprinting", true);
        }
        else
        {
            PlayerSpeed = NormalMovementSpeed;
            Anim.SetBool("Sprinting", false);
        }
        #endregion

        #region Movement Calc
        Vector3 movement = Vector3.ClampMagnitude(new Vector3(Axis.x, 0, Axis.y), 1f);
        if(oldCamPerspective && (Vector2.Angle(Axis, OldAxis) > 30f || oldCamPerspective && movement.magnitude < .6f))
        {
            Debug.Log(Vector2.Angle(Axis, OldAxis));
            oldCamPerspective = false;
        }
        Vector3 _movement = CameraplanarRot * movement;

        moveDir.x = _movement.x * PlayerSpeed;
        moveDir.z = _movement.z * PlayerSpeed;
        #endregion

        #region Rotate Model
        if (_movement != Vector3.zero)
        {
            Anim.SetBool("Walking", true);
            PlayerModel.transform.rotation = Quaternion.Slerp(PlayerModel.transform.rotation, Quaternion.LookRotation(new Vector3(_movement.x, 0f, _movement.z)), 15f * Time.deltaTime);
        }else{
            Anim.SetBool("Walking", false);
        }
        #endregion

        playerVelocity.y += gravityValue * Time.deltaTime;
        CharaCon.Move((moveDir + playerVelocity) * Time.deltaTime);
    }

}
