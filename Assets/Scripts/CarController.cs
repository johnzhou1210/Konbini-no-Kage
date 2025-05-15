using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarController : MonoBehaviour {
    [SerializeField] private Animator innerAnimator, outerAnimator;
    private void Start() {
        StartCoroutine(InnerCarCoroutine());
        StartCoroutine(OuterCarCoroutine());
    }

    private IEnumerator InnerCarCoroutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(4f, 75f));
            innerAnimator.Play("InnerLanePassby");
        }
    }
    
    private IEnumerator OuterCarCoroutine() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(4f, 75f));
            outerAnimator.Play("OuterLanePassby");
        }
    }
    
}
