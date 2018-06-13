using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System.Linq;
using UnityEngine.SceneManagement;

public class CellAuto : MonoBehaviour
{
    public GameObject floor;
    public GameObject wall;
    private Sprite[] floorTiles;

    public int MapWidth = 60;
    public int MapHeight = 60;
    public int PercentAreWalls = 30;
    public int firstLoopIterations;
    public int secondLoopIterations;

    GridGraph myGraphs;
    public List<List<GridNode>> regions;

    public MapHandler mpHandler;

    void Start()
    {
        mpHandler = new MapHandler(MapWidth, MapHeight, PercentAreWalls);

        mpHandler.MakeCaverns(firstLoopIterations, secondLoopIterations); // number of iterations
        //for (int i = 0; i < 30; i++) regions.Add(new List<GridNode>());
        // mpHandler.PrintMap();

        floorTiles = Resources.LoadAll<Sprite>("Map/Floor");
        for (int j = 0; j < mpHandler.MapHeight; j++)
        {
            for (int i = 0; i < mpHandler.MapWidth; i++)
            {
                Vector3 _position = new Vector3((float) (i /*0.16*/), (float) (j /*0.16*/), 0);

                if (mpHandler.Map[i, j] == 0)
                {
                    GameObject floorTile = Instantiate(floor, _position, Quaternion.identity);
                    floorTile.transform.parent = GameObject.FindGameObjectWithTag("Map").transform;
                    floorTile.GetComponent<SpriteRenderer>().sprite =
                        floorTiles[Random.Range(0, floorTiles.Length)];

//                    floorInstance.GetComponent<SpriteRenderer>().siz

//                    Debug.Log("SIZE OF TILE: " + floor.transform.localScale);
                }
                else if (mpHandler.Map[i, j] == 1)
                {
                    GameObject wallTile = Instantiate(wall, _position, Quaternion.identity);
                    wallTile.transform.parent = GameObject.FindGameObjectWithTag("Map").transform;
                }
            }
        }

        AstarPath.active.Scan();
        getGridData(); // can get node data of logic path map - including all separate regions - see GridNode List above

        GameManager.createLevel();

//        AstarPath.active.Scan();
//
//        getGridData(); // can get node data of logic path map - including all separate regions - see GridNode List above
        Debug.Log("REGIONS COUNT: " + regions.Count);
        Debug.Log("NODES COUNT INSIDE REGION[0] CREATE EDGE WALLS: " + regions[0].Count);
//        GameManager.createEdgeWalls();
//        GameManager.
    }


    public void getGridData()
    {
        myGraphs = AstarPath.active.data.gridGraph;
        GridNode[] mynodes = myGraphs.nodes;
        regions = new List<List<GridNode>>();

        // helpful list to add new regions to regions list
        List<int> values = new List<int>();

        // get all regions using gn.Area variable which indicates the index of each region (0,1,2,...) (the first one in regions list is the unwalkable region)
        foreach (GridNode gn in mynodes)
        {
            if (!values.Contains((int) gn.Area)) // if it is a new region
            {
                values.Add((int) gn.Area);
                regions.Add(new List<GridNode>());
            }

            // add node to region
            regions[(int) gn.Area].Add(gn);
        }
    }
}