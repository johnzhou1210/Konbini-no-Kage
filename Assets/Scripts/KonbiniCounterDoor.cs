using System;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class KonbiniCounterDoor : MonoBehaviour {
    [SerializeField, Self] private Animator animator;
    [SerializeField, Self] private AudioSource openDoor;

    private bool debounce = false;

    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") || other.CompareTag("NPC")) {
            OpenDoor();
        }
    }
    
    private void OpenDoor() {
        if (debounce) return;
        debounce = true;
        openDoor.pitch = Random.Range(0.8f, 1.2f);
        openDoor.Play();
        animator.Play("CounterOpen");
        Invoke("ResetDebounce", 1.8f);
    }
    
    public void ResetDebounce() {
        debounce = false;
    }
}
