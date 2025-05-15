using System;
using UnityEngine;
using System.Collections.Generic;
using KBCore.Refs;
using Random = UnityEngine.Random;

public class CounterItemDisplayer : MonoBehaviour {
    [SerializeField, Self] private Renderer rend;
    
    [Header("Spawn Settings")]
    public Transform spawnArea;
    public GameObject itemQuadPrefab;

    [Header("Konbini Items")]
    public List<KonbiniItem> AllItems; // List of item definitions

    [Header("Item Arrangement")]
    public int maxItems = 3;
    public float spacing = 0.25f;

    [SerializeField] private GameObject moneySetups;
    
    private List<GameObject> moneySetupsList;

    private List<GameObject> spawnedItems = new List<GameObject>();

    private void OnValidate() {
        this.ValidateRefs();
    }


    private void OnEnable() {
        GameEvents.OnCounterItemDisplayClearItems += ClearItems;
        GameEvents.OnDisplayCustomerItems += DisplayCustomerItems;
        
        GameQuery.OnGetCounterItemDisplayAllItems = () => AllItems;
        GameQuery.OnGetCounterItemDisplayGetItemByIndex = GetItemByIndex;
    }

    private void OnDisable() {
        GameEvents.OnCounterItemDisplayClearItems -= ClearItems;
        GameEvents.OnDisplayCustomerItems -= DisplayCustomerItems;
        
        GameQuery.OnGetCounterItemDisplayAllItems = null;
        GameQuery.OnGetCounterItemDisplayGetItemByIndex = null;
    }

    public void DisplayCustomerItems(List<KonbiniItem> customerItems)
    {
        ClearItems();

        moneySetupsList[Random.Range(0, moneySetupsList.Count)].SetActive(true);
        
        for (int i = 0; i < customerItems.Count; i++)
        {
            Vector3 offset = new Vector3(Random.Range(-spacing, spacing), 0, Random.Range(-spacing, spacing));
            Vector3 spawnPos = spawnArea.position + offset;

            KonbiniItem itemData = customerItems[i];

            GameObject quad = Instantiate(itemQuadPrefab, spawnPos, Quaternion.Euler(90f, Random.Range(0f, 360f), Random.Range(0f, 360f)), spawnArea);
            quad.GetComponent<Renderer>().material = itemData.itemMaterial;

            quad.name = itemData.itemName;
            spawnedItems.Add(quad);

            Debug.Log($"Spawned Item: {itemData.itemName} - ¥{itemData.price}");
        }

        rend.enabled = true;
    }
    
    public void DisplayCustomerItems()
    {
        // fallback random spawn
        List<KonbiniItem> randomItems = new List<KonbiniItem>();
        for (int i = 0; i < maxItems; i++)
        {
            randomItems.Add(AllItems[Random.Range(0, AllItems.Count)]);
        }
        DisplayCustomerItems(randomItems);
    }


    private void Start() {
        
        moneySetupsList = new List<GameObject>();
        foreach (Transform child in moneySetups.transform) {
            moneySetupsList.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
        // DisplayCustomerItems();
    }

    public void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            Destroy(item);
        }

        foreach (GameObject item in moneySetupsList) {
            item.SetActive(false);
        }
        spawnedItems.Clear();
        rend.enabled = false;
    }

    private KonbiniItem GetItemByIndex(int index) {
        return AllItems[index];
    }
    
}