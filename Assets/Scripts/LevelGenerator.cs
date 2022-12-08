using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

enum TileType
{
    WALL = 0,
    FLOOR = 1,
    TRAP = 2,
    HEALTH = 3,
    CATAPULT = 4,
    LETTER = 5,
}

public class LevelGenerator : MonoBehaviour
{

    private Bounds bounds;
    public GameObject box_prefab;
    public GameObject wall_prefab;
    private int width;
    private int length;
    private List<string> wordBank = new List<string>{ "Apple"};
    Dictionary<char, List<int[]>> objectives;
    List<TileType>[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("here");
        bounds = GetComponent<Collider>().bounds;
        Debug.Log(bounds.min.x);
        Debug.Log(bounds.max.x);
        Debug.Log(bounds.min.y);
        Debug.Log(bounds.max.y);
        Debug.Log(bounds.min.z);
        Debug.Log(bounds.max.z);

        length = 16;
        width = 16;

        grid = new List<TileType>[width, length];
        objectives = new Dictionary<char, List<int[]>>();

        bool success = false;

        while(!success) {
            // create a border
            createBorder(grid);
            // randomly place the x letters
            int letters = placeLetters(grid);
            // randomly place the x deadends
            placeDeadends(grid, letters);

            success = true;
        }

        renderLevel(grid);


        // GameObject prefab = generateBoxObject("B");
        // GameObject box = Instantiate(box_prefab, new Vector3(0, 3.28f, 0), Quaternion.identity);

        // prefab = generateBoxObject("A");
        // box = Instantiate(box_prefab, new Vector3(10, 3.28f, 10), Quaternion.identity);

        Debug.Log("here");
    }

    private void renderLevel(List<TileType>[,] grid) {
        int w = 0;
        for (float x = bounds.min[0]+1; x < bounds.max[0]; x += bounds.size[0] / (float)width - 1e-6f, w++)
        {
            int l = 0;
            for (float z = bounds.min[2]+1; z < bounds.max[2]; z += bounds.size[2] / (float)length - 1e-6f, l++)
            {
                Debug.Log(w + " " + l);
                if ((w >= width) || (l >= width))
                    continue;

                float y = 2f;
                if(grid[w, l] == null)
                    continue;
                if (grid[w, l][0] == TileType.LETTER) 
                {
                    char current;
                    foreach(char c in objectives.Keys) {
                        List<int[]> coords = objectives[c];
                        foreach(int[] coord in coords) {
                            if(w == coord[0] && l == coord[1]) {
                                current = c;
                                GameObject prefab = generateBoxObject(current);
                                GameObject letter = Instantiate(box_prefab, new Vector3(x, y, z), Quaternion.identity);
                                letter.name = "Letter_" + current;
                                break;
                            }
                        }
                    }
                }
                else if (grid[w, l][0] == TileType.WALL)
                {
                    GameObject wall = Instantiate(wall_prefab, new Vector3(x, y, z), Quaternion.identity);
                    wall.name = "WALL";
                }
            }
        }
    }

    private int placeLetters(List<TileType>[,] grid) {
        System.Random random = new System.Random();
        int index = random.Next(wordBank.Count);
        string word = wordBank[index];
    
        foreach (char c in word)
        {
            while (true) {
                System.Random rnd = new System.Random();
                int wr = rnd.Next(1, width - 1);
                int lr = rnd.Next(1, length - 1);

                if (grid[wr, lr] == null)
                {
                    if(!objectives.ContainsKey(Char.ToUpper(c))) {
                        objectives.Add(Char.ToUpper(c), new List<int[]>());
                    }
                    grid[wr, lr] = new List<TileType> { TileType.LETTER };
                    objectives[Char.ToUpper(c)].Add(new int[2] { wr, lr });
                    break;
                }
            }
        }

        return word.Length;
    }

    private void placeDeadends(List<TileType>[,] grid, int x) {
        System.Random rnd = new System.Random();
        for(int i=0; i<x; i++) {
            int random = rnd.Next(0, 2);
            int wr;
            int lr;
            if(random == 0) {
                wr = rnd.Next(1, width-1);
                int lr_i = rnd.Next(0, 2);
                if(lr_i == 0) {
                    lr = 0;
                } else {
                    lr = length-1;
                }
            } else {
                lr = rnd.Next(1, length-1);
                int wr_i = rnd.Next(0, 2);
                if(wr_i == 0) {
                    wr = 0;
                } else {
                    wr = width-1;
                }
            }
            grid[wr, lr] = new List<TileType> { TileType.FLOOR };
        }
    }

    private void createBorder(List<TileType>[,] grid) {
        for (int w = 0; w < width; w++)
            for (int l = 0; l < length; l++)
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                    grid[w, l] = new List<TileType> { TileType.WALL };
    }

    private GameObject generateBoxObject(char letter) {
        GameObject box_changed = box_prefab;

        GameObject left = box_changed.transform.Find("left").gameObject;
        TextMeshPro textMeshProLeft = left.GetComponent<TextMeshPro>();
        textMeshProLeft.text = Char.ToString(letter);

        GameObject right = box_changed.transform.Find("right").gameObject;
        TextMeshPro textMeshProRight = right.GetComponent<TextMeshPro>();
        textMeshProRight.text = Char.ToString(letter);

        GameObject front = box_changed.transform.Find("front").gameObject;
        TextMeshPro textMeshProFront = front.GetComponent<TextMeshPro>();
        textMeshProFront.text = Char.ToString(letter);

        GameObject back = box_changed.transform.Find("back").gameObject;
        TextMeshPro textMeshProBack = back.GetComponent<TextMeshPro>();
        textMeshProBack.text = Char.ToString(letter);

        return box_changed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
