using System;
using UnityEngine;

public class Night3SafeArea : MonoBehaviour {
   private bool isSafe = false;

   private void OnEnable() {
      GameQuery.OnGetNight3InSafeZone = IsSafe;
   }

   private void OnDisable() {
      GameQuery.OnGetNight3InSafeZone = null;
   }

   private void OnTriggerStay(Collider other) { 
      if (other.CompareTag("Player")) {
         isSafe = true;
      }
   }

   private void OnTriggerExit(Collider other) {
      if (other.CompareTag("Player")) {
         isSafe = false;
      }
   }


   private bool IsSafe() {
      return isSafe;
   }
   
}
