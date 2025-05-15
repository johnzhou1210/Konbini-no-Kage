using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/*
 * Compatibility:
 * SIDEDOOR works with all of these except for OUTSIDEWALK
 * STARE and VARIEDSPEED works with all
 * OUTSIDEWALK only works with VARIEDSPEED and STARE
 * OBSESSIVE only works if there isn't OUTSIDEWALK
 */
public enum CustomerTendency {
    NORMAL,
    STARE,
    SIDEDOOR,
    OUTSIDEWALK,
    OBSESSIVE,
    VARIEDSPEED,
}

[System.Serializable]
public class CustomerTendencyGroup {
    public List<CustomerTendencyEntry> Tendencies = new();

    public CustomerTendencyGroup(List<CustomerTendencyEntry> tendencies) { Tendencies = tendencies; }
}

[System.Serializable]
public struct CustomerTendencyEntry {
    public HashSet<CustomerTendency> Types;
    public int Time;

    public CustomerTendencyEntry(HashSet<CustomerTendency> types, int time) {
        Types = types;
        Time = time;
    }
}

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

    private List<Transform> waypoints, vanishingPoints, outsideStarePoints;

    private Coroutine wanderCoroutine;
    private Coroutine checkoutCoroutine;
    private Coroutine leaveCoroutine;
    private Coroutine moveCoroutine;
    private Coroutine varySpeedCoroutine;

    private HashSet<CustomerTendency> customerTendencies;
    private bool weirdStare = false;
    private bool windowShopping = false;

    private Vector3 targetPos;

    private GameObject player;

    private void OnValidate() { this.ValidateRefs(); }


    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        waypoints = new List<Transform>();
        outsideStarePoints = new List<Transform>();
        vanishingPoints = new List<Transform>();
        customerTendencies = new HashSet<CustomerTendency>();
    }

    private void Start() {
        foreach (Transform child in GameObject.FindWithTag("OutsideStarePoints").transform) {
            outsideStarePoints.Add(child);
        }

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
                List<KonbiniItem> queriedItems = GameQuery.OnGetCounterItemDisplayAllItems?.Invoke() ?? new();
                KonbiniItem randomItem = queriedItems[Random.Range(0, queriedItems.Count)];
                customItems.Add(randomItem);
            }
        }
    }

    private IEnumerator WanderCoroutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));
            target = customerTendencies.Contains(CustomerTendency.OUTSIDEWALK) ?
                outsideStarePoints[Random.Range(0, outsideStarePoints.Count)] :
                waypoints[Random.Range(0, waypoints.Count)];
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
        GameEvents.RaiseOnLineupManagerLineUpCustomer(this);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetPos) < .5f);
        animator.Play("Idle");

        Debug.LogWarning("Customer lined up!");
        
        if (customerTendencies.Contains(CustomerTendency.STARE) || Random.Range(0f, 1f) < .75f) {
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

        if (!customerTendencies.Contains(CustomerTendency.OUTSIDEWALK)) {
            checkoutCoroutine = StartCoroutine(CheckoutCoroutine());
        } else {
            leaveCoroutine = StartCoroutine(LeaveStore());
        }


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

        if (customerTendencies.Contains(CustomerTendency.SIDEDOOR)) {
            if (Random.Range(0f, 1f) < .5f) {
                // Remove access to SideDoorArea and grant access to FrontDoorArea
                ModifyNavMeshAreas("SideDoorArea", "FrontDoorArea");
            }
        }

        bool willNod = Random.Range(0f, 1f) <= .67f;
        if (customerTendencies.Contains(CustomerTendency.OUTSIDEWALK)) {
            willNod = false;
        }


        if (willNod) {
            weirdStare = true;
            // stare player regardless of distance
            playerLookRange = 256;
            yield return new WaitForSeconds(Random.Range(0f, 1f));
            animator.Play("Nod");
        }

        
        yield return new WaitForSeconds(Random.Range(.5f, 3f));
        playerLookRange = 0f;
        if (!customerTendencies.Contains(CustomerTendency.STARE)) weirdStare = false;
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
        if (inline && (GameQuery.OnGetLineupManagerQueue?.Invoke() ?? new Queue<CustomerBehavior>()).Peek() == this &&
            ReachedFront == false) {
            ReachedFront = true;
            if (customerTendencies.Contains(CustomerTendency.OBSESSIVE)) {
                UpdateItemsToObsessive();
            }
            Invoke(nameof(SetupItemsAndEnableInteraction), 1f);
        }
    }

    private IEnumerator VarySpeedCoroutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            agent.speed = Random.Range(.5f, 8f);
        }
    }

    public void AddCustomerTendency(CustomerTendency newTendency) {
        customerTendencies.Add(newTendency);

        if (newTendency == CustomerTendency.STARE) {
            weirdStare = true;
        } else if (newTendency == CustomerTendency.SIDEDOOR) {
            // Remove access to FrontDoorArea and grant access to SideDoorArea
            ModifyNavMeshAreas("FrontDoorArea", "SideDoorArea");
        } else if (newTendency == CustomerTendency.OUTSIDEWALK) {
            minWait /= 10f; maxWait /= 10f;
        } else if (newTendency == CustomerTendency.VARIEDSPEED) {
            varySpeedCoroutine = StartCoroutine(VarySpeedCoroutine());
        }
    }

    private void Update() {
        if (!weirdStare) return;
        if (!customerTendencies.Contains(CustomerTendency.STARE) &&
            Vector3.Distance(transform.position, player.transform.position) > playerLookRange) return;

        Vector3 targetDirection = player.transform.position - transform.position;
        targetDirection.y = 0;

        if (targetDirection != Vector3.zero) {
            if (customerTendencies.Contains(CustomerTendency.STARE)) {
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y,
                    player.transform.position.z));
            } else {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
        }
    }

    private void ModifyNavMeshAreas(string areaToRemove, string areaToAdd) {
        int removeBit = 1 << NavMesh.GetAreaFromName(areaToRemove);
        int addBit = 1 << NavMesh.GetAreaFromName(areaToAdd);
        agent.areaMask = (agent.areaMask & ~removeBit) | addBit;
    }

    private void UpdateItemsToObsessive() {
        KonbiniItem rope = GameQuery.OnGetCounterItemDisplayGetItemByIndex?.Invoke(9);
        KonbiniItem scissors = GameQuery.OnGetCounterItemDisplayGetItemByIndex?.Invoke(11);
            
        KonbiniItem Get(int indx) {
            return GameQuery.OnGetCounterItemDisplayGetItemByIndex?.Invoke(indx);
        }
            
        List<List<KonbiniItem>> obsessiveItems = new() {
            /*
             * 1: Bento
             * 2: CandyBar
             * 3: Cigarettes
             * 4: Chips
             * 5: Coffee
             * 6: Karaage
             * 7: Pocky
             * 8: Ramen
             * 9: Rope
             * 10: Sandwich
             * 11: Scissors
             * 12: StrawberryMilk
             * 13: Tea
             * 14: Knife
             * 15: Gloves
             */
                
            new(){Get(2), Get(2), Get(2), Get(2), Get(2), Get(2), Get(2), Get(2),},
            new(){Get(1), Get(15), Get(14)},
            new(){Get(12), Get(9), Get(9), Get(9), Get(9), Get(9), Get(11)},
            new(){Get(14), Get(14), Get(14), Get(14), Get(14), Get(14)},
            new(){Get(11), Get(11), Get(11), Get(11), Get(11), Get(11), Get(11), Get(11), Get(11)},
            new(){Get(9), Get(9), Get(9), Get(9), Get(9), Get(9), Get(9)},
        };
            
        customItems = obsessiveItems[Random.Range(0, obsessiveItems.Count)];
        Debug.LogWarning("updated customItems to " + string.Join(", ", customItems.Select(t => t.ToString())));
    }

    private void SetupItemsAndEnableInteraction() { GameEvents.RaiseOnCustomerItemEnableInteraction(customItems); }
}