using System;
using KBCore.Refs;
using UnityEngine;

public class CinematicListener : MonoBehaviour
{
    [SerializeField, Self] private Animator animator;
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Start() {
        GameManager.OnNightUpdate += PlayCinematicAnimation;
    }

    private void PlayCinematicAnimation(int night) {
        animator.Play("CinematicIntro");
    }

}
