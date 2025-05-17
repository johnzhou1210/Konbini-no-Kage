using System;
using UnityEngine;

public class Night4StalkerInStoreTrigger : MonoBehaviour {
   private bool inStore = false;

   private void OnEnable() {
      GameQuery.OnGetNight4InStoreTrigger = InStore;
   }

   private void OnDisable() {
      GameQuery.OnGetNight4InStoreTrigger = null;
   }

   private void OnTriggerStay(Collider other) { 
      if (other.name.Contains("STALKER")) {
         inStore = true;
      }
   }

   private void OnTriggerExit(Collider other) {
      if (other.name.Contains("STALKER")) {
         inStore = false;
      }
   }


   private bool InStore() {
      return inStore;
   }
   
}
