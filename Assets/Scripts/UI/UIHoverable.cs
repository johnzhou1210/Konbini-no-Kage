using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHoverable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    bool hovering = false;

    [SerializeField, Child] TextMeshProUGUI hoverableText;
    string originalText;
    Color originalColor;

    void Awake() {
        originalText = hoverableText.text;
        originalColor = hoverableText.color;
    }

    void OnValidate() {
        this.ValidateRefs();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovering = false;
    }

    void Update() {
        if (GetComponent<Button>().interactable) {
            hoverableText.text = hovering ? $"> {originalText}" : originalText;
            hoverableText.color = hovering ? Color.white : originalColor;
        } else {
            hoverableText.color = new Color(50f/255,50f/255,50f/255,1);
        }

    }

}
