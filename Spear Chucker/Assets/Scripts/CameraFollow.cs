using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform followTransform; // Camera pivot (child of player)
    public Transform cameraTransform; // Actual camera
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float controllerSensitivity = 200f;
    public float smoothTime = 0.05f;
    public float minLookY = -60f;
    public float maxLookY = 60f;
    
    [Header("Player Reference")]
    public Transform playerTransform; // Reference to player transform

    // Input variables
    private Vector2 currentLook;
    private Vector2 smoothLook;
    private Vector2 lookVelocity;
    private float camRotationX;

    // Input actions
    private InputAction lookAction;
    private PlayerInput playerInput;

    void Awake()
    {
        // Get the PlayerInput component from the player
        playerInput = GetComponentInParent<PlayerInput>();
        
        lookAction = playerInput.actions["Look"];
        

        // If playerTransform isn't set, try to find it
        if (playerTransform == null)
        {
            playerTransform = transform.parent; // Assuming camera is child of player
        }
    }

    void OnEnable()
    {
        if (lookAction != null)
        {
            lookAction.Enable();
            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;
        }
    }

    void OnDisable()
    {
        if (lookAction != null)
        {
            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        currentLook = context.ReadValue<Vector2>();
    }

    void Update()
    {
        HandleCameraLook();
    }

    void HandleCameraLook()
    {
        if (currentLook == Vector2.zero) return;

        // Detect if the player is using mouse input
        bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;

        // Scale look input depending on input device
        Vector2 scaledLook = usingMouse
            ? currentLook * mouseSensitivity
            : currentLook * controllerSensitivity * Time.deltaTime;

        // Smooth input with SmoothDamp
        smoothLook = Vector2.SmoothDamp(smoothLook, scaledLook, ref lookVelocity, smoothTime);

        // Only process if we have meaningful input
        if (smoothLook.magnitude > 0.01f)
        {
            // Rotate player horizontally based on mouse X
            if (playerTransform != null)
            {
                playerTransform.Rotate(Vector3.up * smoothLook.x);
            }

            // Handle vertical rotation on camera pivot
            if (followTransform != null)
            {
                camRotationX -= smoothLook.y;
                camRotationX = Mathf.Clamp(camRotationX, minLookY, maxLookY);
                followTransform.localRotation = Quaternion.Euler(camRotationX, 0f, 0f);
            }
        }
    }
}