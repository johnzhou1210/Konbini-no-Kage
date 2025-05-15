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
        GameEvents.OnTimeUpdate += UpdateTime;
    }

    private void OnDestroy() {
        GameEvents.OnTimeUpdate -= UpdateTime;
    }

    private void UpdateTime(int newTime) {
        print(newTime);
        int minutes = newTime % 60;
        int hours = newTime / 60;
        timeText.text = $"{hours:00}:{minutes:00}";
        
        int currDayOfMonth = GameQuery.OnGetCurrentDayOfMonth?.Invoke() ?? 0;
        dateText.text = $"[1999/08/{currDayOfMonth :00}]";
        
    }

    private void UpdateDate(int currNight) {
        
    }
    
}
