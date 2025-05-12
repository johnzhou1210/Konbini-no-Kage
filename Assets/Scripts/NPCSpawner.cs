using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
   public static NPCSpawner Instance;
   [SerializeField] private List<GameObject> spawnableNPCs;
   [SerializeField] private Transform spawnPointsContainer;
   [SerializeField] private Transform npcContainer;

   
   private List<Transform> spawnPoints;
   
   private void Awake() {
      if (Instance == null) {
         Instance = this;
      } else {
         Destroy(gameObject);
      }
   }

   private void Start() {
      spawnPoints = new List<Transform>();
      foreach (Transform child in spawnPointsContainer) {
         spawnPoints.Add(child);
      }
   }

   public void SpawnRandomNPC() {
      GameObject spawnedNPC = Instantiate(spawnableNPCs[Random.Range(0, spawnableNPCs.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
      spawnedNPC.transform.parent = npcContainer;
   }

   public void ClearSpawnedNPCs() {
      foreach (Transform child in npcContainer) {
         Destroy(child.gameObject);
      }
   }
   
   
}
