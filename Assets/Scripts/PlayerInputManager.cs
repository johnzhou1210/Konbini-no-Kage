using System;
using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField, Self] private PlayerInput playerInput;
    [SerializeField, Self] private CharacterController characterController;
    [SerializeField] private CinemachinePanTilt cmPanTilt;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private Camera playerCamera;
    
    public static event Action<string> OnUpdateInteractPrompt;

    private Vector3 velocity;
    
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Invoke(nameof(InitRadius), 1f);
    }

    private void InitRadius() {
        characterController.radius = 0f;
    }
    
    private void Update() {
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
        
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        // Calculate the movement direction based on input and camera orientation
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * (5f * Time.deltaTime);

        // Adjust movement based on camera pan and tilt values
        move = Quaternion.Euler(0, cmPanTilt.PanAxis.Value, 0) * move;

        characterController.Move(move); // Translate the player in world space

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        string promptText = "";
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)) {
            
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null) {
                promptText = interactable.PromptText;
                if (playerInput.actions["Interact"].IsPressed()) {
                    interactable.Interact();
                }
                
            }
        }

        OnUpdateInteractPrompt?.Invoke(promptText);
        
    }
}
