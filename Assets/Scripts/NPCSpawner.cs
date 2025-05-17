using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
   [SerializeField] private List<GameObject> spawnableNPCs;
   [SerializeField] private Transform spawnPointsContainer;
   [SerializeField] private Transform npcContainer;
   [SerializeField] private GameObject deadlyStalkerPrefab, customerStalkerPrefab;


   private CustomerBehavior customerStalker;
   
   private List<Transform> spawnPoints;

   private void Start() {
      spawnPoints = new List<Transform>();
      foreach (Transform child in spawnPointsContainer) {
         spawnPoints.Add(child);
      }
   }

   private void OnEnable() {
      GameEvents.OnSpawnRandomNPC += SpawnRandomNPC;
      GameEvents.OnClearSpawnedNPCs += ClearSpawnedNPCs;
      GameEvents.OnSpawnDeadlyStalker += SpawnDeadlyStalker;
      GameEvents.OnSpawnStalkerCustomer += SpawnStalkerCustomer;
      GameEvents.OnNight4StalkerLeave += InitiateNight4StalkerLeave;
   }

   private void OnDisable() {
      GameEvents.OnSpawnRandomNPC -= SpawnRandomNPC;
      GameEvents.OnClearSpawnedNPCs -= ClearSpawnedNPCs;
      GameEvents.OnSpawnDeadlyStalker -= SpawnDeadlyStalker;
      GameEvents.OnSpawnStalkerCustomer -= SpawnStalkerCustomer;
      GameEvents.OnNight4StalkerLeave -= InitiateNight4StalkerLeave;
   }

   public void SpawnRandomNPC(HashSet<CustomerTendency> tendencies = null) {
      GameObject spawnedNPC = Instantiate(spawnableNPCs[Random.Range(0, spawnableNPCs.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
      spawnedNPC.transform.parent = npcContainer;
      if (tendencies == null) return;
      
      CustomerBehavior behavior = spawnedNPC.GetComponent<CustomerBehavior>();
      foreach (CustomerTendency tendency in tendencies) {
         behavior.AddCustomerTendency(tendency);
      }
      Debug.LogWarning("Spawned Abnormal "+string.Join(", ", tendencies.Select(t => t.ToString()))+" NPC named " + spawnedNPC.name + "!");
   }

   public void ClearSpawnedNPCs() {
      foreach (Transform child in npcContainer) {
         Destroy(child.gameObject);
      }
   }

   private void SpawnDeadlyStalker(Vector3 pos) {
      if (pos == Vector3.zero) {
         GameObject deadlyStalker = Instantiate(deadlyStalkerPrefab, spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
      } else {
         GameObject deadlyStalker = Instantiate(deadlyStalkerPrefab, pos, Quaternion.identity);
      }
      
      Debug.LogWarning("Spawned deadly stalker!");
   }

   private void SpawnStalkerCustomer() {
      GameObject customerStalkerGO = Instantiate(customerStalkerPrefab,
         spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
      customerStalker = customerStalkerGO.GetComponent<CustomerBehavior>();
      Debug.LogWarning("Spawned customer stalker!");
   }

   private void InitiateNight4StalkerLeave() {
      customerStalker.InitiateLeave();
   }
   
   
}
