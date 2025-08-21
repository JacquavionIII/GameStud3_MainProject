using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Import the new Input System namespace

public class TargetLock : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Camera mainCamera;                      // Reference to the main camera
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook; // Cinemachine FreeLook camera used for character tracking

    [Header("UI")]
    [SerializeField] private Image aimIcon;                          // UI icon shown when a target is locked

    [Header("Settings")]
    [SerializeField] private string enemyTag;                        // The tag used for enemies
    [SerializeField] private Vector2 targetLockOffset;               // Offset for fine-tuning the lock-on position
    [SerializeField] private float minDistance;                      // Minimum distance to target before camera stops rotating
    [SerializeField] private float maxDistance;                      // Maximum lock-on range

    public bool isTargeting;            // True if currently locked onto an enemy
    private float maxAngle = 90f;       // Maximum angle (in degrees) in front of the camera to detect enemies
    private Transform currentTarget;    // The currently locked target

    private float mouseX;               // X-axis input for Cinemachine (manual control)
    private float mouseY;               // Y-axis input for Cinemachine (manual control)

    // Reference to the generated InputActions (from PlayerControls.inputactions asset)
    private PlayerControls controls;

    private void Awake()
    {
        // Initialize the Input System actions
        controls = new PlayerControls();

        // Subscribe to the "TargetLock" action (button press)
        controls.Player.TargetLock.performed += ctx => AssignTarget();

        // Subscribe to the "Look" action (mouse/stick movement)
        controls.Player.Look.performed += ctx => OnLook(ctx.ReadValue<Vector2>());
        controls.Player.Look.canceled += ctx => OnLook(Vector2.zero); // Reset when input is released
    }

    private void OnEnable()
    {
        // Enable the Gameplay action map when this script is active
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        // Disable the Gameplay action map when this script is inactive
        controls.Player.Disable();
    }

    void Start()
    {
        // Disable Cinemachine’s built-in input axis handling
        // We will feed it input manually using the new system
        cinemachineFreeLook.m_XAxis.m_InputAxisName = "";
        cinemachineFreeLook.m_YAxis.m_InputAxisName = "";
    }

    void Update()
    {
        if (isTargeting)
        {
            // Override input values to keep the camera locked onto the target
            NewInputTarget(currentTarget);
        }

        // Show or hide the aim icon depending on lock-on state
        if (aimIcon)
            aimIcon.gameObject.SetActive(isTargeting);

        // Send our calculated input values to Cinemachine
        cinemachineFreeLook.m_XAxis.m_InputAxisValue = mouseX;
        cinemachineFreeLook.m_YAxis.m_InputAxisValue = mouseY;
    }

    // Called when the Look input action is triggered
    private void OnLook(Vector2 lookInput)
    {
        // Only apply player input if not locked onto a target
        if (!isTargeting)
        {
            mouseX = lookInput.x; // Horizontal look input
            mouseY = lookInput.y; // Vertical look input
        }
    }

    // Assigns or clears a lock-on target
    private void AssignTarget()
    {
        if (isTargeting)
        {
            // If already targeting, cancel lock-on
            isTargeting = false;
            currentTarget = null;
            return;
        }

        // Otherwise, try to lock onto the closest target
        if (ClosestTarget())
        {
            currentTarget = ClosestTarget().transform;
            isTargeting = true;
        }
    }

    // Adjusts input values to keep the camera focused on the target
    private void NewInputTarget(Transform target)
    {
        if (!currentTarget) return; // If no target, do nothing

        // Convert target position to viewport coordinates (0-1 range)
        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);

        // Move the aim icon to target’s screen position
        if (aimIcon)
            aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position);

        // Stop adjusting if player is too close to the target
        if ((target.position - transform.position).magnitude < minDistance) return;

        // Adjust X and Y input values to center the camera on the target
        mouseX = (viewPos.x - 0.5f + targetLockOffset.x) * 3f;
        mouseY = (viewPos.y - 0.5f + targetLockOffset.y) * 3f;
    }

    // Finds the closest target in range and in front of the camera
    private GameObject ClosestTarget()
    {
        // Get all objects with the enemy tag
        GameObject[] gos = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closest = null;
        float distance = maxDistance;
        float currAngle = maxAngle;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.magnitude;

            if (curDistance < distance) // Only check if within max range
            {
                // Ensure enemy is within the camera's forward angle
                if (Vector3.Angle(diff.normalized, mainCamera.transform.forward) < maxAngle)
                {
                    closest = go;
                    currAngle = Vector3.Angle(diff.normalized, mainCamera.transform.forward.normalized);
                    distance = curDistance; // Update closest distance
                }
            }
        }
        return closest; // Return the closest valid enemy
    }

    // Draws a visual representation of lock-on range in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}
