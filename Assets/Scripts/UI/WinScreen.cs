using System;
using UnityEngine;
using UnityEngine.SceneManagement;



public class WinScreen : MonoBehaviour
{
   private void Start() {
      GetComponent<AudioSource>().Play();
      Cursor.lockState = CursorLockMode.None;
   }

   private void SaveHighestNightNumber() {
      if ((GameQuery.OnGetCurrentNight?.Invoke() ?? 1) > SceneData.highestNightNumber) {
         SceneData.highestNightNumber = GameQuery.OnGetCurrentNight?.Invoke() ?? 1;
      }
   }
   public void MainMenu() {
      SaveHighestNightNumber();
      SceneManager.LoadScene(0);
   }
   
}
