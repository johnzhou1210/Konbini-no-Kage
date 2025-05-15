using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using Random = UnityEngine.Random;

public class CustomerItem : MonoBehaviour, IInteractable
{
    public string PromptText { get; set; } = "Checkout items";

    [SerializeField, Self] private new AudioSource audio;
    [SerializeField, Self] private new Collider collider;
    
    private void OnValidate() {
        this.ValidateRefs();
    }

    private void OnEnable() {
        GameEvents.OnResetCustomerItem += Reset;
        GameEvents.OnCustomerItemEnableInteraction += EnableInteraction;
    }

    private void OnDisable() {
        GameEvents.OnResetCustomerItem -= Reset;
        GameEvents.OnCustomerItemEnableInteraction -= EnableInteraction;
    }


    public void Interact() {
        Debug.Log("Interacted with customer item");
        audio.pitch = Random.Range(0.8f, 1.2f);
        audio.Play();
        GetComponent<Renderer>().enabled = false;
        GameEvents.RaiseOnCounterItemDisplayClearItems();
        collider.enabled = false;
        GameEvents.RaiseOnLineupManagerAdvanceQueue();
    }

    public void EnableInteraction(List<KonbiniItem> items) {
        if ((GameQuery.OnGetLineupManagerQueue?.Invoke() ?? new Queue<CustomerBehavior>()).Count > 0) {
            collider.enabled = true;
            GameEvents.RaiseOnDisplayCustomerItems(items);
        }
    }

    private void Reset() {
        collider.enabled = false;
    }
    
    
    
}