using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneData
{
   public static int highestNightNumber = 1;
   public static int startingNightNumber = 1;
}


public class GameOverScreen : MonoBehaviour
{


   private void SaveHighestNightNumber() {
      if ((GameQuery.OnGetCurrentNight?.Invoke() ?? 1) > SceneData.highestNightNumber) {
         SceneData.highestNightNumber = GameQuery.OnGetCurrentNight?.Invoke() ?? 1;
      }
   }

   public void RetryNight() {
      SaveHighestNightNumber();
      SceneManager.LoadScene(1);
   }

   public void MainMenu() {
      SaveHighestNightNumber();
      SceneManager.LoadScene(0);
   }
   
}
