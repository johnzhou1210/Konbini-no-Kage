using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class StalkerBehavior : MonoBehaviour {
    [SerializeField, Self] private Animator animator;

    public Transform target; // Assign this in the Inspector
    private NavMeshAgent agent;
    [SerializeField] private float minWait = 3f, maxWait = 8f;
    [SerializeField] private float targetStopRadius = 1.5f;
    [SerializeField] private GameObject plasticBag, knife;
    
    [SerializeField] private List<KonbiniItem> customItems;

    [SerializeField] private AudioSource chaseSound;

    [SerializeField] private GameObject normalFace, grinFace, jumpscareFace, angryFace;

    [SerializeField] private GameObject jumpscareCamObj;
    [SerializeField] private float chaseDistance = 8f;
    


    public bool ReachedFront = false;
    private bool caughtPlayer = false;
    private bool angryCutsceneInProgress = false;

    private List<Transform> stalkerWanderWaypoints, vanishingPoints;

    private Coroutine wanderCoroutine;
    private Coroutine leaveCoroutine;
    private Coroutine chaseCoroutine;
    
    private bool weirdStare = false;

    private Vector3 targetPos;

    private GameObject player;

    private bool chasing = false;
    
    private void OnValidate() { this.ValidateRefs(); }


    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        vanishingPoints = new List<Transform>();
        stalkerWanderWaypoints = new List<Transform>();
     
    }

    private void OnEnable() {
        GameEvents.OnStalkerEndChase += StopChase;

        GameQuery.OnGetStalkerAngryCutsceneInProgress = () => angryCutsceneInProgress;
    }

    private void OnDisable() {
        GameEvents.OnStalkerEndChase -= StopChase;

        GameQuery.OnGetStalkerAngryCutsceneInProgress = null;
    }


    private void Start() {
    
        foreach (Transform child in GameObject.FindWithTag("NPCSpawnPoints").transform) {
            vanishingPoints.Add(child);
        }

        foreach (Transform child in GameObject.FindWithTag("StalkerWanderPoints").transform) {
            stalkerWanderWaypoints.Add(child);
        }
        
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(StalkerAI());
        
        
        
        
    }

    private IEnumerator WanderCoroutine() {
        while (true) {
            print("in here" + stalkerWanderWaypoints.Count);
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            target = stalkerWanderWaypoints[Random.Range(0, stalkerWanderWaypoints.Count)];
            agent?.SetDestination(target.position);
            animator.Play("Walk");
            yield return new WaitUntil(() => Vector3.Distance(transform.position, target.position) < 1f);
            animator.Play("Idle");
        }
    }
    
    

    private IEnumerator StalkerAI() {
        yield return new WaitForSeconds(1f);
 
        wanderCoroutine = StartCoroutine(WanderCoroutine());

        yield return new WaitUntil(
            () => Vector3.Distance(player.transform.position, transform.position) < chaseDistance);
        
        InitiateChase();

        if (wanderCoroutine != null) {
            StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }

        yield return null;
    }

    private IEnumerator ChaseCoroutine() {
        normalFace.SetActive(false);
        grinFace.SetActive(true);
        GetComponent<CapsuleCollider>().enabled = true;
        knife.SetActive(true);
        chaseSound.Play();
        chasing = true;
        animator.Play("Run");
        GameEvents.RaiseSetNight3ChaseIntoPlayerInput(true);
        while (true) {
            yield return new WaitForSeconds(0.5f);
            agent.SetDestination(player.transform.position);
            agent.speed += .025f;
        }
    }

    public void InitiateChase() {
        chaseCoroutine = StartCoroutine(ChaseCoroutine());
    }

    private IEnumerator LeavePlace() {
        if (GameQuery.OnGetIsPlayerCaught?.Invoke() ?? false) yield break;
        agent.SetDestination(GameObject.FindWithTag("StalkerAngryPoint").transform.position);
        
        yield return new WaitUntil(() => Vector3.Distance(transform.position, GameObject.FindWithTag("StalkerAngryPoint").transform.position) < 1f);
        
        animator.Play("Idle");
        
        weirdStare = true;
        yield return new WaitForSeconds(3f);
        normalFace.SetActive(false);
        angryFace.SetActive(true);
        transform.Find("GrowlSound").GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(2f);
        
        animator.Play("Run");
        
        agent.SetDestination(vanishingPoints[Random.Range(0, vanishingPoints.Count)].position);

        yield return new WaitForSeconds(5f);
        angryCutsceneInProgress = false;
        
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < 1f);
        animator.Play("Run");

        yield return new WaitForSeconds(2f);
        

        Destroy(gameObject);

        yield return null;
    }
    

    private void Update() {
        if (weirdStare) {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        }
    }

    private void ModifyNavMeshAreas(string areaToRemove, string areaToAdd) {
        int removeBit = 1 << NavMesh.GetAreaFromName(areaToRemove);
        int addBit = 1 << NavMesh.GetAreaFromName(areaToAdd);
        agent.areaMask = (agent.areaMask & ~removeBit) | addBit;
    }

    private void StopChase() {
        if (chaseCoroutine != null) {
            angryCutsceneInProgress = true;
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
            StartCoroutine(FadeOutChaseSoundAndStop());
            leaveCoroutine = StartCoroutine(LeavePlace());
        }
        
    }

    private IEnumerator FadeOutChaseSoundAndStop() {
        for (float i = 1f; i > 0; i -=  .05f) {
            chaseSound.volume = i;
            yield return new WaitForSeconds(.05f);
        }
        chaseSound.Stop();
        chaseSound.volume = 1f;
        yield return null;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && !caughtPlayer) {
            caughtPlayer = true;
            agent.enabled = false;
            StopChase();
            grinFace.SetActive(false);
            normalFace.SetActive(false);
            angryFace.SetActive(false);
            jumpscareFace.SetActive(true);
            GameEvents.RaiseOnStalkerJumpscare();
            GameEvents.RaiseOnKillPlayerInput();
            animator.Play("Idle");
            weirdStare = true;

            jumpscareCamObj.SetActive(true);
            CinemachineCamera cineCam = jumpscareCamObj.GetComponent<CinemachineCamera>();
            
            cineCam.Priority = 64;
            cineCam.gameObject.GetComponent<Animator>().Play("CameraDeath");
            
            GameEvents.RaiseOnGameOverShow();
            

        }
    }
    

    
    
}