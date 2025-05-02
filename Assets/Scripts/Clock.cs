using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    
    [SerializeField, Self] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

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
        timeText.text = $"{hours:00}:{minutes:00}";

        if (newTime == 0) {
            print("CHANGED DAY");
            GameManager.Instance.CurrDayOfMonth++;
            dateText.text = $"[1999/08/{GameManager.Instance.CurrDayOfMonth:00}]";
        }
    }

    private void UpdateDate(int currNight) {
        
    }
    
}
