using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 12f;                
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float senseTimer = 5f;          // Duration for which the sense VFX remains active
    public bool senseActive = false;      // To track if the sense action is active

    [Header("Ground Check")]
    public Transform groundCheck;            
    public float groundDistance = 0.4f;         
    public LayerMask groundLayer;

    // [Header("Camera Settings")]
    // public Transform cameraTransform;
    // [SerializeField] private float mouseSensitivity = 0.1f;
    // [SerializeField] private float controllerSensitivity = 200f;      // Controling the look sensitivity
    // public float smoothTime = 0.05f;        // How quickly the camera lerps
    // public float minLookY = -60f;           // Clamping the vertical look (up) 
    // public float maxLookY = 60f;            // Clamping the vertical look (down)
    // private float camRotationX;             // Vertical camera rotation
    private Vector2 currentInput;           // Current input from keyboard/gamepad
    // private Vector2 currentLook;            // Current input from mouse/gamepad
    // private Vector2 smoothLook;             // Smoothed look direction
    // private Vector2 lookVelocity;           // Velocity used by SmoothDamp

    [Header("References")]
    public MeatDrop meat;
    public Health health;
    public Rigidbody rb;
    public Animator animator;
    public SpearToss spearToss;
    public SkinnedMeshRenderer targetMeshRenderer; //Reference to the character mesh renderer, since this script is not attched to it (i love finding backdoor methods)
    public Material healMat;
    public Material defaultMat;
    public Transform healVFX;               // Reference to the healing VFX object
    public Transform senseVFX;              // Reference to the sensing VFX object
    public Transform cameraTrans;
    public float rotationSpeed = 10f;
    private Vector3 velocity;               // Jump velocity
    private bool isGrounded;

    [Header("Input Actions")]
    private InputAction moveAction;         // Input action for movement
    private InputAction lookAction;         // Input action for looking around
    private InputAction jumpAction;         // Input action for jumping
    private InputAction attackAction;       // Input action for attacking
    private InputAction healAction;       // Input action for healing
    private InputAction senseAction;       // Input action for sensing 


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Get the player's input actions
        var playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        //lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];
        healAction = playerInput.actions["Heal"];
        senseAction = playerInput.actions["Sense"];
    }

    void OnEnable()     // Subscribe to input actions when the script is enabled
    {
        moveAction.Enable();
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        // lookAction.Enable();
        // lookAction.performed += OnLook;
        // lookAction.canceled += OnLook;

        jumpAction.Enable();
        jumpAction.performed += OnJump;

        attackAction.Enable();
        attackAction.performed += OnAttack;

        healAction.Enable();
        healAction.performed += OnHeal;

        senseAction.Enable();
        senseAction.performed += OnSense;        
    }

    void OnDisable()   // Unsubscribe from input actions when the script is disabled
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;

        // lookAction.performed -= OnLook;
        // lookAction.canceled -= OnLook;

        jumpAction.performed -= OnJump;

        attackAction.performed -= OnAttack;

        healAction.performed -= OnHeal;

        senseAction.performed -= OnSense;

    }

    // Called whenever Move input changes
    public void OnMove(InputAction.CallbackContext context)
    {
        currentInput = context.ReadValue<Vector2>();
        
    }

    // Called whenever Look input changes
    // public void OnLook(InputAction.CallbackContext context)
    // {
    //     currentLook = context.ReadValue<Vector2>();
    // }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            // Jump velocity based on physics equation
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            spearToss.Shoot();
            animator.SetBool("isShooting", true);
        }
        else if (context.canceled)
        {
            animator.SetBool("isShooting", false);
        }
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (meat.meatAmount > 0)
            {
                // I'll add better healing logic here later, just testing this for now
                targetMeshRenderer.material = healMat;
                Debug.Log("Heal action performed");
                animator.SetBool("isHealing", true);
                healVFX.gameObject.SetActive(true); // Activate healing VFX
                meat.meatAmount--;
                health.Heal(10);
            }
        }
        else if (context.canceled)
        {
            targetMeshRenderer.material = defaultMat;
            Debug.Log("No meat to heal");
            animator.SetBool("isHealing", false);
            healVFX.gameObject.SetActive(false); // Deactivate healing VFX
        }
    }

    public void OnSense(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            senseVFX.gameObject.SetActive(true); // Activate sensing VFX
            senseActive = true;
            senseTimer = 5f; // yes im doing this 10 million times cause im making sure it doesnt screw me over like its been
        }
    }

    public void EndSense() //Called by an animation event at the end of the sense animation
    {
        senseVFX.gameObject.SetActive(false); // Deactivate sensing VFX
        senseActive = false;
        senseTimer = 5f; 
    }

    public void Start()
    {
        foreach (var smr in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (smr.gameObject.name == "SuperHero_Male") //We're looking for the specific mesh because this thing keeps on screwing with me in the heal function (fuck I hate shaders)
            {
                targetMeshRenderer = smr;
                break;
            }
        }

        animator = GetComponentInChildren<Animator>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IPickUpAble>() != null)
        {
            other.GetComponent<IPickUpAble>().OnPickUp();
        }
    }

    void Update()
    {
        // Groundcheck lol
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        
        // Reset vertical velocity if grounded and falling
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force keeps player grounded
        }

        //HandleCameraLook(); //Calling this in Update for smoother camera movement

        if (senseActive)
        {
            senseTimer -= Time.deltaTime;
            if (senseTimer <= 0f)
            {
                EndSense();
                senseActive = false;
                senseTimer = 5f;
            }
        }
        else
        {
            senseTimer = 5f;
        }
    }

    void FixedUpdate()
    {
        HandleMovement();//Rather call this in FixedUpdate for physics-based movement (and Im lowkey experimenting here)
    }

    void HandleMovement()
    {
        Transform cam = cameraTrans != null ? cameraTrans : (Camera.main != null ? Camera.main.transform : transform);

        // Project camera forward/right onto XZ plane and normalize
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1f, 0f, 1f)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1f, 0f, 1f)).normalized;

        
        // Preserve analog magnitude (0..1) and clamp keyboard diagonal input to 1
        float inputMagnitude = Mathf.Clamp01(currentInput.magnitude);

        // real-time movement cause the orignal one was fucking out and made me tweak a bit....
        Vector3 inputDir = camRight * currentInput.x + camForward * currentInput.y;
        Vector3 moveDir = inputDir.sqrMagnitude > 0.0001f ? inputDir.normalized : Vector3.zero;

        // Desired horizontal velocity (normalized direction * speed * input strength)
        Vector3 desiredVelocity = moveDir * speed * inputMagnitude;

        // Apply horizontal velocity while keeping current vertical velocity
        rb.linearVelocity = new Vector3(desiredVelocity.x, rb.linearVelocity.y, desiredVelocity.z);

        // Rotate player body toward movement direction when there's input
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
        
        // Apply gravity & jump (velocity.y is modified in Update or OnJump)
        velocity.y += gravity * Time.fixedDeltaTime;
        rb.AddForce(Vector3.up * velocity.y, ForceMode.Acceleration);

        bool isRunning = currentInput.magnitude > 0.1f; //if the player's input magnitude is greater than a small threshold then it'll trigger the bool for the anim
        animator.SetBool("isRunning", isRunning); // Set running animation when there's input

        bool isRunBack = currentInput.magnitude < -0.1f;
        animator.SetBool("isRunBack", isRunBack);

        //other stuff: (a side note)
        //The player stops being able to move after a bit. And when standing still and trying to move right or left, the player spins around instead going straight in that direction...
    }
    
    //Experimenting with cinemachine logic 
    // void HandleCameraLook()
    // {
    //     // Detect if the player is using mouse input
    //     bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;

    //     // Scale look input depending on input device
    //     Vector2 scaledLook = usingMouse
    //     ? currentLook * mouseSensitivity
    //     : currentLook * controllerSensitivity * Time.deltaTime;

    //     // Smooth input with Lerp (or SmoothDamp for extra smoothness)
    //     smoothLook = Vector2.SmoothDamp(smoothLook, currentLook, ref lookVelocity, smoothTime); //using the ref to keep track of the velocity to make the smoothing work

    //     // Horizontal rotation (rotate the player body)
    //     transform.Rotate(Vector3.up * smoothLook.x);

    //     // Vertical rotation (rotate camera only)
    //     camRotationX -= smoothLook.y;
    //     camRotationX = Mathf.Clamp(camRotationX, minLookY, maxLookY);

    //     cameraTransform.localRotation = Quaternion.Euler(camRotationX, 0f, 0f);
    // }
}
