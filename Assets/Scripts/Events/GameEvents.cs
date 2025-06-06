using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents {
   
   #region NPCs
   public static event Action OnClearSpawnedNPCs, OnSpawnStalkerCustomer;
   public static event Action<CustomerTendency> OnSetCustomerTendency;
   public static event Action<HashSet<CustomerTendency>> OnSpawnRandomNPC;
   public static void RaiseOnSpawnRandomNPC(HashSet<CustomerTendency> customerTendency = null) {
      OnSpawnRandomNPC?.Invoke(customerTendency);
   }

   public static void RaiseOnClearSpawnedNPCs() {
      OnClearSpawnedNPCs?.Invoke();
   }

   public static void RaiseOnSpawnStalkerCustomer() {
      OnSpawnStalkerCustomer?.Invoke();
   }

   public static void RaiseOnSetCustomerTendency(CustomerTendency newTendency) {
      OnSetCustomerTendency?.Invoke(newTendency);
   }
   #endregion
   
   #region Time
   public static event Action<int> OnSetDayOfMonth, OnNightUpdate, OnTimeUpdate;
   public static event Action OnTriggerEndShift;
   
   public static void RaiseOnTimeUpdate(int time) {
      OnTimeUpdate?.Invoke(time);
   }

   public static void RaiseOnNightUpdate(int night) {
      OnNightUpdate?.Invoke(night);
   }
   public static void RaiseOnSetDayOfMonth(int dayOfMonth) {
      OnSetDayOfMonth?.Invoke(dayOfMonth);
   }

   public static void RaiseOnTriggerEndShift() {
      OnTriggerEndShift?.Invoke();
   }
   
   #endregion

   #region SecurityCameras
   public static event Action<int> OnSetActiveCamera;
   public static event Action OnExitSecurityCamera, OnGenerateRandomizedStalkerTimeSlots, OnStartWhisperCoroutine;
   public static event Action<int, CameraStatus> OnSecurityCamerasSetCameraStatus;
   public static event Action<bool> OnSecurityCamerasSetBrainInstantSwitch;
   
   public static event Action OnStalkerJumpscare;

   public static void RaiseOnStartWhisperCoroutine() {
      OnStartWhisperCoroutine?.Invoke();
   }
   
   public static void RaiseOnSecurityCamerasSetBrainInstantSwitch(bool state) {
      OnSecurityCamerasSetBrainInstantSwitch?.Invoke(state);
   }
   
   public static void RaiseOnStalkerJumpscare() {
      OnStalkerJumpscare?.Invoke();
   }

   public static void RaiseOnSecurityCamerasSetCameraStatus(int index, CameraStatus status) {
      OnSecurityCamerasSetCameraStatus?.Invoke(index, status);
   }

   public static void RaiseOnSetActiveCamera(int cameraIndx) {
      OnSetActiveCamera?.Invoke(cameraIndx);
   }

   public static void RaiseOnExitSecurityCamera() {
      OnExitSecurityCamera?.Invoke();
   }

   public static void RaiseOnGenerateRandomizedStalkerTimeSlots() {
      OnGenerateRandomizedStalkerTimeSlots?.Invoke();
   }
   #endregion

   #region CounterItemDisplay
   public static event Action OnCounterItemDisplayClearItems;
   public static event Action<List<KonbiniItem>> OnDisplayCustomerItems;
   
   public static void RaiseOnCounterItemDisplayClearItems() {
      OnCounterItemDisplayClearItems?.Invoke();
   }

   public static void RaiseOnDisplayCustomerItems(List<KonbiniItem> items) {
      OnDisplayCustomerItems?.Invoke(items);
   }
   #endregion

   #region CustomerItem
   public static event Action OnResetCustomerItem;
   public static event Action<List<KonbiniItem>> OnCustomerItemEnableInteraction;

   public static void RaiseOnResetCustomerItem() {
      OnResetCustomerItem?.Invoke();
   }

   public static void RaiseOnCustomerItemEnableInteraction(List<KonbiniItem> items) {
      OnCustomerItemEnableInteraction?.Invoke(items);
   }
   
   #endregion

   
   #region LineupManager

   public static event Action OnLineupManagerAdvanceQueue, OnLineupManagerClearItems;
   public static event Action<CustomerBehavior> OnLineupManagerLineUpCustomer;
   
   public static void RaiseOnLineupManagerAdvanceQueue() {
      OnLineupManagerAdvanceQueue?.Invoke();
   }

   public static void RaiseOnLineupManagerClearItems() {
      OnLineupManagerClearItems?.Invoke();
   }

   public static void RaiseOnLineupManagerLineUpCustomer(CustomerBehavior customer) {
      OnLineupManagerLineUpCustomer?.Invoke(customer);
   }

   #endregion

   
   #region DialogTypewriter
   public static event Action<string, float> OnDialogTypewriterStartTypewriter;
   public static event Action<bool> OnDialogTypewriterToggleVisibility;

   public static void RaiseOnDialogTypewriterStartTypewriter(string message, float duration = 3f) {
      OnDialogTypewriterStartTypewriter?.Invoke(message, duration);
   }

   public static void RaiseOnDialogTypewriterToggleVisibility(bool visibility) {
      OnDialogTypewriterToggleVisibility?.Invoke(visibility);
   }
   
   #endregion
   
   #region EndShiftTrigger

   public static event Action<bool> OnEndShiftTriggerSetColliderEnabled;

   public static void RaiseOnEndShiftTriggerSetColliderEnabled(bool val) {
      OnEndShiftTriggerSetColliderEnabled?.Invoke(val);
   }

   #endregion

   #region BreakerBox

   public static event Action<bool> OnSetBreakerBoxDoorOpened, OnBreakerBoxTogglePower;
   public static event Action OnBreakerBoxCallFlickerThenExtinguish;
   public static event Action<int> OnBreakerBoxCallFlicker;

   public static void RaiseOnSetBreakerBoxDoorOpened(bool val) {
      OnSetBreakerBoxDoorOpened?.Invoke(val);
   }

   public static void RaiseOnBreakerBoxTogglePower(bool val) {
      OnBreakerBoxTogglePower?.Invoke(val);
   }

   public static void RaiseOnBreakerBoxCallFlickerThenExtinguish() {
      OnBreakerBoxCallFlickerThenExtinguish?.Invoke();
   }

   public static void RaiseOnBreakerBoxCallFlicker(int flickers) {
      OnBreakerBoxCallFlicker?.Invoke(flickers);
   }
   
   #region GateDoor

   public static event Action<bool> OnSetGateDoorOpened;

   public static void RaiseOnSetGateDoorOpened(bool val) { 
      OnSetGateDoorOpened?.Invoke(val);
   }

   #endregion


   #endregion

   #region KonbiniDoor

   public static event Action OnKonbiniDoorOpened;

   public static void RaiseOnKonbiniDoorOpened() {
      OnKonbiniDoorOpened?.Invoke();
   }

   #endregion
   
   
   #region PlayerInput

   public static event Action OnKillPlayerInput;

   public static void RaiseOnKillPlayerInput() {
      OnKillPlayerInput?.Invoke();
   }

   #endregion

   
   #region Cinematics

   public static event Action OnPlayerFlashRed;

   public static void RaiseOnPlayerFlashRed() {
      OnPlayerFlashRed?.Invoke();
   }

   #endregion
   
   #region StalkerBehavior

   public static event Action OnStalkerEndChase, OnGameOverShow, OnNight4StalkerLeave;
   public static event Action<Vector3> OnSpawnDeadlyStalker;
   public static event Action<bool> OnSetNight3ChaseIntoPlayerInput;

   public static void RaiseOnStalkerEndChase() {
      OnStalkerEndChase?.Invoke();
   }

   public static void RaiseSetNight3ChaseIntoPlayerInput(bool val) {
      OnSetNight3ChaseIntoPlayerInput?.Invoke(val);
   }

   public static void RaiseOnGameOverShow() {
      OnGameOverShow?.Invoke();
   }

   public static void RaiseOnSpawnDeadlyStalker(Vector3 pos) {
      OnSpawnDeadlyStalker?.Invoke(pos);
   }

   public static void RaiseOnNight4StalkerLeave() {
      OnNight4StalkerLeave?.Invoke();
   }
   
   #endregion
   
   #region PoliceCar

   public static event Action OnPoliceCarEnter;

   public static void RaiseOnPoliceCarEnter() {
      OnPoliceCarEnter?.Invoke();
   }

   #endregion

   #region WinScreen

   public static event Action OnShowWinScreen;

   public static void RaiseOnShowWinScreen() {
      OnShowWinScreen?.Invoke();
   }

   #endregion

}
