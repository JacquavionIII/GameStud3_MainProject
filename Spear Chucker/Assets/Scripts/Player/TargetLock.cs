using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TargetLock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;                       // Reference to the main camera
    [SerializeField] private Image aimIcon;                           // UI icon shown when a target is locked

    [Header("Settings")]
    [SerializeField] private string enemyTag = "Enemy";               // The tag used for enemies
    [SerializeField] private string humanEnemyTag = "HumanEnemy";     // The tag used for human enemies
    [SerializeField] private Vector2 targetLockOffset;                // Offset for fine-tuning the lock-on position
    [SerializeField] private float minDistance = 2f;                  // Minimum distance to target before camera stops rotating
    [SerializeField] private float maxDistance = 20f;                // Maximum distance to search for targets
    [SerializeField] private float rotationSpeed = 5f;               // How fast the camera turns toward target        

    public bool isTargeting;             // True if currently locked onto an enemy
    private float maxAngle = 90f;        // Maximum angle (in degrees) in front of the camera to detect enemies
    public Transform currentTarget;     // The currently locked target

    private PlayerInput playerInput;
    private InputAction targetLockAction;


    private void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        targetLockAction = playerInput.actions["TargetLock"]; // Make sure this exists in your InputActions
    }

    private void OnEnable()
    {
        targetLockAction.Enable();
        targetLockAction.performed += AssignTarget;
    }

    private void OnDisable()
    {
        targetLockAction.performed -= AssignTarget;
        targetLockAction.canceled -= AssignTarget;

    }

    void Update()
    {
        if (isTargeting && currentTarget != null)
        {
            LockCameraOnTarget();

            //Move Ui aim icon
            if (aimIcon)
            {
                aimIcon.gameObject.SetActive(true);
                Vector3 screenPos = mainCamera.WorldToScreenPoint(currentTarget.position);
                aimIcon.transform.position = screenPos + (Vector3)targetLockOffset;
            }
        }
        else
        {
            if (aimIcon)
            {
                aimIcon.gameObject.SetActive(false);
            }
        }
    }

    private void AssignTarget(InputAction.CallbackContext context)
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
        if (target != null)
        {
            currentTarget = target.transform;
            isTargeting = true;
        }
    }

    private void LockCameraOnTarget()
    {
        // Vector from camera to target
        Vector3 dirToTarget = currentTarget.position - mainCamera.transform.position;

        // Don't rotate if too close
        if (dirToTarget.magnitude < minDistance) return;

        // Smoothly rotate camera to face target
        Quaternion lookRotation = Quaternion.LookRotation(dirToTarget.normalized);
        mainCamera.transform.rotation = Quaternion.Slerp(
            mainCamera.transform.rotation,
            lookRotation,
            rotationSpeed * Time.deltaTime
        );

        // Also rotate player body to face the target
        Vector3 flatDir = dirToTarget;
        flatDir.y = 0; // ignore vertical for player rotation
        if (flatDir.sqrMagnitude > 0.01f)
        {
            Quaternion bodyRotation = Quaternion.LookRotation(flatDir.normalized);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                bodyRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private GameObject ClosestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject[] humanEnemies = GameObject.FindGameObjectsWithTag(humanEnemyTag);

        // To combine both arrays so that i can 1) expand on enemy types later and 2) avoid duplicate code that will fuck me over later
        GameObject[] allEnemies = new GameObject[enemies.Length + humanEnemies.Length];
        enemies.CopyTo(allEnemies, 0);
        humanEnemies.CopyTo(allEnemies, enemies.Length);

        GameObject closest = null;
        float distance = maxDistance;

        foreach (GameObject enemy in allEnemies)
        {
            Vector3 diff = enemy.transform.position - transform.position;
            float curDistance = diff.magnitude;

            if (curDistance < distance)
            {
                if (Vector3.Angle(diff.normalized, mainCamera.transform.forward) < maxAngle)
                {
                    closest = enemy;
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