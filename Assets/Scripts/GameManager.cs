using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public int MinutesAfterMidnight = 0;
    public int CurrDayOfMonth = 9;
    
    public static event Action<int> OnTimeUpdate, OnNightUpdate;
    
    

    [SerializeField] public Dictionary<int,int> MAMShiftStart, MAMShiftEnd;
    
    private Dictionary<int,int> maxNPCsPerNight;


    private float spawnChance;
    private int currNight = 1;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        MAMShiftStart = new Dictionary<int, int>();
        MAMShiftEnd = new Dictionary<int, int>();
        MAMShiftStart[1] = (60 * 22) + 0; MAMShiftStart[2] = (60 * 22) + 0; MAMShiftStart[3] = (60 * 22) + 0; MAMShiftStart[4] = (60 * 22) + 0;
        MAMShiftEnd[1] = (60 * 6) + 0; MAMShiftEnd[2] = (60 * 6) + 0; MAMShiftEnd[3] = (60 * 6) + 0; MAMShiftEnd[4] = (60 * 6) + 0;
        maxNPCsPerNight = new Dictionary<int, int>();
        maxNPCsPerNight[1] = 5;
        maxNPCsPerNight[2] = 9;
        maxNPCsPerNight[3] = 13;
        maxNPCsPerNight[4] = 19;
    }
    


    private void Start() {
        StartCoroutine(DayNightCycle());
    }

    private IEnumerator DayNightCycle() {
        OnNightUpdate?.Invoke(currNight);
        MinutesAfterMidnight = MAMShiftStart[currNight];
        print("in here");
        int remainingNPCs = maxNPCsPerNight[currNight];
        while (MinutesAfterMidnight != MAMShiftEnd[currNight]) {
            yield return new WaitForSeconds(.25f);

            if (remainingNPCs > 0) {
                int totalMinutesInDay = 1440;
                int remainingTime = (MAMShiftEnd[currNight] - MinutesAfterMidnight + totalMinutesInDay) % totalMinutesInDay;
                spawnChance = (float)remainingNPCs / (float)remainingTime;
                if (Random.value < spawnChance) {
                    NPCSpawner.Instance.SpawnRandomNPC();
                    remainingNPCs--;
                }
            }
            
            MinutesAfterMidnight++;
            if (MinutesAfterMidnight == 60 * 24) MinutesAfterMidnight = 0;
            OnTimeUpdate?.Invoke(MinutesAfterMidnight);
        }

        if (currNight == 4) yield break;
        currNight++;
        NPCSpawner.Instance.ClearSpawnedNPCs();
        CounterItemDisplayer.Instance.ClearItems();
        LineupManager.Instance.ClearItems();
        OnNightUpdate?.Invoke(currNight);
        StartCoroutine(DayNightCycle());
    }

    public void ResetGame() {
        currNight = 1;
        CurrDayOfMonth = 9;
        MinutesAfterMidnight = 0;
    }
    
}
