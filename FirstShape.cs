using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstShape : MonoBehaviour
{
    //this class contains the behaviour for the first shape
    private Vector3 velocity;
    public void SetVelocity(Vector3 vel) { velocity = vel; }
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + velocity * Time.deltaTime;//updates transform position
    }
}