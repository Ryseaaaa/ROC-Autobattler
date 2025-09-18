using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mnm : MonoBehaviour
{
    float spawnTime;
    void Start()
    {
        spawnTime = Time.time;
        gameObject.GetComponent<Rigidbody2D>().angularVelocity = HelperFunctions.ReturnRandomBool(0.5f,0) ? UnityEngine.Random.Range(30f, 50f) : -UnityEngine.Random.Range(30f, 50f);
    }

    void Update()
    {
        if (Time.time > spawnTime + 5) Destroy(gameObject);
    }
}
