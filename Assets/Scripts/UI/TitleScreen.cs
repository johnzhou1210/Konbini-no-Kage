using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
   public void StartGame() {
      SceneManager.LoadScene(1);
   }

   public void QuitGame() {
      if (Application.platform != RuntimePlatform.WebGLPlayer) {
         Application.Quit();
      }
   }
   
}
