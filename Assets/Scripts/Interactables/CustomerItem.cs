using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerItem : MonoBehaviour, IInteractable
{
    public string PromptText { get; set; } = "Checkout items";
    public static CustomerItem Instance;

    [SerializeField, Self] private new AudioSource audio;
    [SerializeField, Self] private new Collider collider;
    
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void Interact() {
        Debug.Log("Interacted with customer item");
        audio.pitch = Random.Range(0.8f, 1.2f);
        audio.Play();
        GetComponent<Renderer>().enabled = false;
        CounterItemDisplayer.Instance.ClearItems();
        collider.enabled = false;
        LineupManager.Instance.AdvanceQueue();
    }

    public void EnableInteraction(List<KonbiniItem> items) {
        if (LineupManager.Instance.GetLength() > 0) {
            collider.enabled = true;
            CounterItemDisplayer.Instance.DisplayCustomerItems(items);
        }
    }
    
    
    
}