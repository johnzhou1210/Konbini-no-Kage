using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

public class RecordingEffect : MonoBehaviour
{
    [SerializeField, Child] private Image image;
    
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Start() {
        StartCoroutine(RecordingEffectCoro());
    }

    private IEnumerator RecordingEffectCoro() {
        while (true) {
            image.enabled = true;
            yield return new WaitForSeconds(1f);
            image.enabled = false;
            yield return new WaitForSeconds(1f);
        }
    }
    
}
