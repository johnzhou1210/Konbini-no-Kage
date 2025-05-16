using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHotkey : MonoBehaviour
{
    private void SaveHighestNightNumber() {
        if ((GameQuery.OnGetCurrentNight?.Invoke() ?? 1) > SceneData.highestNightNumber) {
            SceneData.highestNightNumber = GameQuery.OnGetCurrentNight?.Invoke() ?? 1;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) {
            Cursor.lockState = CursorLockMode.None;
            SaveHighestNightNumber();
            SceneManager.LoadScene(0);
        }
    }
}
