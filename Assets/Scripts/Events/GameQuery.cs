using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameQuery {
   
   #region Time

   public static Func<int> OnGetCurrentDayOfMonth, OnGetCurrentNight, OnGetMinutesAfterMidnight;
   #endregion
   
   #region SecurityCamera
   public static Func<int> OnGetCurrentSecurityCameraIndex;
   public static Func<bool> OnGetIsCheckingCameras;
   #endregion

   #region CounterItemDisplay

   public static Func<List<KonbiniItem>> OnGetCounterItemDisplayAllItems;
   public static Func<int, KonbiniItem> OnGetCounterItemDisplayGetItemByIndex;

   #endregion

   #region LineupManager

   public static Func<Queue<CustomerBehavior>> OnGetLineupManagerQueue;


   #endregion

   
   #region Night3SafeZone

   public static Func<bool> OnGetNight3InSafeZone;
   public static Func<bool> OnGetIsGateOpened;
   public static Func<bool> OnGetStalkerAngryCutsceneInProgress;
   public static Func<bool> OnGetIsPlayerCaught;

   #endregion

   #region Night4StalkerInStoreTrigger
   public static Func<bool> OnGetNight4InStoreTrigger;
   
   #endregion


}
