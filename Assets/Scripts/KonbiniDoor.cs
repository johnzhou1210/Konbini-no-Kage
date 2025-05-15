using System;
using KBCore.Refs;
using UnityEngine;

public class KonbiniDoor : MonoBehaviour {
    [SerializeField, Self] private Animator animator;
    [SerializeField, Self] private new AudioSource audio;

    private bool debounce = false;

    private void OnEnable() {
        GameEvents.OnKonbiniDoorOpened += OpenDoor;
    }

    private void OnDisable() {
        GameEvents.OnKonbiniDoorOpened -= OpenDoor;
    }

    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") || other.CompareTag("NPC")) {
            OpenDoor();
        }
    }

    private void Start() {
        // Invoke(nameof(OpenDoor), 1f);
    }

    private void OpenDoor() {
        if (debounce) return;
        debounce = true;
        animator.Play("KonbiniDoorOpen");
        audio.Play();
    }

    public void ResetDebounce() {
        debounce = false;
    }
}
