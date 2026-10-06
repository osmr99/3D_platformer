using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControllerScript : MonoBehaviour
{
    // The variables I used.
    Platformer_Inputs _inputs;
    CharacterController cc;
    Animator anim;
    Transform camPosition;

    [Header("Movement")]
    [SerializeField] Vector2 moveInput;
    [SerializeField] Vector3 moveDirection;
    public float moveSpeed;
    public float sprintMult;
    public bool sprint;
    public float turnSpeed;
    private float turnSmoothVelocity;

    [Header("Jumping")]
    public float jumpForce;
    public float quickJumpMult;
    public float groundedDelay;
    private bool jumped;
    public int numberOfjumps = 2;
    [SerializeField] private int _jumps;

    [Header("Physics")]
    public float gravityForce;
    [SerializeField] Vector3 playerVelocty;
    public Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] Collider[] groundCollider;
    public bool isGrounded;

    [Header("Leniency")]
    public TMP_Text coyoteDisplay;
    public TMP_Text jumpBufferDisplay;
    public float coyoteTimer;
    public float jumpBufferTime;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float jumpBuffer;

    [Header("Booleans")]
    public bool inAction;

    // Initiate some variables.
    private void Awake()
    {
        _inputs = new Platformer_Inputs();
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        camPosition = Camera.main.transform;
    }

    // Uses Unity's event system to toggle sprint.
    private void Start()
    {
        _inputs.Player.Sprint.performed += ctx => { sprint = !sprint; };
    }

    // This function and the one below make sure that inputs function only when the player controller is actually enabled.
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
        if(!inAction)
        {
            HandleInput();
            HandleMovement();
        }

        // Set animation value
        anim.SetBool("InAction", inAction);
        if (isGrounded) _jumps = numberOfjumps;

        // If the player is near the ground and hasn't recently jumped, prepare coyoteTime.
        // Else, reduce coyoteTime by Time.deltaTime.
        if (isGrounded && !jumped) coyoteTime = coyoteTimer;
        else coyoteTime -= Time.deltaTime;

        // jumpBuffer gets reduced by Time.deltaTime unless doing so would make it negative.
        jumpBuffer -= Time.deltaTime;
        if (jumpBuffer < 0) jumpBuffer = 0;

        // Debug text.
        coyoteDisplay.text = "Coyote Time = " + coyoteTime.ToString();
        jumpBufferDisplay.text = "Jump Buffer = " + jumpBuffer.ToString();
    }

    private void HandlePhysics()
    {
        // Basic isGrounded check. If the player is close enough to the ground, they are considered grounded.
        // This is a problem for jumping and coyote time because, for a moment after jumping, the player is still considered grounded.
        // How I handle this is explained later.
        groundCollider = Physics.OverlapSphere(groundCheck.position, 0.2f, groundLayer);
        if (groundCollider.Length > 0) isGrounded = true;
        else isGrounded = false;
        anim.SetBool("Grounded", isGrounded);

        // If the player is near the ground and hasn't recently jumped, lock y velocity to -0.5.
        // Else, add gravityForce every second (regulated by Time.deltaTime).
        if (isGrounded && !jumped) playerVelocty.y = -0.5f;
        else playerVelocty.y += gravityForce * Time.deltaTime;
    }

    private void HandleInput()
    {
        moveInput = _inputs.Player.Move.ReadValue<Vector2>(); // Read value from inputs using polling.
        if (_inputs.Player.Jump.triggered) jumpBuffer = jumpBufferTime; // Pressing jump doesn't actually jump, just prepares the jump buffer.
        if (jumpBuffer > 0)
        {
            if(coyoteTime > 0)
            {
                jumpBuffer = 0;
                coyoteTime = 0;
                StartCoroutine(Jump());
            }
            else if (_jumps > 0)
            {
                playerVelocty.y = 0;
                _jumps--;
                jumpBuffer = 0;
                coyoteTime = 0;
                StartCoroutine(Jump());
            }
        }// If coyoteTime and jumpBuffer are both positive, then the jump is performed.
        if (_inputs.Player.Jump.WasReleasedThisFrame() && playerVelocty.y > 0) playerVelocty.y *= quickJumpMult; // Velocity is halfed if the player lets go of jump while moving up.
        if(_inputs.Player.Attack.triggered && !inAction)
        {
            inAction = true;
            anim.SetTrigger("Attack");
        }
    }

    private void HandleMovement()
    {
        // This thing should(?) be the exact same as Hendrix's project.
        // I should explain in-line if statements very quickly, though.
        // They follow this format: [boolean expression] ? [if true] : [if false]
        // This decreases the amount of standard if and switch statements required to get something similar to work.
        // Here, two in-line if statements are used to dynamically change the "Speed" float to 0, 0.5, or 1, depending on moveDirection's magnitude and the sprint bool.
        moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        anim.SetFloat("Speed", moveDirection.magnitude != 0 ? (sprint ? 1 : 0.5f) : 0, 0.1f, Time.deltaTime);

        if (moveDirection.magnitude != 0)
        {
            float targetAngle = MathF.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + camPosition.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 newDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            float sprintApplied = 1;
            if (sprint) sprintApplied *= sprintMult;
            cc.Move(newDirection * moveSpeed * sprintApplied * Time.deltaTime);
        }

        anim.SetFloat("vSpeed", playerVelocty.y);
        cc.Move(playerVelocty * Time.deltaTime);
    }

    // The important one. This coroutine helps prevent the problems caused by the allways-running sphere collider.
    // First, it sets jumped to true to actually let jumping function as expected.
    // Second, it sets coyoteTime to zero, to prevent double jumping.
    // Then it actually performs the jump. It has two in-line if statements to control the jump height.
    // The first in-line boosts jump height if the player is sprinting. Why? Because why not?
    // The second in-line reduces the jump height if the player was not holding the button when the jump gets performed.
    // The second in-line thus allows for tapping the jump button right before touching the ground and still getting the expected reduced jump height.
    // Finally, it triggers the jump animation, waits for groundedDelay seconds (I have mine set to 0.1), then sets jumped to false now that we're properly in the air.
    IEnumerator Jump()
    {
        jumped = true;
        //coyoteTime = 0;
        playerVelocty.y = Mathf.Sqrt(jumpForce * -3 * gravityForce * (sprint ? sprintMult : 1) * (_inputs.Player.Jump.IsPressed() ? 1 : quickJumpMult));
        anim.SetTrigger("Jump");
        yield return new WaitForSeconds(groundedDelay);
        jumped = false;
    }
}
