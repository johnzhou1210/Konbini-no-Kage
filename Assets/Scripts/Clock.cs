using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    
    [SerializeField, Self] private TextMeshProUGUI timeText;

    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Start() {
        GameManager.OnTimeUpdate += UpdateTime;
    }

    private void OnDestroy() {
        GameManager.OnTimeUpdate -= UpdateTime;
    }

    private void UpdateTime(int newTime) {
        int minutes = newTime % 60;
        int hours = newTime / 60;
        timeText.text = $"REC {hours:00}:{minutes:00}";
    }
    
}
