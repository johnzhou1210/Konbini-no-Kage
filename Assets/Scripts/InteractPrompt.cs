using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;

public class InteractPrompt : MonoBehaviour {
    [SerializeField, Self] private TextMeshProUGUI prompt;
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Start() {
        PlayerInputManager.OnUpdateInteractPrompt += PromptInteract;
    }

    private void PromptInteract(string promptText) {
        prompt.text = promptText;
    }
    
}
