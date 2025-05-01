using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public int MinutesAfterMidnight = 0;
    public static event Action<int> OnTimeUpdate, OnNightUpdate;

    [SerializeField] public Dictionary<int,int> MAMShiftStart, MAMShiftEnd;
    

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
    }

    private void Start() {
        StartCoroutine(DayNightCycle());
    }

    private IEnumerator DayNightCycle() {
        OnNightUpdate?.Invoke(currNight);
        MinutesAfterMidnight = MAMShiftStart[currNight];
        print("in here");
        while (MinutesAfterMidnight != MAMShiftEnd[currNight]) {
            yield return new WaitForSeconds(.05f);
            MinutesAfterMidnight++;
            OnTimeUpdate?.Invoke(MinutesAfterMidnight);
            if (MinutesAfterMidnight == 60 * 24) MinutesAfterMidnight = 0;
        }

        if (currNight == 4) yield break;
        currNight++;
        OnNightUpdate?.Invoke(currNight);
        StartCoroutine(DayNightCycle());
    }
    
}
