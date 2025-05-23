using System;
using UnityEngine;

public class HideIfStalkerIsOutside : MonoBehaviour {
    [SerializeField] private GameObject behindPlayer;
    [SerializeField] private GameObject scheming;

    private void Update() {
        behindPlayer.SetActive(GameQuery.OnGetNight4InStoreTrigger?.Invoke() ?? false);
        if ((GameQuery.OnGetMinutesAfterMidnight?.Invoke() ?? 0) > TimeUtils.ConvertToMinutesAfterMidnight(4, 0) &&
            (GameQuery.OnGetMinutesAfterMidnight?.Invoke() ?? 0) < TimeUtils.ConvertToMinutesAfterMidnight(6, 0)) {
            if ((GameQuery.OnGetCurrentNight?.Invoke() ?? 1) == 4 && !(GameQuery.OnGetNight4InStoreTrigger?.Invoke() ?? false)) {
                scheming.SetActive(true);
            }
        }
    }
}
