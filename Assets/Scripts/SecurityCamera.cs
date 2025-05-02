using System;
using UnityEngine;

public class SecurityCamera : MonoBehaviour, IInteractable
{
    public string PromptText { get; set; } = "Check CCTV";

    
    
    public void Interact() {
        Debug.Log("Interacted with security camera");
        SecurityCameraManager.Instance.SetActiveCamera(SecurityCameraManager.Instance.SecurityCameraIndx);
    }
}