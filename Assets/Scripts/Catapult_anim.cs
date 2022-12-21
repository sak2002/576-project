using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult_anim : MonoBehaviour
{
    public GameObject weapon_prefab;

    private float shooting_delay;
    private Vector3 shooting_direction;
    private Vector3 weapon_starting_pos;
    private float weapon_velocity;
    private bool target_in_range;
    private GameObject target;
    private Vector3 target_position;
    private float distance_to_target = 0.0f;
    private float min_dist = 10.0f;
    private float max_dist = 30.0f;
    private Animator animation_controller;
    public bool throwing = false;
    

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("player");
        if (weapon_prefab == null)
            Debug.LogError("Error: could not find the weapon prefab in the project! Did you delete/move the prefab from your project?");
        shooting_delay = 10.0f;  
        weapon_velocity = 5.0f;
        weapon_starting_pos = new Vector3(0.0f, 0.0f, 0.0f);
        target_in_range = false;
        animation_controller = GetComponent<Animator>();
        StartCoroutine("Spawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            Debug.LogError("Error: could not find the game character 'player' in the scene. Did you delete the model player from your scene?");
        
        Vector3 target_centroid = target.GetComponent<BoxCollider>().bounds.center;
        Vector3 catapult_centroid = transform.position;
        distance_to_target = Vector3.Distance(target_centroid, catapult_centroid);

        // Here, we rotate the catapult in the direction of the target
        Vector3 direction_to_target = target_centroid - catapult_centroid;
        direction_to_target.Normalize();
        float angle_to_rotate_catapult = Mathf.Rad2Deg * Mathf.Atan2(shooting_direction.x, shooting_direction.z);
        transform.eulerAngles = new Vector3(0.0f, angle_to_rotate_catapult, 0.0f);

        if (distance_to_target >= min_dist && distance_to_target <= max_dist)
            target_in_range = true;
        else
            target_in_range = false;
        

        if(throwing) {
            animation_controller.SetInteger("State", 1);
            throwing = false;
        } else {
            animation_controller.SetInteger("State", 0);
            throwing = true;
        }

        // Debug.Log(throwing);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {            
            if(target_in_range) {
                // Defining the catapult object
                // throwing = true;
                Debug.Log("in loop " + throwing);
                GameObject catapult_arm_base = transform.Find("CatapultArmBase").gameObject;
                GameObject catapult_arm = catapult_arm_base.transform.Find("CatapultArm").gameObject;
                GameObject catapult_sphere = catapult_arm.transform.Find("CatapultSphere").gameObject;
                
                // Then we start to take the aim => Catapult starts to launch and rotates its arm
                // catapult_arm_base.transform.rotation = Quaternion.Euler(80.0f, 0, 0);
                weapon_starting_pos = catapult_sphere.transform.position;

                // Let's define the initial and final positions first
                Vector3 initialPos = weapon_starting_pos;
                Vector3 finalPos = target.transform.position;
                Vector3 displacement = finalPos - initialPos;

                // Calculate time taken
                float time = Mathf.Sqrt(Mathf.Abs(2 * displacement.y / 9.81f));
                // Debug.Log("displacement: " + displacement);
                // Debug.Log("time: " + time);
                Vector3 initial_velocity = displacement / time;
                initial_velocity.y = 0;

                // Now that we have our initial velocities, let us initiate the attack
                GameObject new_object = Instantiate(weapon_prefab, weapon_starting_pos, Quaternion.identity);
                new_object.GetComponent<Rock>().velocity = initial_velocity;
                new_object.GetComponent<Rock>().birth_time = Time.time;
                new_object.GetComponent<Rock>().birth_catapult = transform.gameObject;
                new_object.GetComponent<Rock>().initialPos = weapon_starting_pos;

                // Bringing back the catapult arm to its original position
                // catapult_arm_base.transform.rotation = Quaternion.Euler(-80.0f, 0, 0);
            } 
            // Add some delay, we don't want to bombard the kid
            yield return new WaitForSeconds(shooting_delay);
        }
    }
}
