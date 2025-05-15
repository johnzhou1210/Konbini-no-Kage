using System;
using UnityEngine;

public class EndShiftTrigger : MonoBehaviour
{
    private void OnEnable() {
        GameEvents.OnEndShiftTriggerSetColliderEnabled += SetColliderEnabled;
    }

    private void OnDisable() {
        GameEvents.OnEndShiftTriggerSetColliderEnabled -= SetColliderEnabled;
    }

    private void OnTriggerEnter(Collider other) {
        GameEvents.RaiseOnTriggerEndShift();
        SetColliderEnabled(false);
    }

    private void SetColliderEnabled(bool val) {
        GetComponent<Renderer>().enabled = val;
        GetComponent<Collider>().enabled = val;
        GetComponent<Light>().enabled = val;
    }
    
}
