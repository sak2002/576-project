using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector3 direction;
    public float velocity;
    public float birth_time;
    public GameObject birth_turret;

    // Start is called before the first frame update
    void Start()
    {        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Hereaaaa");
        if (Time.time - birth_time > 10.0f)  // weapon live for 10 sec max
        {
            Destroy(transform.gameObject);
        }
        transform.position = transform.position + velocity * direction * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        ////////////////////////////////////////////////
        // WRITE CODE HERE:
        // (a) if the object collides with Claire, subtract one life from her, and destroy the apple => wrote this logic in the Claire.cs file
        // (b) if the object collides with another apple, or its own turret that launched it (birth_turret), don't do anything
        // (c) if the object collides with anything else (e.g., terrain, a different turret), destroy the apple
        if(!(other.gameObject.name == birth_turret.name || other.gameObject.name == "Apple" || other.gameObject.name == "Apple(Clone)")) {
            Debug.Log("Hello " + other.gameObject.name);
            Destroy(this.gameObject);
        }
        ////////////////////////////////////////////////
    }
}