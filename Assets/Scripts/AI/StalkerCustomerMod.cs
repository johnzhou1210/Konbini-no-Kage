using System;
using UnityEngine;

public class StalkerCustomerMod : MonoBehaviour {
  [SerializeField] private CustomerBehavior customerBehavior;
  
  void SetLayerRecursively(GameObject obj, int newLayer)
  {
    obj.layer = newLayer;
    foreach (Transform child in obj.transform)
    {
      SetLayerRecursively(child.gameObject, newLayer);
    }
  }
  
  private void Update() {
    if (GameQuery.OnGetNight4InStoreTrigger?.Invoke() ?? false) {
      if (GameQuery.OnGetIsCheckingCameras?.Invoke() ?? false) {
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Night4Stalker"));
      } else {
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("NPC"));
      }
    } else {
      SetLayerRecursively(gameObject, LayerMask.NameToLayer("NPC"));
    }
  }
}
