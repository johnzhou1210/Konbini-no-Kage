using System;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class GateDoor : MonoBehaviour, IInteractable {
    private bool gateOpened = false;
    [SerializeField] private Animator animator;
    [SerializeField, Self] private AudioSource gateOpenSFX;
    [SerializeField] private AudioSource gateCloseSFX;
    public string PromptText { get; set; } = "Open gate";


    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnEnable() {
        GameEvents.OnSetGateDoorOpened += SetGateOpened;
        
        GameQuery.OnGetIsGateOpened = GetGateOpened;
    }

    private void OnDisable() {
        GameEvents.OnSetGateDoorOpened -= SetGateOpened;
        
        GameQuery.OnGetIsGateOpened = null;
    }

    

    public void Interact() {
        Debug.Log("Interacted with gate door");
        
        if ((GameQuery.OnGetCurrentNight?.Invoke() ?? 1) < 3) {
            GameEvents.RaiseOnDialogTypewriterStartTypewriter("No reason to go in here right now.");
        } else {
            if (GameQuery.OnGetStalkerAngryCutsceneInProgress?.Invoke() ?? false) {
                GameEvents.RaiseOnDialogTypewriterStartTypewriter("No, that's a death wish!");
            } else {
                GameEvents.RaiseOnSetGateDoorOpened(!gateOpened);
            }
            
        }

    }

    public void SetGateOpened(bool val) {
        if (val) {
            gateOpenSFX.pitch = Random.Range(0.8f, 1.2f);
            gateOpenSFX.Play();
        } else {
            gateCloseSFX.pitch = Random.Range(0.8f, 1.2f);
            gateCloseSFX.Play();
        }
      
        Debug.Log("Gate opened bool is " + val);
        gateOpened = val;
        PromptText = gateOpened ? "Close gate" : "Open gate";
        if (gateOpened) {
            animator.Play("BackDoorOpen");
        } else {
            animator.Play("BackDoorClose");
        }
    }

    private bool GetGateOpened() {
        return gateOpened;
    }
    
}