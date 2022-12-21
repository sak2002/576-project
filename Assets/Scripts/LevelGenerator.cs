using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TileType
{
    WALL = 0,
    FLOOR = 1,
    TRAP = 2,
    HEALTH = 3,
    CATAPULT = 4,
    LETTER = 5,
    DEADEND = 6,
    MAZE_WALL = 7,
    NPC = 8,
    TOWER = 9,
    SWORD = 10,
}

public class LevelGenerator : MonoBehaviour
{

    private Bounds bounds;
    public GameObject box_prefab;
    public GameObject wall_prefab;
    public GameObject tree_prefab_1;
    public GameObject tree_prefab_2;
    public GameObject tree_prefab_3;
    public GameObject heart_prefab;
    public GameObject trap_prefab;
    public GameObject spike_prefab;
    public GameObject slime_prefab;
    public GameObject tower_prefab;
    public GameObject player_prefab;
    public GameObject sword_prefab;
    public string word;
    private int width;
    private int length;
    private List<string> wordBank = new List<string>{"Rock", "Pink", "Huge", "Hope", "Cute", "Tree", "Slide", "Tower", "Apple", "Train"};
    public Dictionary<string, string> wordMeanings;
    List<int[]> destinations;
    List<int[]> letterDestinations;
    List<int[]> dndDestinations;
    List<int[]> npcDestinations;
    List<int[]> towerCoords;
    public Dictionary<char, List<int[]>> objectives;
    public List<TileType>[,] grid;
    int wStart;
    int lStart;

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log("here");
        bounds = GetComponent<Collider>().bounds;
        wordMeanings = new  Dictionary<string, string>();
        wordMeanings.Add("Rock", "A solid mass made up of minerals. Rock forms much of the earth's outer layer, including cliffs and mountains.");
        wordMeanings.Add("Pink", "The color that comes from mixing white and red paint.");
        wordMeanings.Add("Huge", "Of very large weight, size, or amount.");
        wordMeanings.Add("Hope", "A feeling or chance that something will happen the way one wants it to.");
        wordMeanings.Add("Cute", "Pretty, charming, or adorable.");
        wordMeanings.Add("Tree", "A woody plant that has a long main trunk and many branches. Trees usually grow quite tall.");
        wordMeanings.Add("Slide", "A smooth, sloping track down which a person or thing can slide; chute.");
        wordMeanings.Add("Tower", "A tall, narrow building or part of a building that rises high above the ground.");
        wordMeanings.Add("Apple", "A firm, round fruit with juicy white flesh and red, green, or yellow skin.");
        wordMeanings.Add("Train", "A connected series of railroad cars.");

        length = 24;
        width = 24;

        grid = new List<TileType>[width, length];
        objectives = new Dictionary<char, List<int[]>>();
        destinations = new List<int[]>();
        letterDestinations = new List<int[]>();
        dndDestinations = new List<int[]>();
        npcDestinations = new List<int[]>();
        towerCoords = new List<int[]>();

        bool success = false;
        int i = 0;

        while(!success && i < 100) {
            // create a border
            createBorder();

            // pick word
            word = pickWord();
            int numLetters = word.Length;

            // set corners as tower tiles
            // randomly place towers on tower tiles
            placeTowers(numLetters);

            // place random trees with high probability
            placeTrees();

            // place letters, deadend, npc on 20x20

            placeWord(word);
            placeNPCs(numLetters);
            placeDeadends(numLetters);

            placeSwords();

            // place 1 dead on each wall

            // connect letters, deadends to start
            System.Random rnd = new System.Random();
            // grid[3,14] = new List<TileType>{TileType.TRAP};
            // grid[7,9] = new List<TileType>{TileType.TRAP};
            // connect(3, 14, 7, 9);
            while (true) {
                wStart = rnd.Next(5, width - 5);
                lStart = rnd.Next(5, length - 5);

                if(grid[wStart, lStart] == null || grid[wStart, lStart][0] == TileType.FLOOR) {
                    break;
                }
            }

            // Debug.Log("sstart : " + wStart + " " + lStart);

            foreach(int[] dest in destinations) {
            //     Debug.Log("connecting : " + dest[0] + " " + dest[1]);
                connect(wStart, lStart, dest[0], dest[1]);
            }

            success = true;
            i++;
        }

        for(int k=0; k< width; k++) {
            String s = "";
            for(int j=0; j<width; j++) {
                s = s + " " + (grid[k, j][0]).ToString()[0];
            }
            // Debug.Log(s);
        }

        renderLevel(grid);


        // GameObject prefab = generateBoxObject("B");
        // GameObject box = Instantiate(box_prefab, new Vector3(0, 3.28f, 0), Quaternion.identity);

        // prefab = generateBoxObject("A");
        // box = Instantiate(box_prefab, new Vector3(10, 3.28f, 10), Quaternion.identity);

        // Debug.Log("here");
    }

    private void connect(int wstart, int lstart, int wend, int lend) {

        System.Random rnd = new System.Random();
        bool trap_placed = false;
        bool health_placed = false;
        
        int steps = 0;

        int direction_w;
        int direction_l;

        if(wstart > wend) {
            direction_w = -1;
        } else {
            direction_w = 1;
        }

        if(lstart > lend) {
            direction_l = -1;
        } else {
            direction_l = 1;
        }

        int x = wstart;
        int y = lstart;

        while(x != wend || y != lend) {
            // Debug.Log(x + " " + y);
            int choice = rnd.Next(0, 2);
            if(choice == 0) {
                // go in w
                if(x != wend) {
                    x = x + direction_w;
                    if (grid[x, y][0] == TileType.MAZE_WALL) {
                        grid[x, y] = new List<TileType>{TileType.FLOOR};
                    }
                }
            } else {
                // go in l
                if(y != lend) {
                    y = y + direction_l;
                    if (grid[x, y][0] == TileType.MAZE_WALL) {
                        grid[x, y] = new List<TileType>{TileType.FLOOR};
                    }
                }
            }

            steps++;
            
            int prob_trap = rnd.Next(0, 11);
            if(!trap_placed && prob_trap > 7 && steps > 6 && grid[x, y][0] == TileType.FLOOR) {
                grid[x, y] = new List<TileType>{TileType.TRAP};
                trap_placed = true;
            }
            
            int prob_health = rnd.Next(0, 11);
            if(trap_placed && !health_placed && prob_health > 4 && steps > 9 && grid[x, y][0] == TileType.FLOOR) {
                grid[x, y] = new List<TileType>{TileType.HEALTH};
                health_placed = true;
            }
        }
    }

    private void placeTrees() {
        for (int w = 2; w < width-2; w++) {
            for (int l = 2; l < length-2; l++) {
                System.Random rnd = new System.Random();
                int i = rnd.Next(1,11);
                if(i > 3) {
                // if(i > 0) {
                    grid[w, l] = new List<TileType> { TileType.MAZE_WALL };
                }
            }
        }
    }

    private void placeWord(string word) {
        int dist = 6;
        foreach (char c in word)
        {
            while (true) {
                System.Random rnd = new System.Random();
                int wr = rnd.Next(2, width - 2);
                int lr = rnd.Next(2, length - 2);

                if (checkConstraints(letterDestinations, wr, lr, dist) && (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR))
                {
                    if(!objectives.ContainsKey(Char.ToUpper(c))) {
                        objectives.Add(Char.ToUpper(c), new List<int[]>());
                    }
                    grid[wr, lr] = new List<TileType> { TileType.LETTER };
                    
                    // if (grid[wr, lr+1][0] == TileType.MAZE_WALL) grid[wr, lr+1] = new List<TileType> { TileType.FLOOR };
                    // if (grid[wr, lr-1][0] == TileType.MAZE_WALL) grid[wr, lr-1] = new List<TileType> { TileType.FLOOR };
                    // if (grid[wr+1, lr+1][0] == TileType.MAZE_WALL) grid[wr+1, lr+1] = new List<TileType> { TileType.FLOOR };
                    // if (grid[wr-1, lr-1][0] == TileType.MAZE_WALL) grid[wr-1, lr-1] = new List<TileType> { TileType.FLOOR };
                    // if (grid[wr-1, lr+1][0] == TileType.MAZE_WALL) grid[wr-1, lr+1] = new List<TileType> { TileType.FLOOR };
                    // if (grid[wr+1, lr-1][0] == TileType.MAZE_WALL) grid[wr+1, lr-1] = new List<TileType> { TileType.FLOOR };
                    // if (grid[wr+1, lr][0] == TileType.MAZE_WALL) grid[wr+1, lr] = new List<TileType> { TileType.FLOOR };
                    // if (grid[wr-1, lr][0] == TileType.MAZE_WALL) grid[wr-1, lr] = new List<TileType> { TileType.FLOOR };


                    objectives[Char.ToUpper(c)].Add(new int[2] { wr, lr });
                    destinations.Add(new int[2] { wr, lr });
                    letterDestinations.Add(new int[2] { wr, lr });
                    break;
                }
            }
        }
    }

    private void placeNPCs(int x) {
        System.Random rnd = new System.Random();
        int dist = 5;
        for(int i=0; i<x; i++) {
            // Debug.Log("placing npc");
            while (true) {
                int wr = rnd.Next(2, width - 2);
                int lr = rnd.Next(2, length - 2);

                if (checkConstraints(npcDestinations, wr, lr, dist) && (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR))
                {                 
                    grid[wr, lr] = new List<TileType> { TileType.NPC };
                    destinations.Add(new int[2] { wr, lr });
                    npcDestinations.Add(new int[2] { wr, lr });
                    break;
                }
            }
        }
    }

    private void placeDeadends(int x) {
        System.Random rnd = new System.Random();
        for(int i=0; i<x/2; i++) {
            // Debug.Log("placing deadend");
            while (true) {
                int wr = rnd.Next(2, width - 2);
                int lr = rnd.Next(2, length - 2);

                if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
                {                 
                    grid[wr, lr] = new List<TileType> { TileType.DEADEND };
                    destinations.Add(new int[2] { wr, lr });
                    dndDestinations.Add(new int[2] { wr, lr });
                    break;
                }
            }
        }
    }

    public bool checkConstraints(List<int []> dest, int w, int l, int dist) {
        // distance between letters > 2
        foreach(int[] start in dest) {
            if(start[0] == w && start[1] == l)
                continue;
            else {
                if(Math.Abs((start[0]-w) + (start[1]-l)) < dist) {
                    // Debug.Log("not valid");                        
                    return false;
                }
            }
        }
        return true;
    }

    public string pickWord() {
        System.Random random = new System.Random();
        int index = random.Next(wordBank.Count);
        string word = wordBank[index];
        return word;
    }

    public void placeTowers(int x) {
        int numTowers = x/2;
        List<int []> towerNodes = new List<int []>();
        towerNodes.Add(new int[2]{1,1});
        towerNodes.Add(new int[2]{-1,-1});
        towerNodes.Add(new int[2]{-1,1});
        towerNodes.Add(new int[2]{1,-1});
        
        int y = 0;

        grid[0, 0] = new List<TileType> {TileType.TOWER};
        grid[0, 1] = new List<TileType> {TileType.TOWER};
        grid[1, 0] = new List<TileType> {TileType.TOWER};
        grid[1, 1] = new List<TileType> {TileType.TOWER};

        grid[22, 22] = new List<TileType> {TileType.TOWER};
        grid[22, 23] = new List<TileType> {TileType.TOWER};
        grid[23, 22] = new List<TileType> {TileType.TOWER};
        grid[23, 23] = new List<TileType> {TileType.TOWER};

        grid[0, 22] = new List<TileType> {TileType.TOWER};
        grid[0, 23] = new List<TileType> {TileType.TOWER};
        grid[1, 22] = new List<TileType> {TileType.TOWER};
        grid[1, 23] = new List<TileType> {TileType.TOWER};

        grid[22, 0] = new List<TileType> {TileType.TOWER};
        grid[22, 1] = new List<TileType> {TileType.TOWER};
        grid[23, 0] = new List<TileType> {TileType.TOWER};
        grid[23, 1] = new List<TileType> {TileType.TOWER};

        while(y < numTowers && y < 4) {
            // tower location = towerNodes[y]
            int[] towerLoc = towerNodes[y];
            if(towerLoc[0] == 1 && towerLoc[1] == 1) {
                towerCoords.Add(new int[]{22,22});
            } else if(towerLoc[0] == 1 && towerLoc[1] == -1) {
                towerCoords.Add(new int[]{22,-22});
            } else if(towerLoc[0] == -1 && towerLoc[1] == -1) {
                towerCoords.Add(new int[]{-22,-22});
            } else if(towerLoc[0] == -1 && towerLoc[1] == 1) {
                towerCoords.Add(new int[]{-22,22});
            }
            
            y++;
        }
    }

    private void placeSwords() {
        System.Random rnd = new System.Random();
        for(int i=0; i<2; i++) {
            // Debug.Log("placing trap");
            while (true) {
                int wr = rnd.Next(1, width - 1);
                int lr = rnd.Next(1, length - 1);

                if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
                {                 
                    grid[wr, lr] = new List<TileType> { TileType.SWORD };
                    break;
                }
            }
        }
    }

    private void renderLevel(List<TileType>[,] grid) {
        System.Random rnd = new System.Random();
        int w = 0;

        foreach(int[] coord in towerCoords) {
            GameObject tower = Instantiate(tower_prefab, new Vector3(coord[0], 1, coord[1]), Quaternion.identity);
            tower.AddComponent<MeshCollider>();
            tower.name = "TOWER";
        }

        // float xStart = wStart * (bounds.size[0] / (float)width);
        // float zStart = lStart * (bounds.size[2] / (float)length);

        float xStart = bounds.min[0] + (float)wStart * (bounds.size[0] / (float)width);
        float zStart = bounds.min[2] + (float)lStart * (bounds.size[2] / (float)length);

        GameObject player = Instantiate(player_prefab, new Vector3(xStart, 1.0f, zStart), Quaternion.identity);
        player.name = "player";

        for (float x = bounds.min[0]+1; x < bounds.max[0]; x += bounds.size[0] / (float)width, w++)
        {
            int l = 0;
            for (float z = bounds.min[2]+1; z < bounds.max[2]; z += bounds.size[2] / (float)length, l++)
            {
                // Debug.Log(w + " " + l + " " + x + " " + z);
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
                                letter.name = "LETTER_" + current;
                                break;
                            }
                        }
                    }
                }
                else if (grid[w, l][0] == TileType.WALL)
                {
                    GameObject wall = Instantiate(wall_prefab, new Vector3(x, y, z), Quaternion.identity);
                    wall.AddComponent<MeshCollider>();
                    wall.name = "WALL";
                }
                else if (grid[w, l][0] == TileType.HEALTH)
                {
                    GameObject heart = Instantiate(heart_prefab, new Vector3(x, y, z), Quaternion.identity);
                    heart.name = "HEART";
                }
                else if (grid[w, l][0] == TileType.TRAP)
                {
                    GameObject trap = Instantiate(trap_prefab, new Vector3(x, 1f, z), Quaternion.identity);
                    trap.name = "TRAP";
                }
                else if (grid[w, l][0] == TileType.SWORD)
                {
                    GameObject sword = Instantiate(sword_prefab, new Vector3(x, 1.8f, z), Quaternion.identity);
                    sword.name = "SWORD";
                }
                else if (grid[w, l][0] == TileType.NPC)
                {
                    List<GameObject> npcList = new List<GameObject>();
                    npcList.Add(spike_prefab);
                    npcList.Add(slime_prefab);
                    int index = rnd.Next(npcList.Count);
                    GameObject curr_npc = npcList[index];
                    GameObject npc = Instantiate(curr_npc, new Vector3(x, 1f, z), Quaternion.identity);
                    if(curr_npc == slime_prefab)
                        npc.name = "NPC_SLIME";
                    else
                        npc.name = "NPC_SPIKE";
                }
                else if (grid[w, l][0] == TileType.MAZE_WALL)
                {
                    int prob = rnd.Next(1, 11);
                    List<GameObject> trees = new List<GameObject>();
                    trees.Add(tree_prefab_1);
                    trees.Add(tree_prefab_2);
                    trees.Add(tree_prefab_3);
                    int index = rnd.Next(trees.Count);
                    GameObject curr_tree = trees[index];
                    if(curr_tree == tree_prefab_2 || curr_tree == tree_prefab_3)
                        y = 1f;
                    GameObject tree = Instantiate(curr_tree, new Vector3(x, y, z), Quaternion.identity);
                    tree.AddComponent<BoxCollider>();
                    tree.name = "TREE";
                }
            }
        }
    }

    // private void placeHealth(int x) {
    //     System.Random rnd = new System.Random();
    //     for(int i=0; i<x; i++) {
    //         Debug.Log("placing heart");
    //         while (true) {
    //             int wr = rnd.Next(1, width - 1);
    //             int lr = rnd.Next(1, length - 1);

    //             if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
    //             {                 
    //                 grid[wr, lr] = new List<TileType> { TileType.HEALTH };
    //                 destinations.Add(new int[2] { wr, lr });
    //                 break;
    //             }
    //         }
    //     }
    // }

    private void createBorder() {
        for (int w = 0; w < width; w++) {
            for (int l = 0; l < length; l++) {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                    grid[w, l] = new List<TileType> { TileType.WALL };
                else
                    grid[w, l] = new List<TileType> { TileType.FLOOR };
            }
        }
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
