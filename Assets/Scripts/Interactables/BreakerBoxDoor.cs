using System;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakerBoxDoor : MonoBehaviour, IInteractable {
    private bool boxOpened = false;
    [SerializeField] private Animator animator;
    [SerializeField, Self] private AudioSource openSound;
   
    public string PromptText { get; set; } = "Open electric breaker box";

    private void OnValidate() { this.ValidateRefs(); }

    private void OnEnable() { GameEvents.OnSetBreakerBoxDoorOpened += SetBoxOpened; }

    private void OnDisable() { GameEvents.OnSetBreakerBoxDoorOpened -= SetBoxOpened; }

    private void Start() { PromptText = boxOpened ? "Close electric breaker box" : "Open electric breaker box"; }

    public void Interact() {
        Debug.Log("Interacted with breaker box door");
        GameEvents.RaiseOnSetBreakerBoxDoorOpened(!boxOpened);
    }


    public void SetBoxOpened(bool val) {
        openSound.pitch = Random.Range(0.8f, 1.2f);
        openSound.Play();

        Debug.Log("Box opened bool is " + val);
        boxOpened = val;
        PromptText = boxOpened ? "Close electric breaker box" : "Open electric breaker box";
        if (boxOpened) {
            animator.Play("BreakerBoxOpen");
        } else {
            animator.Play("BreakerBoxClose");
        }
    }

    
}