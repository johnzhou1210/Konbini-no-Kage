using System;
using System.Collections;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class BreakerBox : MonoBehaviour, IInteractable
{
    public string PromptText { get; set; } = "Interact with breaker box";
    [SerializeField] private GameObject[] onSwitches, offSwitches;
    [SerializeField, Self] private AudioSource audioSource;
    [SerializeField] private AudioSource closeSound, breakSound;

    [SerializeField] private GameObject[] outageGOs, nonOutageGOs; 
    
    private bool lightsOn = true;
    private Coroutine flickerCoroutine;

    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnEnable() {
        GameEvents.OnBreakerBoxTogglePower += SetLightsEnabled;
        GameEvents.OnBreakerBoxCallFlickerThenExtinguish += CallFlickerThenExtinguish;
        GameEvents.OnBreakerBoxCallFlicker += CallFlicker;
    }

    private void OnDisable() {
        GameEvents.OnBreakerBoxTogglePower -= SetLightsEnabled;
        GameEvents.OnBreakerBoxCallFlickerThenExtinguish -= CallFlickerThenExtinguish;
        GameEvents.OnBreakerBoxCallFlicker -= CallFlicker;
        if (flickerCoroutine != null) {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
    }

    private void Start() {
        PromptText = lightsOn ? "Turn off power" : "Turn on power";
    }

    public void Interact() {
        Debug.Log("Interacted with breaker box");
        GameEvents.RaiseOnBreakerBoxTogglePower(!lightsOn);
        
    }


    private IEnumerator FlickerThenExtinguish() {
        // remove player from security camera if they are currently viewing them when an outage occurs
        SetAllLightsActive(false);
        yield return new WaitForSeconds(.25f);
        SetAllLightsActive(true);
        yield return new WaitForSeconds(.4f);
        SetAllLightsActive(false);
        yield return new WaitForSeconds(.2f);
        SetAllLightsActive(true);
        yield return new WaitForSeconds(.1f);
        SetLightsEnabled(false);
        breakSound.Play();
        yield return null;
    }
    
    private IEnumerator Flicker(int flickers) {
        for (int i = 0; i < flickers; i++) {
            SetAllLightsActive(false);
            yield return new WaitForSeconds(Random.Range(0.1f,0.5f));
            SetAllLightsActive(true);
            yield return new WaitForSeconds(Random.Range(0.1f,0.5f));
        }
        SetAllLightsActive(lightsOn);
    }
    

    private void SetLightsEnabled(bool val) {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
        
        Debug.Log("Lights on is " + val);
        lightsOn = val;
        PromptText = lightsOn ? "Turn off power" : "Turn on power";
        foreach (GameObject onSwitch in onSwitches) {
            onSwitch.SetActive(val);
        }
        foreach (GameObject offSwitch in offSwitches) {
            offSwitch.SetActive(!val);
        }
        
        // remove player from security camera if they are currently viewing them when an outage occurs
        if (!lightsOn) GameEvents.RaiseOnExitSecurityCamera();
        
        SetAllLightsActive(lightsOn);
    }

    private void CallFlickerThenExtinguish() {
        flickerCoroutine = StartCoroutine(FlickerThenExtinguish());
    }

    private void CallFlicker(int flickers) {
        if (flickerCoroutine != null) {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        flickerCoroutine = StartCoroutine(Flicker(flickers));
    }
    
    private void SetAllLightsActive(bool val) {
        foreach (GameObject outageGO in outageGOs) {
            outageGO.SetActive(!val);
        }

        foreach (GameObject nonOutageGO in nonOutageGOs) {
            nonOutageGO.SetActive(val);
        }
        
    }
    
    
    
    
    
    public void PlayDoorCloseSound() {
        closeSound.pitch = Random.Range(0.8f, 1.2f);
        closeSound.Play();
    }
    
}