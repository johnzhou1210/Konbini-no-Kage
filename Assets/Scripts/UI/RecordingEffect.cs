using System;
using System.Collections;
using KBCore.Refs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecordingEffect : MonoBehaviour
{
    [SerializeField, Self] private TextMeshProUGUI text;

    private Coroutine coro;
    
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnEnable() {
        StartCoroutine(RecordingEffectCoro());
    }

    private IEnumerator RecordingEffectCoro() {
        while (true) {
            text.text = "[REC] <color=#a1011a>\u25cf</color>";
            yield return new WaitForSeconds(1f);
            text.text = "[REC]";
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnDisable() {
        if (coro != null) {
            StopCoroutine(coro);
            coro = null;
        }
    }
}
