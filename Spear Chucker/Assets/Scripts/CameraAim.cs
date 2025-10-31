using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraAim : MonoBehaviour
{
    [Header("Camera References")]
    public GameObject mainCamera;
    public GameObject aimCamera;
    public GameObject crosshairUI;

    [Header("Aim Settings")]
    public float aimBlendTime = 0.25f;

    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private InputAction aimAction;
    private float aimValue;

    void Start()
    {
        // Get references to required components
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        
        // Get aim action from input system
        if (playerInput != null)
        {
            aimAction = playerInput.actions["Aim"];
        }

        // Ensure initial camera states
        if (mainCamera != null) mainCamera.SetActive(true);
        if (aimCamera != null) aimCamera.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(false);
    }

    void Update()
    {
        // Get aim value from input system
        if (aimAction != null)
        {
            aimValue = aimAction.ReadValue<float>();
        }
        // Fallback: try to get from PlayerMovement if available
        else if (playerMovement != null)
        {
            // You'll need to add a public method or property in PlayerMovement to access aim state
            // For now, we'll use a placeholder
            aimValue = 0f; // Replace with actual aim value from PlayerMovement
        }

        HandleAimState();
    }

    void HandleAimState()
    {
        if (aimValue > 0.5f) // Aiming (button pressed)
        {
            if (mainCamera != null && mainCamera.activeInHierarchy)
            {
                mainCamera.SetActive(false);
            }
            
            if (aimCamera != null && !aimCamera.activeInHierarchy)
            {
                aimCamera.SetActive(true);
                StartCoroutine(ShowCrosshair());
            }
        }
        else // Not aiming
        {
            if (aimCamera != null && aimCamera.activeInHierarchy)
            {
                aimCamera.SetActive(false);
            }
            
            if (mainCamera != null && !mainCamera.activeInHierarchy)
            {
                mainCamera.SetActive(true);
            }
            
            if (crosshairUI != null && crosshairUI.activeInHierarchy)
            {
                crosshairUI.SetActive(false);
            }
        }
    }

    IEnumerator ShowCrosshair()
    {
        if (crosshairUI != null)
        {
            yield return new WaitForSeconds(aimBlendTime);
            crosshairUI.SetActive(true);
        }
    }

    // Public method to set aim value externally (if needed)
    public void SetAimValue(float value)
    {
        aimValue = value;
    }
}