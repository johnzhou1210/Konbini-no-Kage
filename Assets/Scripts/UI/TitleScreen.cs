using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {
   [SerializeField] private GameObject startButton, continueButton, optionsButton, quitButton;
   [SerializeField] private GameObject mainButtons, nightButtons;

   [SerializeField] private GameObject[] individualNightButtons;
   
   public void StartGame() {
      SceneManager.LoadScene(1);
   }

   public void QuitGame() {
      if (Application.platform != RuntimePlatform.WebGLPlayer) {
         Application.Quit();
      }
   }

   public void OnContinuePress() {
      mainButtons.SetActive(false);
      nightButtons.SetActive(true);
      
      // Show only needed nights
      foreach (GameObject button in individualNightButtons) {
         button.SetActive(false);
      }
      
      for (int i = 0; i < SceneData.highestNightNumber; i++) {
         individualNightButtons[i].SetActive(true);
      }
      
   }

   public void OnBackButtonPress() {
      nightButtons.SetActive(false);
      mainButtons.SetActive(true);
   }

   public void StartNight1() {
      SceneData.startingNightNumber = 1;
      StartGame();
   }
   public void StartNight2() {
      SceneData.startingNightNumber = 2;
      StartGame();
   }
   public void StartNight3() {
      SceneData.startingNightNumber = 3;
      StartGame();
   }
   public void StartNight4() {
      SceneData.startingNightNumber = 4;
      StartGame();
   }
   
   private void Update() {
      if (SceneData.highestNightNumber == 1) {
         continueButton.GetComponent<Button>().interactable = false;
      }
   }
}
