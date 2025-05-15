using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float charDelay = 0.05f;

    private Coroutine typingCoroutine;


    private void OnEnable() {
        GameEvents.OnDialogTypewriterStartTypewriter += StartTypewriter;
        GameEvents.OnDialogTypewriterToggleVisibility += ToggleVisibility;
    }

    private void OnDisable() {
        GameEvents.OnDialogTypewriterStartTypewriter -= StartTypewriter;
        GameEvents.OnDialogTypewriterToggleVisibility -= ToggleVisibility;
    }


    private void StartTypewriter(string message, float duration)
    {
        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        typingCoroutine = StartCoroutine(TypeText(message, duration));
    }

    private IEnumerator TypeText(string message, float duration)
    {
        ToggleVisibility(true);
        textComponent.text = "";
        foreach (char c in message)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        yield return new WaitForSeconds(duration);
        textComponent.text = "";
        ToggleVisibility(false);
    }

    private void ToggleVisibility(bool val) {
        if (val == false) textComponent.text = ""; 
        transform.parent.GetComponent<Image>().enabled = val;
        transform.parent.GetComponent<Shadow>().enabled = val;
    }
    
    
    
}