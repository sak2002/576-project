using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeMovement : MonoBehaviour
{

    GameObject gridObj;
    List<TileType>[,] grid;
    private Transform transform;
    int w;
    int l;
    float x;
    float y;
    int steps = 0;
    bool[,] visited;
    List<int[]> directions;
    int goal_w;
    int goal_l;
    float goal_x;
    float goal_y;
    float speed;
    float attack_speed;
    private GameObject target;


    // Start is called before the first frame update
    void Start()
    {
        gridObj = GameObject.Find("16x16");
        // Debug.Log(gridObj);
        grid = gridObj.GetComponent<LevelGenerator>().grid;
        transform = GetComponent<Transform>();

        w = (int)(transform.position.x + 23)/2;
        l = (int)(transform.position.z + 23)/2;
        x = (transform.position.x);
        y = (transform.position.z);
        goal_w = w;
        goal_l = l;
        goal_x = (float)(goal_w*2) - 23;
        goal_y = (float)(goal_l*2) - 23;

        directions = new List<int[]>();
        directions.Add(new int[2]{-1,0});
        directions.Add(new int[2]{1,0});
        directions.Add(new int[2]{0,-1});
        directions.Add(new int[2]{0,1});

        visited = new bool[24, 24];
        for(int i = 0; i<24; i++) {
            for(int j = 0; j<24; j++) {
                visited[w,l] = false;
            }
        }

        speed = 0.5f;
        attack_speed = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // check if player is in front
        target = GameObject.Find("player");

        if(target != null) {
            Vector3 target_pos = target.GetComponent<BoxCollider>().bounds.center;
            Vector3 current_pos = transform.position;
            current_pos.y = 2;
            Vector3 dir = target_pos-current_pos;

            // Debug.Log("target pos is " + target_pos);
            // Debug.Log("my pos is " + current_pos);
            // Debug.Log("angle is " + Vector3.Angle(transform.forward, dir));
            // Debug.Log("Disctance is " + dir.magnitude);


            Debug.DrawRay(current_pos, dir, Color.red);

            if(inLineOfSight(dir, current_pos) && inFront(dir) && inProximity(target_pos)) {
                // Debug.Log("Draw");
                // Debug.DrawRay(current_pos, dir, Color.green);
                Vector3 dir_hat = new Vector3(target_pos.x - current_pos.x, 0, target_pos.z - current_pos.z);
                dir_hat = dir_hat/dir_hat.magnitude;
                transform.position += dir_hat * attack_speed * Time.deltaTime;
                transform.forward = dir_hat;
                x = transform.position.x;
                y = transform.position.z;
            } else {
                moveRandomly();
            }

            

            // Debug.Log(target_pos + " " + current_pos);

            // Vector3 dir = target_pos - current_pos;
            // dir = dir / dir.magnitude;

            // if()
        } else {
        // else move randomly
            moveRandomly();
        }
    }

    private bool inFront(Vector3 dir) {
        Vector3 fwd = transform.forward;
        fwd.y = dir.y;
        float angle = Vector3.Angle(transform.forward, dir);
        // Debug.Log(angle);
        if(Math.Abs(angle) < 20) {
             
            return true;
        }
        return false;
    }

    private bool inProximity(Vector3 target_pos) {
        Vector3 dir = target_pos - transform.position;
        // Debug.Log("magd " + dir.magnitude);
        if(dir.magnitude < 4) {
            return true;
        }
        return false;
    }

    private bool inLineOfSight(Vector3 dir, Vector3 current_pos) {
        RaycastHit hit;
        if(Physics.Raycast(current_pos, dir, out hit, Mathf.Infinity)) {
            // Debug.Log(hit.transform.name);
            // // Debug.Log(current_pos);
            // Debug.DrawRay(current_pos, dir, Color.green);
            if(hit.transform.name == "player") {
                return true;
            }
        }
        return false;
    }

    private void moveRandomly() {
        // Debug.Log("herhe");
        System.Random random = new System.Random();
        if(steps > 10) {
            // Debug.Log("Stuck");
            for(int i = 0; i<24; i++) {
                for(int j = 0; j<24; j++) {
                    visited[i,j] = false;
                }
            }
            steps = 0;
        }

        if(Math.Abs(x - goal_x) < 0.01f && Math.Abs(y - goal_y) < 0.01f) {
            // get a new goal
            transform.position = new Vector3(goal_x, transform.position.y ,goal_y);
            x = goal_x;
            y = goal_y;
            // Debug.Log("Currently at " + x + " " + y);
            w = (int)(x + 23)/2;
            l = (int)(y + 23)/2;

            int i = 0;
            while(i<4) {
                // Debug.Log("trying");
                int index = random.Next(directions.Count);
                int[] direction = directions[index];
                // Debug.Log("checking in " + (w+direction[0]) + " " + (l+direction[1]) + " " + visited[w+direction[0], l+direction[1]] + " " + grid[w+direction[0], l+direction[1]][0]);
                if(visited[w+direction[0], l+direction[1]] == false && (grid[w+direction[0], l+direction[1]][0] == TileType.FLOOR || grid[w+direction[0], l+direction[1]][0] == TileType.HEALTH)) {
                    goal_w = w+direction[0];
                    goal_l = l+direction[1];

                    goal_x = (goal_w*2) - 23;
                    goal_y = (goal_l*2) - 23;
                    // Debug.Log("Currently goal " + goal_x + " " + goal_y);
                    break;
                }
                i++;
            }

            if(i > 4) {
                steps = 11;
            } else {
                steps++;
            }
            visited[w, l] = true;
        } else {
            // move towards goal
            Vector3 dir = new Vector3(goal_x - x, 0, goal_y - y);
            dir = dir / dir.magnitude;
            transform.position += dir * speed * Time.deltaTime;
            transform.forward = dir;
            x = transform.position.x;
            y = transform.position.z;
            // Debug.Log("Iteration Currently at " + x + " " + y);
        }
    }
}
