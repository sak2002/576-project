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


    // Start is called before the first frame update
    void Start()
    {
        gridObj = GameObject.Find("16x16");
        Debug.Log(gridObj);
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
    }

    // Update is called once per frame
    void Update()
    {
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
