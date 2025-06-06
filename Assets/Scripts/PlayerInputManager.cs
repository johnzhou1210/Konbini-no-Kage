using System;
using KBCore.Refs;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour {
    [SerializeField, Self] private PlayerInput playerInput;
    [SerializeField, Self] private CharacterController characterController;
    [SerializeField] private CinemachinePanTilt cmPanTilt;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask interactMask;
    
    
    private CinemachineCamera activeCamera;

    private bool canInteract = true;
    
    
    #region Night 3
    private bool night3Chase = false;
    private float safeZoneTimeElapsedWithoutMoving = 0f;
    #endregion

    public static event Action<string> OnUpdateInteractPrompt;

    private Vector3 velocity;
    private bool playerDead = false;

    private void OnValidate() { this.ValidateRefs(); }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Invoke(nameof(InitRadius), 1f);
    }

    private void InitRadius() { characterController.radius = 0.5f; }

    private void OnEnable() {
        GameEvents.OnKillPlayerInput += KillPlayerInput;
        GameEvents.OnSetNight3ChaseIntoPlayerInput += SetNight3Chase;
        
        GameQuery.OnGetIsPlayerCaught = () => { return playerDead; };
    }

    private void OnDisable() {
        GameEvents.OnKillPlayerInput -= KillPlayerInput;
        GameEvents.OnSetNight3ChaseIntoPlayerInput -= SetNight3Chase;

        GameQuery.OnGetIsPlayerCaught = null;
    }

    private void Update() {
        if (playerDead) return;
        
        if (night3Chase) {
            if ((GameQuery.OnGetNight3InSafeZone?.Invoke() ?? false) && playerInput.actions["Move"].ReadValue<Vector2>().magnitude < 0.5f ) {
                safeZoneTimeElapsedWithoutMoving += Time.deltaTime;
            } else {
                safeZoneTimeElapsedWithoutMoving = 0f;
            }

            if (safeZoneTimeElapsedWithoutMoving > 3f && !(GameQuery.OnGetIsGateOpened?.Invoke() ?? false) && !playerDead) {
                GameEvents.RaiseOnStalkerEndChase();
                night3Chase = false;
            }
        }
   
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        if (GameQuery.OnGetIsCheckingCameras?.Invoke() ?? false) {
            if (playerInput.actions["CamLeft"].WasPerformedThisFrame()) {
                GameEvents.RaiseOnSetActiveCamera(GameQuery.OnGetCurrentSecurityCameraIndex?.Invoke() - 1 ?? 0);
            } 
            if (playerInput.actions["CamRight"].WasPerformedThisFrame()) {
                GameEvents.RaiseOnSetActiveCamera(GameQuery.OnGetCurrentSecurityCameraIndex?.Invoke() + 1 ?? 0);
            }
        }

        
        if (playerInput.actions["Exit"].WasPerformedThisFrame()) {
            Debug.LogWarning("Trying to exit cam");
            if (GameQuery.OnGetIsCheckingCameras?.Invoke() ?? false) {
                GameEvents.RaiseOnExitSecurityCamera();
                canInteract = true;
            }
        }
        
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

       

        
        if (!GameQuery.OnGetIsCheckingCameras?.Invoke() ?? false) {
            // Player movement
            Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * (5f * Time.deltaTime);
            move = Quaternion.Euler(0, cmPanTilt.PanAxis.Value, 0) * move;
            characterController.Move(move);
        }



        string promptText = TryInteract();


        OnUpdateInteractPrompt?.Invoke(promptText);
    }

    private string TryInteract() {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask)) {
            if (!canInteract) return "";

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null) {
                string promptText = interactable.PromptText;
                if (playerInput.actions["Interact"].IsPressed()) {
                    canInteract = false;
                    interactable.Interact();
                    Invoke("ResetInteract", 2f);
                }

                return promptText;
            }
        }

        return "";
    }

    private void ResetInteract() {
        canInteract = true;
    }

    private void KillPlayerInput() {
        playerDead = true;
    }

    private void SetNight3Chase(bool val) {
        night3Chase = val;
    }
    
    
    
}