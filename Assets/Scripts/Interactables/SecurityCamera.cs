using System;
using KBCore.Refs;
using UnityEngine;

public class SecurityCamera : MonoBehaviour, IInteractable
{
    public string PromptText { get; set; } = "Check Security Cameras";
    [SerializeField] private MeshRenderer litScreen, unlitScreen;

    public void Interact() {
        Debug.Log("Interacted with security camera");
        GameEvents.RaiseOnSetActiveCamera(GameQuery.OnGetCurrentSecurityCameraIndex?.Invoke() ?? 0);
        LightScreen();
    }

    public void LightScreen() {
        litScreen.enabled = true;
        unlitScreen.enabled = false;
    }

    public void UnlitScreen() {
        litScreen.enabled = false;
        unlitScreen.enabled = true;
    }
    
    
    
}