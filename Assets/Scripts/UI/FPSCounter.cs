using UnityEngine;
using TMPro; // Use UnityEngine.UI if you're using legacy Text

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText; // Assign in Inspector
    private float deltaTime;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        
    }

    
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; // Smoothing
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
       
    }
}