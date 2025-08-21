using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Cinemachine;
using UnityEngine.UI;

public class LockOnSys : MonoBehaviour
{
    [Header("Objects")]
    [Space]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineFreeLook cinemachineFreeLook;
    [Space]
    [Header("UI")]
    [SerializeField] private GameObject lockOnIcon;
    [Space]
    [Header("Settings")]
    [SerializeField] private string enemyTag;
    [SerializeField] private KeyCode _Input;
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
