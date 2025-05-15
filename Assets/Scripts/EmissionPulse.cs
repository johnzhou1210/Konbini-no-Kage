using UnityEngine;

public class EmissionPulse : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color baseEmissionColor = Color.white;
    [SerializeField] private float pulseSpeed = 2f; // Speed of pulsing
    [SerializeField] private float minIntensity = 1f;
    [SerializeField] private float maxIntensity = 5f;

    private Material materialInstance;
    private float timeOffset;

    private void Awake()
    {
        materialInstance = targetRenderer.material;
        timeOffset = Random.value * Mathf.PI * 2f; // desync multiple pulses
    }

    private void Update()
    {
        // Create a smooth pulse with sin
        float emissionIntensity = Mathf.Lerp(minIntensity, maxIntensity,
            (Mathf.Sin(Time.time * pulseSpeed + timeOffset) + 1f) / 2f);

        // Convert to gamma space for HDR
        Color finalColor = baseEmissionColor * Mathf.LinearToGammaSpace(emissionIntensity);

        materialInstance.SetColor("_EmissionColor", finalColor);
    }
}