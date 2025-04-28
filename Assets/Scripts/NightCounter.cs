using System;
using KBCore.Refs;
using TMPro;
using UnityEngine;

public class NightCounter : MonoBehaviour {
    [SerializeField, Self] private TextMeshProUGUI nightCounter;
    
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Start() {
        GameManager.OnNightUpdate += UpdateNightCounter;
    }

    private void UpdateNightCounter(int night) {
        nightCounter.text = $"<size=64>第{ConvToKanji(night)}夜</size>\n      {ConvToEnglish(night)} night";
    }

    private string ConvToKanji(int num) {
        switch (num) {
            case 1:
                return "一";
            case 2:
                return "二";
            case 3:
                return "三";
            case 4:
                return "<color=red>死</color>";
            default:
                return "";
        }
    }

    private string ConvToEnglish(int num) {
        switch (num) {
            case 1:
                return "first";
            case 2:
                return "second";
            case 3:
                return "third";
            case 4:
                return "<color=red>final</color>";
            default:
                return "";
        }
    }
    
}
