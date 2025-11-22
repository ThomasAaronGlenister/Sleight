using Deck;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.FilePathAttribute;

public class Chamber : MonoBehaviour
{
    //Size of the chamber, between small, long, tall, and large
    private ChamberSize meChamberSize = ChamberSize.eeDefault;

    //Number of exit points in the chamber
    public int mnNumExits = 0;

    //Number of potential rooms based on room size
    public int mnPotentialRooms = 4;

    //Flag indicating that this room is a start room
    public bool mbStartRoom = false;

    //Tilemaps
    Tilemap mcChamberWallsTilemap;
    Tilemap mcChamberGroundTilemap;
    Tilemap mcEnemySpawnPointTilemap;

    //List of enemy spawn points
    List<Vector2> macSpawnPoints = new List<Vector2>();

    //Map of Exits and Tiles to remove
    Dictionary<ChamberExits, List<Vector2>> macChamberExitDictionary 
        = new Dictionary<ChamberExits, List<Vector2>>();

    //Map of Exits the chambers that they lead to
    Dictionary<ChamberExits, GameObject> macChamberGraph
        = new Dictionary<ChamberExits, GameObject>();

    //List of available exits based on this chambers size
    List<ChamberExits> maeAvailableExits = new List<ChamberExits>();

    //List of X,Y coordinates this chamber covers
    Dictionary<ChamberExits, UnityEngine.Vector2> macChamberGrid
        = new Dictionary<ChamberExits, UnityEngine.Vector2>();

    //List of Exit/Entrance connections between the chambers
    Dictionary<ChamberExits, ChamberExits> macChamberConnections
        = new Dictionary<ChamberExits, ChamberExits>();

    List<GameObject> macMapTiles = new List<GameObject>();

    //EnemyPrefab
    public GameObject mcEnemyPrefab;

    //Enemy Attributes sets
    EnemyAttributesLoader mcEnemyAttributesLoader;


    /*
     * METHOD: Initialize Chamber attributes.
     */
    public void InitializeChamber()
    {
        Transform lcWallTransform = transform.Find("Walls");

        if (lcWallTransform != null)
        {
            mcChamberWallsTilemap = lcWallTransform.GetComponent<Tilemap>();
        }
        else
        {
            Debug.Log("Chamber Does not include wall component");
        }

        Transform lcGroundTransform = transform.Find("Ground");

        if (lcWallTransform != null)
        {
            mcChamberGroundTilemap = lcGroundTransform.GetComponent<Tilemap>();
        }
        else
        {
            Debug.Log("Chamber Does not include ground component");
        }

        Transform lcSpawnPointTransform = transform.Find("EnemySpawnPoints");

        //If chamber contains 
        if (lcSpawnPointTransform != null)
        {
            mcEnemySpawnPointTilemap = lcSpawnPointTransform.GetComponent<Tilemap>();

            Debug.Log("Found Spawn Point Transform");

            //Collect World coordinates from spawn points on map
            for (int x = mcEnemySpawnPointTilemap.cellBounds.xMin; x < mcEnemySpawnPointTilemap.cellBounds.xMax; x++)
            {
                for (int y = mcEnemySpawnPointTilemap.cellBounds.yMin; y < mcEnemySpawnPointTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int localLocation = new Vector3Int(
                        x: x,
                        y: y,
                        z: 0);

                    Vector3 location = mcEnemySpawnPointTilemap.CellToWorld(localLocation);
                    if (mcEnemySpawnPointTilemap.HasTile(localLocation))
                    {
                        Debug.Log("Add Spawn Point X: " + location.x + " Y: " + location.y);
                        macSpawnPoints.Add(location);
                    }
                }
            }
        }


        //Set chamber size based on tag
        //Start Room only has left/right exit
        if (mbStartRoom)
        {
            mnPotentialRooms = 2;

            macChamberGrid[ChamberExits.eeRight] = new Vector2(0, 0);
            macChamberGrid[ChamberExits.eeLeft] = new Vector2(0, 0);
        }
        else if (gameObject.CompareTag("LargeRoom"))
        {
            meChamberSize = ChamberSize.eeLarge;
            mnPotentialRooms = 8;
        }
        else if (gameObject.CompareTag("LongRoom"))
        {
            meChamberSize = ChamberSize.eeLong;
            mnPotentialRooms = 6;
        }
        else if (gameObject.CompareTag("TallRoom"))
        {
            meChamberSize = ChamberSize.eeTall;
            mnPotentialRooms = 6;
        }
        else if (gameObject.CompareTag("DefaultRoom"))
        {
            meChamberSize = ChamberSize.eeDefault;
            mnPotentialRooms = 4;
        }

        //Set list of available exits
        InitializeAvailableExits();

        //Add list of wall tiles that may be converted to exits
        GenerateExitTileLists();

        mcEnemyAttributesLoader = new EnemyAttributesLoader();
    }

    public void PopulateWithEnemies()
    {
        //If any designated spawn points exist in this chamber
        if(macSpawnPoints.Count != 0)
        {
            Vector2 lcSpawnPoint = macSpawnPoints[UnityEngine.Random.Range(0, macSpawnPoints.Count)];

            GameObject lcEnemy = Instantiate(mcEnemyPrefab, lcSpawnPoint, Quaternion.identity);
            lcEnemy.GetComponent<EnemyAI>().SetEnemyAttributes(mcEnemyAttributesLoader.GetBaseEnemyAttributes(3));
            lcEnemy.transform.SetParent(this.transform);

            macSpawnPoints.Remove(lcSpawnPoint);
        }

        /*
        GameObject lcEnemy2 = Instantiate(mcEnemyPrefab, new Vector3(2, 0, 0), Quaternion.identity);
        lcEnemy2.GetComponent<EnemyAI>().SetEnemyAttributes(mcEnemyAttributesLoader.GetBaseEnemyAttributes(0));
        lcEnemy2.transform.SetParent(this.transform);

        GameObject lcEnemy3 = Instantiate(mcEnemyPrefab, new Vector3(-5, 0, 0), Quaternion.identity);
        lcEnemy.GetComponent<EnemyAI>().SetEnemyAttributes(mcEnemyAttributesLoader.GetBaseEnemyAttributes(0));
        lcEnemy.transform.SetParent(this.transform);
        */
    }

    /**
     * METHOD: Sets the list of available exits to this chamber
     */
    private void InitializeAvailableExits()
    {
        //Start chamber only has left and right exits
        if(mbStartRoom)
        {
            maeAvailableExits.Add(ChamberExits.eeLeft);
            maeAvailableExits.Add(ChamberExits.eeRight);
        }
        else
        {
            switch (meChamberSize)
            {
                case ChamberSize.eeLarge:
                    maeAvailableExits.Add(ChamberExits.eeLeft);
                    maeAvailableExits.Add(ChamberExits.eeRight);
                    maeAvailableExits.Add(ChamberExits.eeTopLeft);
                    maeAvailableExits.Add(ChamberExits.eeTopRight);
                    maeAvailableExits.Add(ChamberExits.eeMiddleLeft);
                    maeAvailableExits.Add(ChamberExits.eeMiddleRight);
                    maeAvailableExits.Add(ChamberExits.eeBottomLeft);
                    maeAvailableExits.Add(ChamberExits.eeBottomRight);
                    break;
                case ChamberSize.eeLong:
                    maeAvailableExits.Add(ChamberExits.eeLeft);
                    maeAvailableExits.Add(ChamberExits.eeRight);
                    maeAvailableExits.Add(ChamberExits.eeTopLeft);
                    maeAvailableExits.Add(ChamberExits.eeTopRight);
                    maeAvailableExits.Add(ChamberExits.eeBottomLeft);
                    maeAvailableExits.Add(ChamberExits.eeBottomRight);
                    break;
                case ChamberSize.eeTall:
                    maeAvailableExits.Add(ChamberExits.eeLeft);
                    maeAvailableExits.Add(ChamberExits.eeRight);
                    maeAvailableExits.Add(ChamberExits.eeTop);
                    maeAvailableExits.Add(ChamberExits.eeBottom);
                    maeAvailableExits.Add(ChamberExits.eeMiddleLeft);
                    maeAvailableExits.Add(ChamberExits.eeMiddleRight);
                    break;
                case ChamberSize.eeDefault:
                    maeAvailableExits.Add(ChamberExits.eeLeft);
                    maeAvailableExits.Add(ChamberExits.eeRight);
                    maeAvailableExits.Add(ChamberExits.eeTop);
                    maeAvailableExits.Add(ChamberExits.eeBottom);
                    break;

            }
        }
    }// END InitializeAvailableExits

    /*
     * METHOD: Returns the number of rooms this chamber may have
     */
    public int GetNumPotentialRooms()
    {
        return mnPotentialRooms;
    }

    /**
     * METHOD: Creates Exit and connection to chamber
     */
    public bool CreateExit(ChamberExits peChamberExit, GameObject pcConnectedChamber)
    {
        bool lbExitCreated = false;

        if(macChamberExitDictionary[peChamberExit].Count != 0 && maeAvailableExits.Contains(peChamberExit))
        {
            lbExitCreated = true;

            //Block this exit from being used by another connection
            BlockExit(peChamberExit);

            //Add Chamber connection
            macChamberGraph.Add(peChamberExit, pcConnectedChamber);

            //clear the exit tiles
            foreach (Vector2 lcExit in macChamberExitDictionary[peChamberExit])
            {
                //Ground Tiles must be removed if Chamber exit is bottom or top
                if(peChamberExit == ChamberExits.eeBottom || 
                    peChamberExit == ChamberExits.eeBottomLeft || 
                    peChamberExit == ChamberExits.eeBottomRight ||
                    peChamberExit == ChamberExits.eeTop ||
                    peChamberExit == ChamberExits.eeTopLeft ||
                    peChamberExit == ChamberExits.eeTopRight)
                {
                    mcChamberGroundTilemap.SetTile(new Vector3Int((int)lcExit.x, (int)lcExit.y, 0), null);
                }
                //Else Wall tiles will be removed
                else
                {
                    mcChamberWallsTilemap.SetTile(new Vector3Int((int)lcExit.x, (int)lcExit.y, 0), null);
                }
            }

        }
        else
        {
            Debug.Log("Chamber " + gameObject.ToString() + " Failed to open exit : " + peChamberExit);
        }

            return lbExitCreated;
    }

    public void ConnectExits(ChamberExits peChamberExit, ChamberExits peConnectedChamberEntrance)
    {
        macChamberConnections.Add(peChamberExit, peConnectedChamberEntrance);
    }

    public ChamberExits GetConnectedChamberEntrance(ChamberExits peChamberExit)
    {
        ChamberExits leNextChamberEntrance = ChamberExits.eeNone;
        if(macChamberConnections.ContainsKey(peChamberExit))
        {
            leNextChamberEntrance = macChamberConnections[peChamberExit];   
        }

        return leNextChamberEntrance;
    }

    /**
     * METHOD: Returns the chamber at the opposite of this chambers exit
     */
    public GameObject GetConnectedChamber(ChamberExits peChamberExit)
    {
        if(macChamberGraph.ContainsKey(peChamberExit))
        {
            return macChamberGraph[peChamberExit];
        }
        else
        {
            Debug.Log("No Connection exists between Chamber and exit: " + peChamberExit);
            return null;
        }
    }

    public void BlockExit(ChamberExits peChamberExit)
    {
        //Remove Available exits from this chamber that may overlap with adjacent chamber
        maeAvailableExits.Remove(peChamberExit);
    }

    /**
     * Gets a random valid exit to this chamber based on provided entrance
     */
    public ChamberExits GetValidExit(ChamberExits peChamberEntrance)
    {
        ChamberExits lbExit = ChamberExits.eeNone;

        List<ChamberExits> lacExits = new List<ChamberExits>();

        //If entrance is none, pick random exit from all potential exits
        if(peChamberEntrance == ChamberExits.eeNone)
        {
            lacExits = maeAvailableExits;
        }
        //Check Whether new entrance is Top left or right
        else if (peChamberEntrance == ChamberExits.eeLeft || peChamberEntrance == ChamberExits.eeMiddleLeft)
        {
            if(maeAvailableExits.Contains(ChamberExits.eeRight))
            {
                lacExits.Add(ChamberExits.eeRight);
            }

            if (maeAvailableExits.Contains(ChamberExits.eeMiddleRight))
            {
                lacExits.Add(ChamberExits.eeMiddleRight);
            }
        }
        else if(peChamberEntrance == ChamberExits.eeRight || peChamberEntrance == ChamberExits.eeMiddleRight)
        {
            if (maeAvailableExits.Contains(ChamberExits.eeLeft))
            {
                lacExits.Add(ChamberExits.eeLeft);
            }

            if (maeAvailableExits.Contains(ChamberExits.eeMiddleLeft))
            {
                lacExits.Add(ChamberExits.eeMiddleLeft);
            }
        }
        else if (peChamberEntrance == ChamberExits.eeBottom || 
            peChamberEntrance == ChamberExits.eeBottomRight || peChamberEntrance == ChamberExits.eeBottomLeft)
        {
            if (maeAvailableExits.Contains(ChamberExits.eeTop))
            {
                lacExits.Add(ChamberExits.eeTop);
            }

            if (maeAvailableExits.Contains(ChamberExits.eeTopLeft))
            {
                lacExits.Add(ChamberExits.eeTopLeft);
            }

            if (maeAvailableExits.Contains(ChamberExits.eeTopRight))
            {
                lacExits.Add(ChamberExits.eeTopRight);
            }
        }
        else if (peChamberEntrance == ChamberExits.eeTop || 
            peChamberEntrance == ChamberExits.eeTopLeft || peChamberEntrance == ChamberExits.eeTopRight)
        {
            if (maeAvailableExits.Contains(ChamberExits.eeBottom))
            {
                lacExits.Add(ChamberExits.eeBottom);
            }

            if (maeAvailableExits.Contains(ChamberExits.eeBottomLeft))
            {
                lacExits.Add(ChamberExits.eeBottomLeft);
            }

            if (maeAvailableExits.Contains(ChamberExits.eeBottomRight))
            {
                lacExits.Add(ChamberExits.eeBottomRight);
            }
        }

        if (lacExits.Count != 0)
        {
            lbExit = lacExits[UnityEngine.Random.Range(0, lacExits.Count)];
        }

        return lbExit;
    }


    private void GenerateExitTileLists()
    {

        foreach (ChamberExits leExits in Enum.GetValues(typeof(ChamberExits)))
        {
            macChamberExitDictionary.Add(leExits, new List<Vector2>());

            switch (leExits)
            {
                case ChamberExits.eeBottom:
                    if (meChamberSize == ChamberSize.eeDefault || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-3, -4));
                    }
                    break;
                case ChamberExits.eeLeft:
                    macChamberExitDictionary[leExits].Add(new Vector2(-10, -2));
                    macChamberExitDictionary[leExits].Add(new Vector2(-10, -3));
                    break;
                case ChamberExits.eeMiddleLeft:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, 5));
                    }
                    break;
                case ChamberExits.eeTopLeft:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                    }
                    break;
                case ChamberExits.eeTop:
                    if (meChamberSize == ChamberSize.eeDefault || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, 3 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, 3 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-3, 3 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                    }
                    break;
                case ChamberExits.eeTopRight:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(3, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(4, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                    }
                    break;
                case ChamberExits.eeRight:
                    if (meChamberSize == ChamberSize.eeDefault || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(2, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(2, -3));
                    }
                    else
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(9, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(9, -3));
                    }
                    break;
                case ChamberExits.eeMiddleRight:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0), 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0), 5));
                    }
                    break;
                case ChamberExits.eeBottomLeft:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -4));
                    }
                    break;
                case ChamberExits.eeBottomRight:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(3, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(4, -4));
                    }
                    break;

            }
        }
    } // End GenerateExitTileLists

    /**
     * METHOD: Returns the size of this chamber
     */
    public ChamberSize GetChamberSize()
    {
        return meChamberSize;
    }

    public Vector2 GetExitPoint(ChamberExits peChamberExit)
    {
        Vector2 lcExitPoint = new();

        if(macChamberGrid.ContainsKey(peChamberExit))
        {
            lcExitPoint = macChamberGrid[peChamberExit];
        }

        return lcExitPoint;
    }

    public void AddGridPoints(List<Vector2> pacChamberPoints)
    {
        if(pacChamberPoints.Count != 0)
        {
            if (meChamberSize == ChamberSize.eeDefault && pacChamberPoints.Count == 1)
            {
                macChamberGrid[ChamberExits.eeBottom] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeRight] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeLeft] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeTop] = pacChamberPoints[0];
            }
            else if(meChamberSize == ChamberSize.eeLong && pacChamberPoints.Count == 2)
            {
                macChamberGrid[ChamberExits.eeBottomRight] = pacChamberPoints[1];
                macChamberGrid[ChamberExits.eeBottomLeft] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeRight] = pacChamberPoints[1];
                macChamberGrid[ChamberExits.eeLeft] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeTopRight] = pacChamberPoints[1];
                macChamberGrid[ChamberExits.eeTopLeft] = pacChamberPoints[0];
            }
            else if (meChamberSize == ChamberSize.eeTall && pacChamberPoints.Count == 2)
            {
                macChamberGrid[ChamberExits.eeBottom] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeRight] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeLeft] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeMiddleRight] = pacChamberPoints[1];
                macChamberGrid[ChamberExits.eeMiddleLeft] = pacChamberPoints[1];
                macChamberGrid[ChamberExits.eeTop] = pacChamberPoints[1];
            }
            else if (meChamberSize == ChamberSize.eeLarge && pacChamberPoints.Count == 4)
            {
                macChamberGrid[ChamberExits.eeBottomLeft] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeBottomRight] = pacChamberPoints[1];
                macChamberGrid[ChamberExits.eeRight] = pacChamberPoints[1];
                macChamberGrid[ChamberExits.eeLeft] = pacChamberPoints[0];
                macChamberGrid[ChamberExits.eeMiddleRight] = pacChamberPoints[3];
                macChamberGrid[ChamberExits.eeMiddleLeft] = pacChamberPoints[2];
                macChamberGrid[ChamberExits.eeTopLeft] = pacChamberPoints[2];
                macChamberGrid[ChamberExits.eeTopRight] = pacChamberPoints[3];
            }
            else
            {
                Debug.Log("Chamber points Count error " + pacChamberPoints.Count);
            }
        }
        else
        {
            Debug.Log("Chamber Points size is 0");
        }
    }

    public void AddMapIcons(GameObject pcMapIcon)
    {
        macMapTiles.Add(pcMapIcon);
    }

    public void Enter()
    {
        foreach(GameObject lcMapTile in macMapTiles)
        {
            if(!lcMapTile.activeSelf)
            {
                lcMapTile.SetActive(true);
            }

            lcMapTile.GetComponent<CanvasGroup>().alpha = 1.0f;
        }
    }

    public void Exit()
    {
        foreach (GameObject lcMapTile in macMapTiles)
        {
            lcMapTile.GetComponent<CanvasGroup>().alpha = 0.35f;
        }
    }
}
