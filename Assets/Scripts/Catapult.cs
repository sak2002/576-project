using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : MonoBehaviour
{
    // Angular speed in radians per sec.
    public float speed = 1.0f;
    public GameObject weapon_prefab;

    private float shooting_delay; 
    private Vector3 direction_from_turret_to_target;
    private Vector3 shooting_direction;
    private Vector3 projectile_starting_pos;
    private float projectile_velocity;
    private GameObject target;
    private Vector3 target_position;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("player");
        target_position = target.transform.position;
        if (weapon_prefab == null)
            Debug.LogError("Error: could not find the barrel prefab in the project! Did you delete/move the prefab from your project?");
        shooting_delay = 0.5f;  
        projectile_velocity = 5.0f;
        direction_from_turret_to_target = new Vector3(0.0f, 0.0f, 0.0f);
        projectile_starting_pos = transform.position;
        StartCoroutine("Spawn");
    }


    void Update()
    {
        // // Determine which direction to rotate towards
        // Vector3 targetDirection = target.transform.position - transform.position;

        // // The step size is equal to speed times frame time.
        // float singleStep = speed * Time.deltaTime;

        // // Rotate the forward vector towards the target direction by one step
        // Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // // Draw a ray pointing at our target in
        // Debug.DrawRay(transform.position, newDirection, Color.red);

        // transform.rotation = Quaternion.LookRotation(newDirection);

        // Vector3 target_velocity = (target.transform.position - target_position) / Time.deltaTime;
        // shooting_direction = newDirection;
        // shooting_direction.Normalize();

        // Vector3 current_turret_direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y), 1.1f, Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y));
        // projectile_starting_pos = transform.position + 1.1f * current_turret_direction;  // estimated position of the turret's front of the cannon
        Vector3 target_centroid = target.GetComponent<CapsuleCollider>().bounds.center;
        // Vector3 turret_centroid = GetComponent<"ForestCataPultArm">.GetComponent<SphereCollider>().GetComponent<>.bounds.center;
        Vector3 turret_centroid = transform.position;
        // direction_from_turret_to_target = target_centroid - turret_centroid;
        // direction_from_turret_to_target.Normalize();

        Vector3 target_velocity = (target.transform.position - target_position) / Time.deltaTime;
        target_position = target.transform.position;
        shooting_direction = iterative_solution(target, target_centroid, turret_centroid, projectile_velocity);
        shooting_direction.Normalize();
        ////////////////////////////////////////////////

        float angle_to_rotate_turret = Mathf.Rad2Deg * Mathf.Atan2(shooting_direction.x, shooting_direction.z);
        transform.eulerAngles = new Vector3(0.0f, angle_to_rotate_turret, 0.0f);
        Vector3 current_turret_direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y), 1.1f, Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.y));
        projectile_starting_pos = transform.position + 1.1f * current_turret_direction;  // estimated position of the turret's front of the cannon
    }

    private IEnumerator Spawn()
    {
        while (true)
        {       
            Debug.Log("Spawning new weapon");
            GameObject new_object = Instantiate(weapon_prefab, projectile_starting_pos, Quaternion.identity);
            if(new_object == null) {
                Debug.LogError("NULL HAI OBJECT");
            }
            new_object.AddComponent<Weapon>();
            new_object.GetComponent<Weapon>().direction = shooting_direction;
            new_object.GetComponent<Weapon>().velocity = projectile_velocity;
            new_object.GetComponent<Weapon>().birth_time = Time.time;
            new_object.GetComponent<Weapon>().birth_turret = transform.gameObject;
        }
        yield return new WaitForSeconds(shooting_delay); // next shot will be shot after this delay
    }

    Vector3 iterative_solution(GameObject target, Vector3 target_centroid, Vector3 turret_centroid, float projectile_speed) {
        Vector3 future_pos = target_centroid;

        return (future_pos - turret_centroid);
    }


}
