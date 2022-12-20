using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mousey : MonoBehaviour
{

    private Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public float walking_velocity;
    // public Text text;    
    public float velocity;
    public float time_of_death;
    public int num_lives;
    public bool has_won;
    public bool dead;
    public bool immune;
    public float immune_timer;

    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 1.5f;
        velocity = 0.0f;
        num_lives = 5;
        dead = false;
        has_won = false;
        immune = false;
        immune_timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(immune) {
            if(immune_timer == 0) {
                animation_controller.SetInteger("state", 5);
            }
            immune_timer += Time.deltaTime;
            Debug.Log(immune_timer);
            if(immune_timer > 10) {
                immune = false;
                immune_timer = 0;
            }
        }

        // Debug.Log("here");
        if(num_lives == 0 && dead == false && has_won == false) {
            animation_controller.SetInteger("state", 6);
            // Debug.Log("state 6");
            if(!dead)
                time_of_death = Time.time;
            dead = true;
            return;
        }

        if(Input.GetKey(KeyCode.UpArrow)) {
            if(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) {
                animation_controller.SetInteger("state", 1);
                // Debug.Log("state 1");
                if(Input.GetKey(KeyCode.Space)) {
                    animation_controller.SetInteger("state", 2);
                    // Debug.Log("state 2");
                    velocity += 0.6f;
                    if(velocity >= 3.0f*walking_velocity) {
                        velocity = 3.0f*walking_velocity;
                    }
                } else {
                    velocity += 0.4f;
                    if(velocity >= 2.0f*walking_velocity) {
                        velocity = 2.0f*walking_velocity;
                    }
                }
            } else {
                animation_controller.SetInteger("state", 3);
                // Debug.Log("state 3");
                velocity += 0.2f;
                if(velocity >= walking_velocity) {
                    velocity = walking_velocity;
                }
            }
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            animation_controller.SetInteger("state", 4);
            velocity = -1.0f;
        } else {
            animation_controller.SetInteger("state", 0);
            velocity = 0.0f;
        }

        if (Input.GetKey(KeyCode.LeftArrow)){
          transform.Rotate(new Vector3(0.0f,-0.5f,0.0f));
        }
        if (Input.GetKey(KeyCode.RightArrow)){
          transform.Rotate(new Vector3(0.0f,0.5f,0.0f));
        }
        
        // Debug.Log(velocity);
        Vector3 direction = transform.forward;
        direction.y = 0;
        transform.position += direction * velocity * Time.deltaTime;
    }

    void OnCollisionEnter(Collision e) {
        // Debug.Log(e.gameObject.name);
        if(e.gameObject.name.StartsWith("HEART")) {
            num_lives++;
            Debug.Log(num_lives);
        } else if (e.gameObject.name.StartsWith("NPC") && !immune) {
            num_lives -= 1;
            immune = true;
        }
        
    }
}
