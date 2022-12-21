// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// enum TileType
// {
//     WALL = 0,
//     FLOOR = 1,
//     TRAP = 2,
//     HEALTH = 3,
//     CATAPULT = 4,
//     LETTER = 5,
//     DEADEND = 6,
//     MAZE_WALL = 7,
//     NPC = 8,
// }

// public class LevelGeneratorCopy : MonoBehaviour
// {

//     private Bounds bounds;
//     public GameObject box_prefab;
//     public GameObject wall_prefab;
//     public GameObject tree_prefab_1;
//     public GameObject tree_prefab_2;
//     public GameObject tree_prefab_3;
//     public GameObject heart_prefab;
//     public GameObject trap_prefab;
//     public GameObject spike_prefab;
//     public GameObject slime_prefab;
//     private int width;
//     private int length;
//     private List<string> wordBank = new List<string>{ "Apple", "Game", "Word"};
//     List<int[]> destinations;
//     Dictionary<char, List<int[]>> objectives;
//     List<TileType>[,] grid;

//     // Start is called before the first frame update
//     void Start()
//     {
//         Debug.Log("here");
//         bounds = GetComponent<Collider>().bounds;
//         Debug.Log(bounds.min.x);
//         Debug.Log(bounds.max.x);
//         Debug.Log(bounds.min.y);
//         Debug.Log(bounds.max.y);
//         Debug.Log(bounds.min.z);
//         Debug.Log(bounds.max.z);

//         length = 24;
//         width = 24;

//         grid = new List<TileType>[width, length];
//         objectives = new Dictionary<char, List<int[]>>();
//         destinations = new List<int[]>();

//         bool success = false;

//         while(!success) {
//             // create a border
//             createBorder(grid);
//             // randomly place the x letters
//             int letters = placeLetters(grid);
//             // randomly place the x deadends
//             placeDeadends(grid, letters);
//             placeHealth(letters);
//             createMaze(grid);
//             placeTraps(letters);
//             placeNPCs(letters);
//             success = checkConstraints();
//             if(!success) {
//                 grid = new List<TileType>[width, length];
//                 objectives = new Dictionary<char, List<int[]>>();
//                 destinations = new List<int[]>();
//             }
//         }

//         renderLevel(grid);


//         // GameObject prefab = generateBoxObject("B");
//         // GameObject box = Instantiate(box_prefab, new Vector3(0, 3.28f, 0), Quaternion.identity);

//         // prefab = generateBoxObject("A");
//         // box = Instantiate(box_prefab, new Vector3(10, 3.28f, 10), Quaternion.identity);

//         // Debug.Log("here");
//     }

//     public bool checkConstraints() {
//         // distance between words > 8
//         foreach(int[] start in destinations) {
//             foreach(int[] end in destinations) {
//                 if(start[0] == end[0] && start[1] == end[1])
//                     continue;
//                 else {
//                     if(Math.Sqrt((start[0]-end[0])*(start[0]-end[0]) + (start[1]-end[1])*(start[1]-end[1])) < 3) {
//                         return false;
//                     }
//                 }
//             }
//         }

//         //traps are in the middle
//         for (int w = 0; w < width; w++) {
//             for (int l = 0; l < length; l++) {
//                 if(grid[w, l][0] == TileType.TRAP) {                  
//                     if(grid[w+1, l][0] == TileType.WALL || grid[w+1, l+1][0] == TileType.WALL || grid[w+1, l-1][0] == TileType.WALL ||
//                         grid[w, l+1][0] == TileType.WALL || grid[w, l-1][0] == TileType.WALL ||
//                         grid[w-1, l][0] == TileType.WALL || grid[w-1, l+1][0] == TileType.WALL || grid[w-1, l-1][0] == TileType.WALL) {
//                             return false;
//                     }
//                 }
//             }
//         }
//         return true;
//     }

//     private void createPath(int w, int l, int w1, int l1, List<TileType>[,] grid) {
//         if(w < w1) {
//             if(l == l1) {
//                 int x = w;
//                 while(x <= w1) {
//                     if(grid[x, l][0] == TileType.LETTER || grid[x, l][0] == TileType.DEADEND || grid[x, l][0] == TileType.WALL || grid[x, l][0] == TileType.HEALTH) {
//                         x++;
//                     } else {
//                         grid[x, l] = new List<TileType> {TileType.FLOOR};
//                         x++;
//                     }
//                 }
//             } else if (l < l1) {
//                 int x = w;
//                 while(x <= w1) {
//                     if(grid[x, l][0] == TileType.LETTER || grid[x, l][0] == TileType.DEADEND || grid[x, l][0] == TileType.WALL || grid[x, l][0] == TileType.HEALTH) {
//                         x++;
//                     } else {
//                         grid[x, l] = new List<TileType> {TileType.FLOOR};
//                         x++;
//                     }
//                 }

//                 int y = l;
//                 while(y <= l1) {
//                     if(grid[w1, y][0] == TileType.LETTER || grid[w1, y][0] == TileType.DEADEND || grid[w1, y][0] == TileType.WALL || grid[w1, y][0] == TileType.HEALTH) {
//                         y++;
//                     } else {
//                         grid[w1, y] = new List<TileType> {TileType.FLOOR};
//                         y++;
//                     }
//                 }
//             } else {
//                 int x = w;
//                 while(x <= w1) {
//                     if(grid[x, l][0] == TileType.LETTER || grid[x, l][0] == TileType.DEADEND || grid[x, l][0] == TileType.WALL || grid[x, l][0] == TileType.HEALTH) {
//                         x++;
//                     } else {
//                         grid[x, l] = new List<TileType> {TileType.FLOOR};
//                         x++;
//                     }
//                 }

//                 int y = l1;
//                 while(y <= l) {
//                     if(grid[w1, y][0] == TileType.LETTER || grid[w1, y][0] == TileType.DEADEND || grid[w1, y][0] == TileType.WALL || grid[w1, y][0] == TileType.HEALTH) {
//                         y++;
//                     } else {
//                         grid[w1, y] = new List<TileType> {TileType.FLOOR};
//                         y++;
//                     }
//                 }
//             }
//         }  else {
//             if(l == l1) {
//                 return;
//             } else if(l<l1) {
//                 int y = l;
//                 while(y <= l1) {
//                     if(grid[w, y][0] == TileType.LETTER || grid[w, y][0] == TileType.DEADEND || grid[w, y][0] == TileType.WALL || grid[w, y][0] == TileType.HEALTH) {
//                         y++;
//                     } else {
//                         grid[w, y] = new List<TileType> {TileType.FLOOR};
//                         y++;
//                     }
//                 }
//             } else {
//                 int y = l1;
//                 while(y <= l) {
//                     if(grid[w, y][0] == TileType.LETTER || grid[w, y][0] == TileType.DEADEND || grid[w, y][0] == TileType.WALL || grid[w, y][0] == TileType.HEALTH) {
//                         y++;
//                     } else {
//                         grid[w, y] = new List<TileType> {TileType.FLOOR};
//                         y++;
//                     }
//                 }
//             }
//         }
//     }

//     private void placeTraps(int x) {
//         System.Random rnd = new System.Random();
//         for(int i=0; i<x; i++) {
//             Debug.Log("placing trap");
//             while (true) {
//                 int wr = rnd.Next(1, width - 1);
//                 int lr = rnd.Next(1, length - 1);

//                 if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
//                 {                 
//                     grid[wr, lr] = new List<TileType> { TileType.TRAP };
//                     break;
//                 }
//             }
//         }
//     }

//         private void placeNPCs(int x) {
//         System.Random rnd = new System.Random();
//         for(int i=0; i<x/2; i++) {
//             Debug.Log("placing trap");
//             while (true) {
//                 int wr = rnd.Next(1, width - 1);
//                 int lr = rnd.Next(1, length - 1);

//                 if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
//                 {                 
//                     grid[wr, lr] = new List<TileType> { TileType.NPC };
//                     break;
//                 }
//             }
//         }
//     }

//     private void createMaze(List<TileType>[,] grid) {
//         System.Random rnd = new System.Random();
//         for (int w = 0; w < width; w++) {
//             for (int l = 0; l < length; l++) {
//                 if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
//                     continue;
//                 if(grid[w , l][0] == TileType.FLOOR) {                    
//                     if(grid[w+1, l][0] == TileType.LETTER || grid[w+1, l+1][0] == TileType.LETTER || grid[w+1, l-1][0] == TileType.LETTER ||
//                         grid[w, l+1][0] == TileType.LETTER || grid[w, l-1][0] == TileType.LETTER ||
//                         grid[w-1, l][0] == TileType.LETTER || grid[w-1, l+1][0] == TileType.LETTER || grid[w-1, l-1][0] == TileType.LETTER) {
                            
//                     } else {
//                         grid[w, l] = new List<TileType> {TileType.MAZE_WALL}; 
//                     }
//                 }
//             }
//         }
//         // create path between all destinations
//         foreach(int[] start in destinations) {
//             foreach(int[] end in destinations) {
//                 createPath(start[0], start[1], end[0], end[1], grid);
//             }
//         }

//         // add random spaces
//         for (int w = 0; w < width; w++) {
//             for (int l = 0; l < length; l++) {
//                 if(grid[w, l][0] == TileType.MAZE_WALL) {                  
//                     if(grid[w+1, l][0] == TileType.FLOOR || grid[w+1, l+1][0] == TileType.FLOOR || grid[w+1, l-1][0] == TileType.FLOOR ||
//                         grid[w, l+1][0] == TileType.FLOOR || grid[w, l-1][0] == TileType.FLOOR ||
//                         grid[w-1, l][0] == TileType.FLOOR || grid[w-1, l+1][0] == TileType.FLOOR || grid[w-1, l-1][0] == TileType.FLOOR) {
//                             int prob = rnd.Next(1,11);
//                             if(prob > 8) {
//                                 grid[w, l][0] = TileType.FLOOR;
//                             }
//                     }
//                 }
//             }
//         }
//     }

//     private void renderLevel(List<TileType>[,] grid) {
//         System.Random rnd = new System.Random();
//         int w = 0;
//         for (float x = bounds.min[0]+1; x < bounds.max[0]; x += bounds.size[0] / (float)width - 1e-6f, w++)
//         {
//             int l = 0;
//             for (float z = bounds.min[2]+1; z < bounds.max[2]; z += bounds.size[2] / (float)length - 1e-6f, l++)
//             {
//                 // Debug.Log(w + " " + l);
//                 if ((w >= width) || (l >= width))
//                     continue;

//                 float y = 2f;
//                 if(grid[w, l] == null)
//                     continue;
//                 if (grid[w, l][0] == TileType.LETTER) 
//                 {
//                     char current;
//                     foreach(char c in objectives.Keys) {
//                         List<int[]> coords = objectives[c];
//                         foreach(int[] coord in coords) {
//                             if(w == coord[0] && l == coord[1]) {
//                                 current = c;
//                                 GameObject prefab = generateBoxObject(current);
//                                 GameObject letter = Instantiate(box_prefab, new Vector3(x, y, z), Quaternion.identity);
//                                 letter.name = "Letter_" + current;
//                                 break;
//                             }
//                         }
//                     }
//                 }
//                 else if (grid[w, l][0] == TileType.WALL)
//                 {
//                     GameObject wall = Instantiate(wall_prefab, new Vector3(x, y, z), Quaternion.identity);
//                     wall.AddComponent<MeshCollider>();
//                     wall.name = "WALL";
//                 }
//                 else if (grid[w, l][0] == TileType.HEALTH)
//                 {
//                     GameObject heart = Instantiate(heart_prefab, new Vector3(x, y, z), Quaternion.identity);
//                     heart.name = "HEART";
//                 }
//                 else if (grid[w, l][0] == TileType.TRAP)
//                 {
//                     GameObject trap = Instantiate(trap_prefab, new Vector3(x, 1f, z), Quaternion.identity);
//                     trap.name = "TRAP";
//                 }
//                 else if (grid[w, l][0] == TileType.NPC)
//                 {
//                     List<GameObject> npcList = new List<GameObject>();
//                     npcList.Add(spike_prefab);
//                     npcList.Add(slime_prefab);
//                     int index = rnd.Next(npcList.Count);
//                     GameObject curr_npc = npcList[index];
//                     GameObject npc = Instantiate(curr_npc, new Vector3(x, 1f, z), Quaternion.identity);
//                     npc.name = "NPC";
//                 }
//                 else if (grid[w, l][0] == TileType.MAZE_WALL)
//                 {
//                     int prob = rnd.Next(1, 11);
//                     List<GameObject> trees = new List<GameObject>();
//                     trees.Add(tree_prefab_1);
//                     trees.Add(tree_prefab_2);
//                     trees.Add(tree_prefab_3);
//                     int index = rnd.Next(trees.Count);
//                     GameObject curr_tree = trees[index];
//                     GameObject tree = Instantiate(curr_tree, new Vector3(x, y, z), Quaternion.identity);
//                     tree.AddComponent<BoxCollider>();
//                     tree.name = "TREE";
//                 }
//             }
//         }
//     }

//     private int placeLetters(List<TileType>[,] grid) {
//         System.Random random = new System.Random();
//         int index = random.Next(wordBank.Count);
//         string word = wordBank[index];
    
//         foreach (char c in word)
//         {
//             while (true) {
//                 System.Random rnd = new System.Random();
//                 int wr = rnd.Next(1, width - 1);
//                 int lr = rnd.Next(1, length - 1);

//                 if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
//                 {
//                     if(!objectives.ContainsKey(Char.ToUpper(c))) {
//                         objectives.Add(Char.ToUpper(c), new List<int[]>());
//                     }
//                     grid[wr, lr] = new List<TileType> { TileType.LETTER };
//                     objectives[Char.ToUpper(c)].Add(new int[2] { wr, lr });
//                     destinations.Add(new int[2] { wr, lr });
//                     break;
//                 }
//             }
//         }

//         return word.Length;
//     }

//     private void placeDeadends(List<TileType>[,] grid, int x) {
//         System.Random rnd = new System.Random();
//         for(int i=0; i<2*x; i++) {
//             int random = rnd.Next(0, 2);
//             int wr;
//             int lr;
//             if(random == 0) {
//                 wr = rnd.Next(1, width-1);
//                 int lr_i = rnd.Next(0, 2);
//                 if(lr_i == 0) {
//                     lr = 0;
//                 } else {
//                     lr = length-1;
//                 }
//             } else {
//                 lr = rnd.Next(1, length-1);
//                 int wr_i = rnd.Next(0, 2);
//                 if(wr_i == 0) {
//                     wr = 0;
//                 } else {
//                     wr = width-1;
//                 }
//             }
//             grid[wr, lr] = new List<TileType> { TileType.DEADEND };
//             destinations.Add(new int[2] { wr, lr });
//         }

//         for(int i=0; i<x/2; i++) {
//             while (true) {
//                 int wr = rnd.Next(1, width - 1);
//                 int lr = rnd.Next(1, length - 1);

//                 if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
//                 {                 
//                     grid[wr, lr] = new List<TileType> { TileType.DEADEND };
//                     destinations.Add(new int[2] { wr, lr });
//                     break;
//                 }
//             }
//         }
//     }

//     private void placeHealth(int x) {
//         System.Random rnd = new System.Random();
//         for(int i=0; i<x; i++) {
//             Debug.Log("placing heart");
//             while (true) {
//                 int wr = rnd.Next(1, width - 1);
//                 int lr = rnd.Next(1, length - 1);

//                 if (grid[wr, lr] == null || grid[wr, lr][0] == TileType.FLOOR)
//                 {                 
//                     grid[wr, lr] = new List<TileType> { TileType.HEALTH };
//                     destinations.Add(new int[2] { wr, lr });
//                     break;
//                 }
//             }
//         }
//     }

//     private void createBorder(List<TileType>[,] grid) {
//         for (int w = 0; w < width; w++) {
//             for (int l = 0; l < length; l++) {
//                 if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
//                     grid[w, l] = new List<TileType> { TileType.WALL };
//                 else
//                     grid[w, l] = new List<TileType> { TileType.FLOOR };
//             }
//         }
//     }

//     private GameObject generateBoxObject(char letter) {
//         GameObject box_changed = box_prefab;

//         GameObject left = box_changed.transform.Find("left").gameObject;
//         TextMeshPro textMeshProLeft = left.GetComponent<TextMeshPro>();
//         textMeshProLeft.text = Char.ToString(letter);

//         GameObject right = box_changed.transform.Find("right").gameObject;
//         TextMeshPro textMeshProRight = right.GetComponent<TextMeshPro>();
//         textMeshProRight.text = Char.ToString(letter);

//         GameObject front = box_changed.transform.Find("front").gameObject;
//         TextMeshPro textMeshProFront = front.GetComponent<TextMeshPro>();
//         textMeshProFront.text = Char.ToString(letter);

//         GameObject back = box_changed.transform.Find("back").gameObject;
//         TextMeshPro textMeshProBack = back.GetComponent<TextMeshPro>();
//         textMeshProBack.text = Char.ToString(letter);

//         return box_changed;
//     }

//     // Update is called once per frame
//     void Update()
//     {
        
//     }
// }
