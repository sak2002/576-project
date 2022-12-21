using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public Vector3 velocity;
    public float birth_time;
    public GameObject birth_catapult;
    public Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float time_since_launch = Time.time - birth_time;
        if (time_since_launch > 10.0f)  // weapon lives for 10 sec max
        {
            Destroy(transform.gameObject);
        }
        float t = Time.deltaTime;
        Vector3 finalPos = new Vector3(0, 0, 0);
        finalPos.x = transform.position.x + velocity.x * t;
        finalPos.y = initialPos.y - 0.5f * 9.81f * time_since_launch * time_since_launch;
        finalPos.z = transform.position.z + velocity.z * t;
        transform.position = finalPos;
    }
}
