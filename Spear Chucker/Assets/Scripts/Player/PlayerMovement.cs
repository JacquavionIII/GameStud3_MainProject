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

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 3f;      // Controling the look sensitivity
    public float smoothTime = 0.05f;        // How quickly the camera lerps
    public float minLookY = -60f;           // Clamping the vertical look (up) 
    public float maxLookY = 60f;            // Clamping the vertical look (down)

    [Header("References")]
    public Rigidbody rb;
    public Animator animator;
    public SpearToss spearToss;
    public SkinnedMeshRenderer targetMeshRenderer; //Reference to the character mesh renderer, since this script is not attched to it (i love finding backdoor methods)
    public Material healMat;
    public Material defaultMat;
    public Transform healVFX;               // Reference to the healing VFX object
    public Transform senseVFX;              // Reference to the sensing VFX object
    private Vector2 currentInput;           // Current input from keyboard/gamepad
    private Vector2 currentLook;            // Current input from mouse/gamepad
    private Vector2 smoothLook;             // Smoothed look direction
    private Vector2 lookVelocity;           // Velocity used by SmoothDamp
    private Vector3 velocity;               // Jump velocity
    private bool isGrounded;
    private float camRotationX;             // Vertical camera rotation


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
        lookAction = playerInput.actions["Look"];
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

        lookAction.Enable();
        lookAction.performed += OnLook;
        lookAction.canceled += OnLook;

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

        lookAction.performed -= OnLook;
        lookAction.canceled -= OnLook;

        jumpAction.performed -= OnJump;

        attackAction.performed -= OnAttack;

        healAction.performed -= OnHeal;

        senseAction.performed -= OnSense;

    }

    // Called whenever Move input changes
    public void OnMove(InputAction.CallbackContext context)
    {
        currentInput = context.ReadValue<Vector2>();
        animator.SetBool("isRunning", true); // Set running animation when there's input
        if (context.canceled)
        {
            animator.SetBool("isRunning", false); // Stop running animation when input stops
        }
    }

    // Called whenever Look input changes
    public void OnLook(InputAction.CallbackContext context)
    {
        currentLook = context.ReadValue<Vector2>();
    }

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
            // I'll add better healing logic here later, just testing this for now
            targetMeshRenderer.material = healMat;
            Debug.Log("Heal action performed");
            animator.SetBool("isHealing", true);
            healVFX.gameObject.SetActive(true); // Activate healing VFX
        }
        else if (context.canceled)
        {
            targetMeshRenderer.material = defaultMat;
            Debug.Log("Heal action canceled");
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
        else if (context.canceled)
        {
            senseActive = false;
            senseTimer = 5f;
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

        HandleCameraLook(); //Calling this in Update for smoother camera movement

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
       // real-time movement cause the orignal one was fucking out and made me tweak a bit....
        Vector3 move = (transform.right * currentInput.x + transform.forward * currentInput.y).normalized * speed;

        // movement along x with the rb, if this fucks up I'm gonna tweak cause it was working before
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // Apply gravity & jump (velocity.y is modified in Update or OnJump)
        velocity.y += gravity * Time.fixedDeltaTime;
        rb.AddForce(Vector3.up * velocity.y, ForceMode.Acceleration);
    }
    
    void HandleCameraLook()
    {
        // Smooth input with Lerp (or SmoothDamp for extra smoothness)
        smoothLook = Vector2.SmoothDamp(smoothLook, currentLook, ref lookVelocity, smoothTime); //using the ref to keep track of the velocity to make the smoothing work

        // Horizontal rotation (rotate the player body)
        transform.Rotate(Vector3.up * smoothLook.x * lookSensitivity * Time.deltaTime);

        // Vertical rotation (rotate camera only)
        camRotationX -= smoothLook.y * lookSensitivity * Time.deltaTime;
        camRotationX = Mathf.Clamp(camRotationX, minLookY, maxLookY);

        cameraTransform.localRotation = Quaternion.Euler(camRotationX, 0f, 0f);
    }
}
