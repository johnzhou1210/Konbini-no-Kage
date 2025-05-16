using System;
using KBCore.Refs;
using UnityEngine;

public class KnifeStab : MonoBehaviour {
   [SerializeField, Self] private AudioSource stabSound;
   
   private void OnValidate() {
      this.ValidateRefs();
   }

   public void PlayKnifeStab() {
      stabSound.Play();
   }

   public void PlayBodyDrop() {
      transform.Find("BodyDrop").GetComponent<AudioSource>().Play();
   }

   public void PlayJumpScareSFX() {
      transform.Find("JumpScareSFX").GetComponent<AudioSource>().Play();
      Invoke(nameof(PlayJumpScarePostSFX), 8f);
   }

   private void PlayJumpScarePostSFX() {
      transform.Find("JumpScarePostSFX").GetComponent<AudioSource>().Play();
   }

   public void FlashRed() {
      GameEvents.RaiseOnPlayerFlashRed();
   }
   
}
