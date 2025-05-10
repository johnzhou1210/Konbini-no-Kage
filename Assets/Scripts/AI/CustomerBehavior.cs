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
    [SerializeField] private Vector3 vanishingPoint = Vector3.zero;
    [SerializeField] private float playerLookRange = 5f;
    [SerializeField] private float windowShoppingChance = .5f;
    [SerializeField] private GameObject plasticBag;


    [SerializeField] private Transform waypointsTransform;
    [SerializeField] private Transform checkoutTransform;

    private List<Transform> waypoints;

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
        agent = GetComponent<NavMeshAgent>();

        foreach (Transform child in waypointsTransform) {
            waypoints.Add(child);
        }

        StartCoroutine(CustomerAI());
        player = GameObject.FindGameObjectWithTag("Player");
        if (Random.Range(0f, 1f) < windowShoppingChance) {
            windowShopping = true;
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
        targetPos = checkoutTransform.position;
        LineupManager.Instance.LineUpCustomer(this);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < .5f);
        animator.Play("Idle");

        Debug.LogWarning("Customer lined up!");
        CustomerItem.Instance.EnableInteraction();


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
        MoveTo(vanishingPoint);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < .5f);
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
        if (inline) {
            CustomerItem.Instance.EnableInteraction();
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
}