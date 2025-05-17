using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;

public class PoliceCar: MonoBehaviour {
    [SerializeField, Self] private AudioSource sirenSound;
    private void OnValidate() {
        this.ValidateRefs();
    }

    public void PlaySirenSound() {
        sirenSound.Play();
    }


}
