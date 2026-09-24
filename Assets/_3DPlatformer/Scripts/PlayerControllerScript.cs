using System.Collections;
using UnityEngine;


public class PlayerControllerScript : MonoBehaviour
{
    Platformer_Inputs _inputs;
    [SerializeField] CharacterController cc;
    [SerializeField] Animator _anim;
    [SerializeField] private Transform camPosition;

    [Header("Movement Variables")]
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    public float moveSpeed = 4;
    public float runSpeed = 8;
    public float jumpForce = 4;
    public float turnSpeed = 0.1f;
    private float turnSmoothVelocity;

    [Header("Player Physics Varibles")]
    public float gravityForce = -8;
    [SerializeField] Vector3 playerVelocity;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] Collider[] groundCollider;

    [Header("Animation Blending")]
    public float moveBlend;

    [Header("Boolean Variables")]
    public bool isGrounded;

    private void Awake()
    {
        _inputs = new Platformer_Inputs();
        cc = GetComponent<CharacterController>();
        _anim = GetComponentInChildren<Animator>();
        camPosition = Camera.main.transform;
    }

    private void OnEnable()
    {
        _inputs.Enable();
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    private void Update()
    {
        HandlePhysics();
        HandleInput();
        HandleMovement();
    }

    void HandlePhysics()
    {
        groundCollider = Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer);
        if (groundCollider.Length > 0)
            isGrounded = true;
        else
            isGrounded = false;

        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -0.5f;
        else
            playerVelocity.y += gravityForce * Time.deltaTime;
    }

    void HandleInput()
    {
        moveInput = _inputs.Player.Move.ReadValue<Vector2>();

        // Player Jump
        if(_inputs.Player.Jump.triggered && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -3f * gravityForce);
        }
    }

    void HandleMovement()
    {
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        _anim.SetFloat("Speed", moveDirection.magnitude, moveBlend, Time.deltaTime);
        if(moveDirection.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x,
                moveDirection.z) * Mathf.Rad2Deg + camPosition.eulerAngles.y;
            float _angle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                targetAngle, ref turnSmoothVelocity, turnSpeed);
            transform.rotation = Quaternion.Euler(0, _angle, 0);
            Vector3 newDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            cc.Move(newDirection * moveSpeed * Time.deltaTime);
        }

        cc.Move(playerVelocity * Time.deltaTime);
    }
}
