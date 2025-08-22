using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TargetLock : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Camera mainCamera;                       // Reference to the main camera
    [SerializeField] private CinemachineCamera cinemachineCamera;     // Cinemachine 3.x camera

    [Header("UI")]
    [SerializeField] private Image aimIcon;                           // UI icon shown when a target is locked

    [Header("Settings")]
    [SerializeField] private string enemyTag;                         // The tag used for enemies
    [SerializeField] private Vector2 targetLockOffset;                // Offset for fine-tuning the lock-on position
    [SerializeField] private float minDistance;                       // Minimum distance to target before camera stops rotating
    [SerializeField] private float maxDistance;                       // Maximum lock-on range

    public bool isTargeting;             // True if currently locked onto an enemy
    private float maxAngle = 90f;        // Maximum angle (in degrees) in front of the camera to detect enemies
    private Transform currentTarget;     // The currently locked target

    private float mouseX;                // Horizontal input value for camera
    private float mouseY;                // Vertical input value for camera

    private PlayerControls controls;     // Input actions reference

    // Input Axis Controllers for Cinemachine 3.x
    private InputAxisController xAxisController;
    private InputAxisController yAxisController;

    private void Awake()
    {
        controls = new PlayerControls();

        // Subscribe to lock-on action
        controls.Player.TargetLock.performed += ctx => AssignTarget();

        // Subscribe to look action
        controls.Player.Look.performed += ctx => OnLook(ctx.ReadValue<Vector2>());
        controls.Player.Look.canceled += ctx => OnLook(Vector2.zero);

        // Get references to the axis controllers from the CinemachineCamera
        xAxisController = cinemachineCamera.GetComponentInChildren<InputAxisController>(true); // Horizontal
        yAxisController = cinemachineCamera.GetComponentsInChildren<InputAxisController>(true)[1]; // Vertical
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    void Update()
    {
        if (isTargeting)
        {
            NewInputTarget(currentTarget); // Override inputs if locked
        }

        // Show or hide aim icon depending on targeting
        if (aimIcon) aimIcon.gameObject.SetActive(isTargeting);

        // Apply calculated inputs to Cinemachine’s InputAxisControllers
        if (xAxisController) xAxisController.Input.Value = mouseX;
        if (yAxisController) yAxisController.Input.Value = mouseY;
    }

    private void OnLook(Vector2 lookInput)
    {
        // Only apply player input if not targeting
        if (!isTargeting)
        {
            mouseX = lookInput.x;
            mouseY = lookInput.y;
        }
    }

    private void AssignTarget()
    {
        if (isTargeting)
        {
            // lock off of target
            isTargeting = false;
            currentTarget = null;
            return;
        }

        // Lock onto the closest target
        GameObject target = ClosestTarget();
        if (target)
        {
            currentTarget = target.transform;
            isTargeting = true;
        }
    }

    private void NewInputTarget(Transform target)
    {
        if (!currentTarget) return;

        Vector3 viewPos = mainCamera.WorldToViewportPoint(target.position);

        // Move aim icon
        if (aimIcon)
            aimIcon.transform.position = mainCamera.WorldToScreenPoint(target.position);

        // Stop adjusting if too close
        if ((target.position - transform.position).magnitude < minDistance) return;

        // Override inputs to center the camera
        mouseX = (viewPos.x - 0.5f + targetLockOffset.x) * 3f;
        mouseY = (viewPos.y - 0.5f + targetLockOffset.y) * 3f;
    }

    private GameObject ClosestTarget()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject closest = null;
        float distance = maxDistance;
        Vector3 position = transform.position;

        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.magnitude;

            if (curDistance < distance)
            {
                if (Vector3.Angle(diff.normalized, mainCamera.transform.forward) < maxAngle)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }
}