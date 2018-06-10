using UnityEngine;
using System.Collections;
using System.Collections.Generic; //Allows us to use Lists. 
using UnityEngine.SceneManagement;
using Pathfinding;
using System.Linq;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameObject player, AI, floor, wall, key, canvas, pausePanel, healthPickup, enemy;
    private static Sprite[] floorTiles, wallTiles;
    public static GameObject[] enemies;
    public static Text playerHealthText, companionHeathText, keysCollectedText;
    GameObject resumeButton;
    GameObject exitButton;
    public AudioClip backgroundClip, introClip, menuClip;

    public static GameManager
        instance = null; //Static instance of GameManager which allows it to be accessed by any other script.

    static int keysCollected = 0;
    static List<List<GridNode>> regions;
    static int MapWidth;
    static int MapHeight;
    public static float playerHealth = 100;
    public static float companionHealth = 50;
    static int level = 0;
    static GridNode keyNode = null;
    static bool setOnClick = false;
    static string[] states = {"walkingLeft", "walkingRight", "walkingDown", "walkingUp", "idle"};

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null) // If instance already exists
            instance = this;
        else if (instance != this) //If instance already exists and it's not this:
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        player = (GameObject) Resources.Load("Characters/Player");
        AI = (GameObject) Resources.Load("Characters/AI");
        floor = (GameObject) Resources.Load("Map/Floor/Floor");
        floorTiles = Resources.LoadAll<Sprite>("Map/Floor");
        wall = (GameObject) Resources.Load("Map/Wall/Wall");
        wallTiles = Resources.LoadAll<Sprite>("Map/Wall");
        key = (GameObject) Resources.Load("Misc/Key");
        healthPickup = (GameObject) Resources.Load("Misc/Health Pickup");
        enemy = (GameObject) Resources.Load("Characters/Enemy");

        SoundManager.instance.playMusic(introClip);
        // load the intro scene
        SceneManager.LoadScene("intro");
    }

    void Update()
    {
        updateUI();

        if (playerHealth <= 0)
        {
            playerHealth = 100;
            Initiate.Fade("end", Color.black, 1);
            Initiate.Fade("main menu", Color.black, 1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            pauseGame();
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "main menu")
        {
            SoundManager.instance.playMusic(menuClip);
            GameObject.FindGameObjectWithTag("Start Button").GetComponent<Button>().onClick.AddListener(InitGame);
            GameObject.FindGameObjectWithTag("ExitButton").GetComponent<Button>().onClick.AddListener(exitGame);
        }
    }

    //Initializes the game for each level.
    void InitGame()
    {
        SoundManager.instance.playMusic(backgroundClip);
        Initiate.Fade("start", Color.black, 1);
    }

    void pauseGame()
    {
        GameObject playerInstance = GameObject.FindGameObjectWithTag("Player");
        GameObject AIInstance = GameObject.FindGameObjectWithTag("AI");
        if (setOnClick == false)
        {
            // pause for the first time
            // show pause panel and assign the required variables
            pausePanel = (GameObject) Resources.Load("Misc/PausePanel");
            pausePanel = Instantiate(pausePanel);
            pausePanel.SetActive(true);

            resumeButton = GameObject.FindGameObjectWithTag("ResumeButton");
            exitButton = GameObject.FindGameObjectWithTag("ExitButton");
            pausePanel.transform.SetParent(canvas.transform, false);
            resumeButton.transform.SetParent(pausePanel.transform, false);
            exitButton.transform.SetParent(pausePanel.transform, false);
            resumeButton.GetComponent<Button>().onClick.AddListener(resumeGame);
            exitButton.GetComponent<Button>().onClick.AddListener(exitGame);

            playerInstance.GetComponent<Player>().inputEnabled = false;
            playerInstance.GetComponent<Animator>().speed = 0;
            AIInstance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            AIInstance.GetComponent<Animator>().speed = 0;
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                enemy.GetComponent<Animator>().speed = 0;
            }

            setOnClick = true;
        }
        else
        {
            if (pausePanel.activeInHierarchy == false)
            {
                // pause
                pausePanel.SetActive(true);
                playerInstance.GetComponent<Player>().inputEnabled = false;
                playerInstance.GetComponent<Animator>().speed = 0;
                AIInstance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                AIInstance.GetComponent<Animator>().speed = 0;
                enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    enemy.GetComponent<Animator>().speed = 0;
                }
            }
            else
            {
                // resume
                pausePanel.SetActive(false);
                playerInstance.GetComponent<Animator>().speed = 1;
                playerInstance.GetComponent<Player>().inputEnabled = true;
                AIInstance.GetComponent<Animator>().speed = 1;
                AIInstance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

                enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    enemy.GetComponent<Animator>().speed = 1;
                }
            }
        }
    }

    // onClick listener for Pause button
    void resumeGame()
    {
        GameObject pausePanel = GameObject.FindGameObjectWithTag("PausePanel");

        if (pausePanel != null)
        {
            GameObject playerInstance = GameObject.FindGameObjectWithTag("Player");
            GameObject AIInstance = GameObject.FindGameObjectWithTag("AI");

            if (pausePanel.activeInHierarchy == true)
            {
                pausePanel.SetActive(false);

                playerInstance.GetComponent<Animator>().speed = 1;
                playerInstance.GetComponent<Player>().inputEnabled = true;
                AIInstance.GetComponent<Animator>().speed = 1;
                AIInstance.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemy in enemies)
                {
                    enemy.GetComponent<Animator>().speed = 1;
                    enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                }
            }
        }
    }

    void exitGame()
    {
        Application.Quit();
    }

    public static void changeLevel()
    {
        if (keysCollected < 3)
        {
            setOnClick = false;
            level++;
            Initiate.Fade("main", Color.black, 1);
        }
        else
            Initiate.Fade("end", Color.black, 1);
    }

    public static void addKey()
    {
        keysCollected++;
    }

    public static int getLevel()
    {
        return level;
    }

    void updateUI()
    {
        GameObject playerHealthObj = GameObject.FindGameObjectWithTag("Player Health");
        GameObject companionHealthObj = GameObject.FindGameObjectWithTag("Companion Health");
        GameObject keysCollectedObj = GameObject.FindGameObjectWithTag("Keys Collected");
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        if (playerHealthObj != null)
        {
            playerHealthText = playerHealthObj.GetComponent<Text>();
            companionHeathText = companionHealthObj.GetComponent<Text>();
            keysCollectedText = keysCollectedObj.GetComponent<Text>();

            string[] words = playerHealthText.text.Split(' ');
            if (float.Parse(words[2]) != playerHealth)
            {
                if (playerHealth < 0)
                    playerHealthText.text = "Player Health: " + 0;
                else
                    playerHealthText.text = "Player Health: " + playerHealth;
            }

            words = companionHeathText.text.Split(' ');
            if (float.Parse(words[2]) != companionHealth)
            {
                if (companionHealth < 0)
                    companionHeathText.text = "Companion Health: " + 0;
                else
                    companionHeathText.text = "Companion Health: " + companionHealth;
            }

            words = keysCollectedText.text.Split(' ');
            if (int.Parse(words[2]) != keysCollected)
                keysCollectedText.text = "Keys Collected: " + keysCollected;
        }
    }

    public static void createLevel()
    {
        regions = GameObject.FindGameObjectWithTag("Map").GetComponent<CellAuto>().regions;
        MapWidth = GameObject.FindGameObjectWithTag("Map").GetComponent<CellAuto>().MapWidth;
        MapHeight = GameObject.FindGameObjectWithTag("Map").GetComponent<CellAuto>().MapHeight;

        createEdgeWalls();
        // create exits (and spawn player and his companion), connect the regions and spawn the key with a certain probability
        createExits();

        if (SceneManager.GetActiveScene().name == "main")
        {
            connectRegions();
            spawnKey();
            spawnPickups();
            spawnEnemies();
        }
        else
        {
            // if it is the starting room create one door/exit and spawn player and his companion
            spawnPlayer();
        }
    }

    // this function is used only at the starting room for spawning the player
    static void spawnPlayer()
    {
        float randNum = Random.Range(1, 100);
        int minY = 0;
        bool stop = false;

        // find the lowest line of nodes of the room
        while (!stop)
        {
            foreach (GridNode node in regions[1])
            {
                if (node.ZCoordinateInGrid == minY)
                {
                    stop = true;
                    break;
                }
            }

            if (!stop)
                minY++;
        }

        GameObject p;
        if (randNum <= 50)
        {
            // spawn player on the leftmost node of the bottom line of nodes
            int minX = 99999;
            foreach (GridNode node in regions[1])
            {
                if (node.ZCoordinateInGrid == minY && node.XCoordinateInGrid < minX)
                    minX = node.XCoordinateInGrid;
            }


            p = Instantiate(player, new Vector3(minX, minY + 1, 0), Quaternion.identity);
            if (companionHealth > 0)
            {
                GameObject ai = Instantiate(AI, new Vector3(minX, minY, 0), Quaternion.identity);
                ai.GetComponentInChildren<MeshRenderer>().sortingOrder = 2;
            }
        }
        else
        {
            // spawn player on the rightmost node of the bottom line of nodes
            int maxX = -1;
            foreach (GridNode node in regions[1])
            {
                if (node.ZCoordinateInGrid == minY && node.XCoordinateInGrid > maxX)
                    maxX = node.XCoordinateInGrid;
            }

            p = Instantiate(player, new Vector3(maxX, minY + 1, 0), Quaternion.identity);
            if (companionHealth > 0)
            {
                GameObject ai = Instantiate(AI, new Vector3(maxX, minY, 0), Quaternion.identity);
                ai.GetComponentInChildren<MeshRenderer>().sortingOrder = 2;
            }
        }

        // hook the camera to the player
        GameObject.FindGameObjectWithTag("MainCamera").transform.parent = p.transform;
        GameObject.FindGameObjectWithTag("MainCamera").transform.localPosition = new Vector3(0, 0, -1);
    }

    // this function changes sprites of wall tiles that are above walkable floor tiles in order to make the map more realistic
    static void createEdgeWalls()
    {
        GameObject[] mapTiles = GameObject.FindGameObjectsWithTag("Tile");
        
        foreach (GridNode node in regions[0])
        {
            int x = node.XCoordinateInGrid;
            int y = node.ZCoordinateInGrid;
            if (y - 1 >= 0)
            {
                GameObject tileBelow = mapTiles[(y - 1) * MapWidth +x];
                if (tileBelow.layer == 8)
                    mapTiles[MapWidth * y + x].GetComponent<SpriteRenderer>().sprite =
                        wallTiles[Random.Range(0, wallTiles.Length)];
            }
        }
    }

    // this function creates 2 exits for the main rooms and 1 for the starting room
    static void createExits()
    {
        GameObject[] mapTiles = GameObject.FindGameObjectsWithTag("Tile");

        int x, y;

        if (SceneManager.GetActiveScene().name == "main")
        {
            // get the coordinates of the first node of the first walkable region
            x = regions[1][0].XCoordinateInGrid;
            y = regions[1][0].ZCoordinateInGrid;

            for (int j = y - 1; j >= 0; j--)
            {
                Vector3 _position = new Vector3((float) (x), (float) (j), 0);

                // destroy the wall tile first
                Destroy(mapTiles[MapWidth * j + x]);

                // instantiate the floor tile
                GameObject floorTile = Instantiate(floor, _position, Quaternion.identity);
                floorTile.GetComponent<SpriteRenderer>().sprite = floorTiles[Random.Range(0, floorTiles.Length)];

                if (j == 0) // if it is the downmost node
                {
                    // spawn player and companion

                    _position.y++;
                    GameObject p = Instantiate(player, _position, Quaternion.identity);
                    GameObject.FindGameObjectWithTag("MainCamera").transform.parent = p.transform;
                    GameObject.FindGameObjectWithTag("MainCamera").transform.localPosition = new Vector3(0, 0, -1);

                    if (companionHealth > 0)
                    {
                        _position.y--;
                        GameObject ai = Instantiate(AI, _position, Quaternion.identity);
                        ai.GetComponentInChildren<MeshRenderer>().sortingOrder = 2;
                    }
                }
            }
        }

        // get the coordinates of the last node of the last walkable region
        x = regions.Last().Last().XCoordinateInGrid;
        y = regions.Last().Last().ZCoordinateInGrid;

        for (int j = y + 1; j < MapHeight; j++)
        {
            Vector3 _position = new Vector3((float) (x), (float) (j), 0);

            // destroy the wall tile first
            Destroy(mapTiles[MapWidth * j + x]);

            // instantiate the floor tile
            GameObject floorTile = Instantiate(floor, _position, Quaternion.identity);
            floorTile.GetComponent<SpriteRenderer>().sprite = floorTiles[Random.Range(0, floorTiles.Length)];

            if (j == MapHeight - 1) // if it is the upmost node
            {
                // create exit
                floorTile.AddComponent<BoxCollider2D>();
                floorTile.GetComponent<BoxCollider2D>().isTrigger = true;
                floorTile.AddComponent<Door>();
            }
        }
    }

    // this functions connects regions by creating tunnels between each pair of them
    static void connectRegions()
    {
        GameObject[] mapTiles = GameObject.FindGameObjectsWithTag("Tile");
        int startX, startY, endX, endY, x, y, stepX, stepY;
        bool continueX, continueY;
        // connect regions pair by pair
        for (int i = 1; i < regions.Count - 1; i++)
        {
            // take the coordinates of the first node of each of the 2 regions
            startX = regions[i][0].XCoordinateInGrid;
            startY = regions[i][0].ZCoordinateInGrid;
            endX = regions[i + 1][0].XCoordinateInGrid;
            endY = regions[i + 1][0].ZCoordinateInGrid;

            if (startX < endX)
                stepX = 1; // go right
            else
                stepX = -1; // go left
            if (startY < endY)
                stepY = 1; // go up
            else
                stepY = -1; // go down

            x = startX;
            y = startY;
            continueX = true;
            continueY = true;
            while (continueX || continueY)
            {
                // if a target coordinate is reached do not continue in this direction
                if (x == endX)
                    continueX = false;
                if (y == endY)
                    continueY = false;

                if (continueX)
                {
                    x += stepX;
                    if (GameObject.FindGameObjectWithTag("Map").GetComponent<CellAuto>().mpHandler.Map[x, y] ==
                        1) // if it is a wall
                    {
                        Vector3 _position = new Vector3((float) (x), (float) (y), 0);

                        // make the wall tile a floor tile
                        Destroy(mapTiles[MapWidth * y + x]);
                        Instantiate(floor, _position, Quaternion.identity).GetComponent<SpriteRenderer>().sprite =
                            floorTiles[Random.Range(0, floorTiles.Length)];
                    }
                }

                if (continueY)
                {
                    y += stepY;
                    if (GameObject.FindGameObjectWithTag("Map").GetComponent<CellAuto>().mpHandler.Map[x, y] == 1)
                    {
                        Vector3 _position = new Vector3((float) (x), (float) (y), 0);

                        Destroy(mapTiles[MapWidth * y + x]);
                        Instantiate(floor, _position, Quaternion.identity).GetComponent<SpriteRenderer>().sprite =
                            floorTiles[Random.Range(0, floorTiles.Length)];
                    }
                }
            }
        }
    }

    // this function spawns the key in the smallest region with probability 80%
    static void spawnKey()
    {
        GameObject[] mapTiles = GameObject.FindGameObjectsWithTag("Tile");
        int randNum = Random.Range(1, 100);
        if (randNum >= 20)
        {
            // by default spawns the key in a random node in the first walkable region
            randNum = Random.Range(0, regions[1].Count);
            keyNode = regions[1][randNum];
            Vector3 _position = new Vector3(regions[1][randNum].XCoordinateInGrid,
                regions[1][randNum].ZCoordinateInGrid, 0);
            int min = 9999;

            // if there are more than 1 walkable regions find the smallest one and keep the coordinates of its last node
            for (int i = 2; i < regions.Count; i++)
            {
                if (regions[i].Count < min && i != regions.Count - 1)
                {
                    min = regions[i].Count;
                    keyNode = regions[i].Last();
                    _position = new Vector3(regions[i].Last().XCoordinateInGrid, regions[i].Last().ZCoordinateInGrid,
                        0);
                }
            }

            // spawn key
            Instantiate(key, _position, Quaternion.identity);
        }
    }

    static void spawnEnemies()
    {
        int randNum, randNum1;
        int enemiesNum = Random.Range(1, level);
        // spawn an enemy next to the key (if it exists)
        if (keyNode != null)
        {
            List<GraphNode> connections = new List<GraphNode>();
            keyNode.GetConnections(connections.Add);
            foreach (GridNode node in connections)
            {
                if (node.Walkable)
                {
                    // spawn enemy
                    if (keysCollected < 2)
                    {
                        GameObject e = Instantiate(enemy,
                            new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid, 0), Quaternion.identity);
                        /*e.tag = "Enemy";
                        e.AddComponent<Enemy>();
                        e.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Enemy AI");*/
                        int rNum = Random.Range(1, 100);

                        // 50-50 chance for the enemy to be ranged or melee
                        if (rNum <= 50)
                        {
                            e.GetComponent<Enemy>().ranged = true;
                            e.GetComponent<Enemy>().maxRange = level + Random.Range(2, 9);
                            e.GetComponent<Enemy>().tendency_to_chase_player = 1.5f; // big tendency to chase player
                        }
                        else
                            e.GetComponent<Enemy>().tendency_to_chase_player = 0.5f; // small tendency to chase player
                    }
                    else // spawn boss
                    {
                        GameObject e = Instantiate(enemy,
                            new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid, 0), Quaternion.identity);

                        e.transform.localScale = new Vector3(2, 2, 1);
                        e.GetComponent<Animator>().runtimeAnimatorController =
                            (RuntimeAnimatorController) Resources.Load("Animations/Enemy/FinalBossAnimator");

                        e.GetComponent<Enemy>().tendency_to_chase_player = 1.5f; // big tendency to chase player
                        e.GetComponent<Enemy>().health = 150;
                        e.GetComponent<Enemy>().DPS = 10;
                        e.GetComponent<AIPath>().maxSpeed = 1.75f;
                    }

                    break;
                }
            }
        }

        int startI = 0;
        if (keyNode != null)
            startI = 1;

        // spawn the rest of the enemies
        for (int i = startI; i < enemiesNum; i++)
        {
            randNum = 1;
            if (regions.Count > 2)
                randNum = Random.Range(2, regions.Count - 1);

            randNum1 = Random.Range(0, regions[randNum].Count - 1);

            GridNode node = regions[randNum][randNum1];
            float randNumFloat = Random.Range(0.1f, 1.5f);
            // spawn enemy
            GameObject e = Instantiate(enemy, new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid, 0),
                Quaternion.identity);

            int rNum = Random.Range(1, 100);
            // 50-50 chance for the enemy to be ranged or melee
            if (rNum <= 50)
            {
                e.GetComponent<Enemy>().ranged = true;
                e.GetComponent<Enemy>().maxRange = level + Random.Range(2, 9);
                e.GetComponent<Enemy>().tendency_to_chase_player = 1.5f; // big tendency to chase player
            }
            else
                e.GetComponent<Enemy>().tendency_to_chase_player = randNumFloat; // random tendency to chase player
        }
    }

    static void spawnPickups()
    {
        int healthPickupsNum = Random.Range(1, level + 2);
        for (int i = 0; i < healthPickupsNum; i++)
        {
            int randNum = 1;
            if (regions.Count > 2)
                randNum = Random.Range(2, regions.Count - 1);

            int randNum1 = Random.Range(0, regions[randNum].Count - 1);

            // random node for the pickup to be spawned
            GridNode node = regions[randNum][randNum1];
            while (node == keyNode)
            {
                randNum = Random.Range(2, regions.Count - 1);
                randNum1 = Random.Range(0, regions[randNum].Count - 1);
                node = regions[randNum][randNum1];
            }

            // spawn pickup
            GameObject hP = Instantiate(healthPickup, new Vector3(node.XCoordinateInGrid, node.ZCoordinateInGrid, 0),
                Quaternion.identity);
            hP.GetComponent<HealthPickup>().health = level + Random.Range(8, 15);
        }
    }
}