using System;
using KBCore.Refs;
using UnityEngine;

public class KonbiniSideDoor : MonoBehaviour {
    [SerializeField, Self] private Animator animator;
    [SerializeField] private AudioSource openDoor, closeDoor;

    private bool debounce = false;

    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") || other.CompareTag("NPC")) {
            OpenDoor();
        }
    }

    private void Start() {
        // Invoke(nameof(OpenDoor), 1f);
    }

    public void OpenDoor() {
        if (debounce) return;
        debounce = true;
        animator.Play("OpenFromInside");
        Invoke("ResetDebounce", 2.05f);
    }

    public void OpenSound() {
        openDoor.Play();
    }

    public void CloseSound() {
        closeDoor.Play();
    }

    public void ResetDebounce() {
        debounce = false;
    }
}
