using System;
using System.Collections;
using UnityEngine;

public class PoliceLight : MonoBehaviour
{
    [SerializeField] private Light redLight;

    private void Start() {
        StartCoroutine(FlashRedLight());
    }

    IEnumerator FlashRedLight()
    {
        while (true)
        {
            redLight.enabled = true;
            yield return new WaitForSeconds(0.25f);
            redLight.enabled = false;
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    void Update() {
        redLight.transform.Rotate(Vector3.up * (180f * Time.deltaTime));
    }


}
