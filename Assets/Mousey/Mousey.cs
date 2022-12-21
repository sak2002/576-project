using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool hit;
    public bool immune;
    public float immune_timer;
    public bool sword;
    public float sword_timer;
    private GameObject gridObj;
    public GameObject canvas;
    private Slider slider;
    private GameObject shieldUI;
    private GameObject swordUI;
    private Text wordUI;
    private Text lettersUI;
    Dictionary<char, List<int[]>> objectives;
    Dictionary<char, int> wordList;
    string word;
    bool idle_hack = true;


    // Start is called before the first frame update
    void Start()
    {

        // GameObject canvas = GameObject.Find("Canvas");
        // scroll_bar = canvas.transform.Find("HealthBar").transform.Find("HealthBar").transform.Find("HealthBarUI")
        canvas = GameObject.Find("Canvas");
        slider = canvas.transform.Find("Bar").GetComponent<Slider>();
        shieldUI = canvas.transform.Find("Shield").gameObject;
        swordUI = canvas.transform.Find("Sword").gameObject;
        wordUI = canvas.transform.Find("Word").gameObject.GetComponent<Text>();
        lettersUI = canvas.transform.Find("Letters").gameObject.GetComponent<Text>();
        Debug.Log(slider);
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        gridObj = GameObject.Find("16x16");
        objectives = gridObj.GetComponent<LevelGenerator>().objectives;
        word = gridObj.GetComponent<LevelGenerator>().word;
        Debug.Log(word);
        wordUI.text = "Target word: " + word;
        lettersUI.text = "Letters found: ";
        wordList = new Dictionary<char, int>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 1.5f;
        velocity = 0.0f;
        num_lives = 5;
        dead = false;
        has_won = false;
        immune = false;
        hit = false;
        immune_timer = 0.0f;
        sword = false;
        sword_timer = 0.0f;
        buildLetterList();
        shieldUI.SetActive(false);
        swordUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = num_lives;
        
        if(num_lives < 0 && !has_won && !dead) {
            animation_controller.SetInteger("state", 7);
            Debug.Log("DEEEAAADD");
            dead = true;
            return;
        }

        if(dead) {
            return;
        }

        if(hasWon() && !has_won) {
            Debug.Log("You Won!");
            has_won = true;
            return;
        }

        if(has_won) {
            return;
        }

        // if(hit) {
        //     Debug.Log("inside hit");
        //     animation_controller.SetInteger("state", 5);
        //     hit = false;
        // }

        if(immune) {
            immune_timer += Time.deltaTime;
            if(immune_timer > 10) {
                immune = false;
                shieldUI.SetActive(false);
                immune_timer = 0;
            }
        }

        if(sword) {
            sword_timer += Time.deltaTime;
            if(sword_timer > 10) {
                sword = false;
                swordUI.SetActive(false);
                sword_timer = 0;
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
            if(idle_hack) {
                velocity = 0.001f;
                idle_hack = !idle_hack;
            }
            else {
                velocity = -0.001f;
                idle_hack = !idle_hack;
            }
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
        } else if (e.gameObject.name.StartsWith("NPC_SLIME")) {
            if(sword) {
                Destroy(e.gameObject);
            } else {
                if(!immune) {
                    hit = true;
                    num_lives -= 1;
                    Debug.Log(num_lives);
                    immune = true;
                    shieldUI.SetActive(true);
                }
            }
        } else if (e.gameObject.name.StartsWith("NPC_SPIKE")) {
            if(sword) {
                Destroy(e.gameObject);
            } else {
                if(!immune) {
                    hit = true;
                    num_lives -= 2;
                    Debug.Log(num_lives);
                    immune = true;
                    shieldUI.SetActive(true);
                }
            }
        } else if (e.gameObject.name.StartsWith("LETTER")) {
            char c = e.gameObject.name[7];
            lettersUI.text = lettersUI.text + " " + c;
            Debug.Log(c);
            wordList[c] = wordList[c] - 1;
            Destroy(e.gameObject);
        } else if (e.gameObject.name.StartsWith("SWORD")) {
            sword = true;
            swordUI.SetActive(true);
            Destroy(e.gameObject);
        } else if (e.gameObject.name.StartsWith("Weapon") || e.gameObject.name.StartsWith("Rock")) {
            if(!immune) {
                hit = true;
                num_lives -= 2;
                Debug.Log(num_lives);
                Debug.Log("hitting rocks");
                immune = true;
                shieldUI.SetActive(true);
            }
        } else if (e.gameObject.name.StartsWith("Spikes") || e.gameObject.name.StartsWith("Spear")) {
            if(!immune) {
                hit = true;
                num_lives -= 1;
                Debug.Log(num_lives);
                Debug.Log("hitting spikes");
                immune = true;
                shieldUI.SetActive(true);
            }
        }
    }

    public void buildLetterList() {
        foreach(char c in objectives.Keys) {
            wordList.Add(c, objectives[c].Count);
        }
    }

    public bool hasWon() {
        foreach(char c in wordList.Keys) {
            if(wordList[c] > 0) {
                return false;
            }
        }
        return true;
    }
}
