using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CustomerBehavior : MonoBehaviour {
    [SerializeField, Self] private Animator animator;

    public Transform target; // Assign this in the Inspector
    private NavMeshAgent agent;
    [SerializeField] private float minWait = 3f, maxWait = 8f;
    [SerializeField] private float minCheckoutTime = 10f, maxCheckoutTime = 60f;
    [SerializeField] private float targetStopRadius = 1.5f;
    [SerializeField] private float playerLookRange = 5f;
    [SerializeField] private float windowShoppingChance = .5f;
    [SerializeField] private GameObject plasticBag;

    [SerializeField] private int minRandomItems = 1, maxRandomItems = 4;
    [SerializeField] private List<KonbiniItem> customItems;
    

    public bool ReachedFront = false;

    private List<Transform> waypoints, vanishingPoints;

    private Coroutine wanderCoroutine;
    private Coroutine checkoutCoroutine;
    private Coroutine leaveCoroutine;
    private Coroutine moveCoroutine;

    private bool weirdStare = false;
    private bool windowShopping = false;

    private Vector3 targetPos;

    private GameObject player;

    private void OnValidate() { this.ValidateRefs(); }

    private void Start() {
        waypoints = new List<Transform>();
        vanishingPoints = new List<Transform>();
        agent = GetComponent<NavMeshAgent>();

        foreach (Transform child in GameObject.FindWithTag("NPCSpawnPoints").transform) {
            vanishingPoints.Add(child);
        }
        
        foreach (Transform child in GameObject.FindWithTag("StandPoints").transform) {
            waypoints.Add(child);
        }

        StartCoroutine(CustomerAI());
        player = GameObject.FindGameObjectWithTag("Player");
        if (Random.Range(0f, 1f) < windowShoppingChance) {
            windowShopping = true;
        }
        
        if (customItems == null || customItems.Count == 0) {
            customItems = new List<KonbiniItem>();
            // Add random items
            int randomItems = Random.Range(minRandomItems, maxRandomItems + 1);
            for (int i = 0; i < randomItems; i++) {
                KonbiniItem randomItem = CounterItemDisplayer.Instance.AllItems[Random.Range(0, CounterItemDisplayer.Instance.AllItems.Count)];
                customItems.Add(randomItem);
            }
        }
        
    }

    private IEnumerator WanderCoroutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            target = waypoints[Random.Range(0, waypoints.Count)];
            agent?.SetDestination(target.position);
            animator.Play("Walk");
            yield return new WaitUntil(() => Vector3.Distance(transform.position, target.position) < targetStopRadius);
            animator.Play("Idle");
        }
    }

    private IEnumerator CheckoutCoroutine() {
        if (windowShopping) {
            if (wanderCoroutine != null) {
                StopCoroutine(wanderCoroutine);
                wanderCoroutine = null;
            }
            leaveCoroutine = StartCoroutine(LeaveStore());
            yield break;
        }
        
        // Set destination to checkout position
        targetPos = GameObject.FindWithTag("Lineup").transform.position;
        LineupManager.Instance.LineUpCustomer(this);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < .5f);
        animator.Play("Idle");

        Debug.LogWarning("Customer lined up!");
  
        // SetupItemsAndEnableInteraction();


        if (Random.Range(0f, 1f) < .75f) {
            weirdStare = true;
        }


        yield return null;
    }

    private IEnumerator CustomerAI() {
        wanderCoroutine = StartCoroutine(WanderCoroutine());

        yield return new WaitForSeconds(Random.Range(minCheckoutTime, maxCheckoutTime));

        if (wanderCoroutine != null) {
            StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }

        checkoutCoroutine = StartCoroutine(CheckoutCoroutine());


        yield return null;
    }

    public void InitiateLeave(bool boughtItem = false) {
        if (wanderCoroutine != null) {
            StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }

        if (checkoutCoroutine != null) {
            StopCoroutine(checkoutCoroutine);
            checkoutCoroutine = null;
        }

        leaveCoroutine = StartCoroutine(LeaveStore(boughtItem));
    }

    private IEnumerator LeaveStore(bool boughtItem = false) {
        if (boughtItem) plasticBag.SetActive(true);
        
        bool willNod = Random.Range(0f, 1f) <= .67f;

        if (willNod) {
            weirdStare = true;
            // stare player regardless of distance
            playerLookRange = 256;
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            animator.Play("Nod");
        }

        yield return new WaitForSeconds(Random.Range(.5f, 3f));
        playerLookRange = 0f;
        weirdStare = false;
        MoveTo(vanishingPoints[Random.Range(0, vanishingPoints.Count)].position);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < 1f);
        animator.Play("Idle");

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);

        yield return null;
    }

    public void MoveTo(Vector3 tPos, bool inline = false) {
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        moveCoroutine = StartCoroutine(MoveCoroutine(tPos, inline));
    }

    private IEnumerator MoveCoroutine(Vector3 tPos, bool inline = false) {
        float distanceThreshold = 1f;
        if (inline) distanceThreshold = 1.5f;
        if (Vector3.Distance(tPos, transform.position) > distanceThreshold) animator.Play("Walk");
        targetPos = tPos;
        agent.stoppingDistance = .1f;
        agent.SetDestination(targetPos);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, tPos) < .5f);
        if (inline && LineupManager.Instance.queue.Peek() == this && ReachedFront == false) {
            ReachedFront = true;
            Invoke(nameof(SetupItemsAndEnableInteraction), 1f);
            
            
        }
    }

    private void Update() {
        if (!weirdStare) return;
        if (Vector3.Distance(transform.position, player.transform.position) > playerLookRange) return;
        
        Vector3 targetDirection = player.transform.position - transform.position;
        targetDirection.y = 0;

        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
        }

        
    }

    private void SetupItemsAndEnableInteraction() {
        CustomerItem.Instance.EnableInteraction(customItems);
    }
    
}