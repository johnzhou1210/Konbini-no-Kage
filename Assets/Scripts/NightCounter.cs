using System;
using System.Collections;
using KBCore.Refs;
using TMPro;
using UnityEngine;

public class NightCounter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI nightCounterKanji, nightCounterEnglish;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    

    private void OnEnable() {
        GameEvents.OnNightUpdate += UpdateNightCounter;
    }

    private void Start() {
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();
    }


    private void OnDisable() {
        GameEvents.OnNightUpdate -= UpdateNightCounter;
    }
    

    private void UpdateNightCounter(int night) {
        
        animator.Play("NightCounterFadeIn");
        nightCounterKanji.text = $"<size=128>第{ConvToKanji(night)}夜</size>";
        nightCounterEnglish.text = $"{ConvToEnglish(night)} night";
        nightCounterKanji.gameObject.GetComponent<TextJitter>().jitterAmount = 4f;
        nightCounterEnglish.gameObject.GetComponent<TextJitter>().jitterAmount = 2f;
        if (night == 4) {
            Invoke(nameof(DoPun), 4f);
        }
        
    }

    private void DoPun() {
        nightCounterKanji.text = $"<size=128>第<color=#A1011A>死</color>夜</size>";
        nightCounterEnglish.text = $"<color=#A1011A>final</color> night";
        nightCounterKanji.gameObject.GetComponent<TextJitter>().jitterAmount = 0f;
        nightCounterEnglish.gameObject.GetComponent<TextJitter>().jitterAmount = 0f;
        audioSource.Play();
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
                return "四";
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
                return "fourth";
            default:
                return "";
        }
    }
    
}
