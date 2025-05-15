using System;
using System.Collections.Generic;
using UnityEngine;

public class LineupManager : MonoBehaviour
{
    public float CheckoutStopRadius { get; private set; } = 1.5f;
    [field: SerializeField] public List<Transform> QueuePositions { get; private set; }
    public Queue<CustomerBehavior> queue = new Queue<CustomerBehavior>();


    private void OnEnable() {
        GameEvents.OnLineupManagerAdvanceQueue += AdvanceQueue;
        GameEvents.OnLineupManagerClearItems += ClearItems;
        GameEvents.OnLineupManagerLineUpCustomer += LineUpCustomer;

        GameQuery.OnGetLineupManagerQueue = () => queue;
    }

    private void OnDisable() {
        GameEvents.OnLineupManagerAdvanceQueue -= AdvanceQueue;
        GameEvents.OnLineupManagerClearItems -= ClearItems;
        GameEvents.OnLineupManagerLineUpCustomer -= LineUpCustomer;
        
        GameQuery.OnGetLineupManagerQueue = null;
    }

    public void LineUpCustomer(CustomerBehavior b) {
        queue.Enqueue(b);
        AssignQueuePositions();
        // CheckoutStopRadius += NPCLineupGap;
        // OnLineupGapUpdate?.Invoke(CheckoutStopRadius);
    }
    
    public void AdvanceQueue() {
        CustomerBehavior dequeuedCustomer = queue.Dequeue();
        dequeuedCustomer.InitiateLeave(true);
        Invoke(nameof(AssignQueuePositions), 3f);
        // CheckoutStopRadius += NPCLineupGap;
        // OnLineupGapUpdate?.Invoke(CheckoutStopRadius);
    }
    
    private void AssignQueuePositions()
    {
        int i = 0;
        foreach (var customer in queue)
        {
            if (i < QueuePositions.Count)
            {
                
                customer.MoveTo(QueuePositions[i].position, true);
            }
            else
            {
                // Handle overflow: wait at a distance or idle
                // customer.MoveTo(waitingArea.position);
            }
            i++;
        }
    }

    public void DequeueCustomer() {
        if (queue.Count > 0) {
            queue.Dequeue();
            AssignQueuePositions();
        }
    }

    public int GetLength() {
        return queue.Count;
    }


    public void ClearItems() {
        queue.Clear();
        
    }
    
}
