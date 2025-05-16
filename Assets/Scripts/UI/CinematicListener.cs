using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

public class CinematicListener : MonoBehaviour
{
    [SerializeField, Self] private Animator animator;
    private Coroutine flashRoutine;
    [SerializeField] private Image flashImage;
    [SerializeField] private Color flashColor = new Color(1f, 0f, 0f, 0.6f);
    [SerializeField] private GameObject gameOverPanel, gameManager;
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnEnable() {
        GameEvents.OnNightUpdate += PlayCinematicAnimation;
        GameEvents.OnPlayerFlashRed += PlayerFlashRed;
        GameEvents.OnGameOverShow += InvokeShowGameOver;
    }

    private void OnDisable() {
        GameEvents.OnNightUpdate -= PlayCinematicAnimation;
        GameEvents.OnPlayerFlashRed -= PlayerFlashRed;
        GameEvents.OnGameOverShow -= InvokeShowGameOver;
    }

    private void PlayCinematicAnimation(int night) {
        animator.Play("CinematicIntro");
    }

    private void PlayerFlashRed() {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine() {
        float flashDuration = 2f;
        float elapsed = 0f;
        Color red = new Color(1f, 0f, 0f, 0.6f);
        Color black = new Color(0f, 0f, 0f, 1f);

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flashDuration;
            flashImage.color = Color.Lerp(red, black, t);
            yield return null;
        }

        flashImage.color = black;
    }

    private void InvokeShowGameOver() {
        Invoke(nameof(ShowGameOver), 8f);
    }
    
    private void ShowGameOver() {
        GameEvents.RaiseOnClearSpawnedNPCs();
        gameManager.SetActive(false);
        flashImage.color = Color.clear;
        gameOverPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

}
