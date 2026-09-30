using Deck;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using static UnityEditor.FilePathAttribute;

public class Chamber : MonoBehaviour
{
    //Size of the chamber, between small, long, tall, and large
    public ChamberSize meChamberSize = ChamberSize.eeDefault;

    private Vector2 mcChamberDimensions = Vector2.zero;

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
    Tilemap mcTreasureSpawnPointTilemap;
    Tilemap mcDestructibleSpawnPointTilemap;
    Tilemap mcExitDoorSpawnPointTilemap;

    Tilemap mcChamberPassThroughPlatformsTilemap;

    PlatformEffector2D mcPassThroughPlatformEffector;

    //List of enemy spawn points
    private List<Vector2> macEnemySpawnPoints = new List<Vector2>();

    //List of Treasure spawn points
    private List<Vector2> macTreasureSpawnPoints = new List<Vector2>();

    //List of destructible spawn points
    private List<Vector2> macDestructibleSpawnPoints = new List<Vector2>();

    //List of Exit Door spawn points
    private List<Vector2> macExitDoorSpawnPoints = new List<Vector2>();

    //Map of Exits and Tiles to remove
    private Dictionary<ChamberExits, List<Vector2>> macChamberExitDictionary 
        = new Dictionary<ChamberExits, List<Vector2>>();

    //Map of Exits the chambers that they lead to
    Dictionary<ChamberExits, GameObject> macChamberGraph
        = new Dictionary<ChamberExits, GameObject>();

    //List of available exits based on this chambers size
    public List<ChamberExits> maeSetAvailableExits = new List<ChamberExits>();

    private List<ChamberExits> maeAvailableExits;

    //List of X,Y coordinates this chamber covers
    private Dictionary<ChamberExits, UnityEngine.Vector2> macChamberGrid
        = new Dictionary<ChamberExits, UnityEngine.Vector2>();

    //List of Exit/Entrance connections between the chambers
    private Dictionary<ChamberExits, ChamberExits> macChamberConnections
        = new Dictionary<ChamberExits, ChamberExits>();

    private List<GameObject> macMapTiles = new List<GameObject>();

    //List of Darkened Tile map sets
    private GameObject[] macExits = new GameObject[(int)ChamberExits.eeNone];

    //EnemyPrefab
    [SerializeField] private GameObject mcEnemyPrefab;

    //Treasure chest prefab
    [SerializeField] private GameObject mcTreasurePrefab;

    //Treasure chest prefab
    [SerializeField] private GameObject mcDestructiblePrefab;

    //Doorway prefab
    [SerializeField] private GameObject mcDoorwayPrefab;

    //Enemy Attributes sets
    EnemyAttributesSet mcEnemyAttributesLoader;

    Dictionary<ChamberExits, List<ChamberExits>> macChamberExitOptions
        = new Dictionary<ChamberExits, List<ChamberExits>>();

    public GameObject mcChamberCameraCollider;
    private GameObject mcChamberCameraColliderCopy;

    private GameObject mcDoorway = null;
    public bool mbHasDoorway = false;
    private Vector3 mcDoorPosition = Vector3.zero;

    //Perpendicular chamber linked to this chamber
    public Chamber mcSideChamber; 

    //Returns a the chamber prependicular to this chamber, accessable via door to chamber on different axis.
    public Chamber GetSideChamber()
    {
        return mcSideChamber;
    }

    public void SetSideChamber(Chamber pcSideChamber)
    {
        mcSideChamber = pcSideChamber;
    }

    public bool HasDoorway()
    {
        return mbHasDoorway;
    }

    public void SetDoorway(bool pbHasDoorway)
    {
        mbHasDoorway = pbHasDoorway;
    }

    public Vector3 GetDoorPosition()
    {
        Vector3 lcDoorPos = Vector3.zero;
        if(mcDoorway)
        {
            lcDoorPos = new Vector3(mcDoorway.transform.position.x, mcDoorway.transform.position.y - 1.15f, mcDoorway.transform.position.z);
        }
        return lcDoorPos;
    }

    public void CreateDoorway()
    {
        mbHasDoorway = true;

        if(macExitDoorSpawnPoints.Count != 0)
        {
            //Instantiate doorway at random door spawn position
            Vector2 lcSpawnPoint = macExitDoorSpawnPoints[UnityEngine.Random.Range(0, macExitDoorSpawnPoints.Count)];

            Vector2 DoorPoint = new Vector2(lcSpawnPoint.x, (lcSpawnPoint.y + 1.15f));

            mcDoorway = Instantiate(mcDoorwayPrefab, DoorPoint, Quaternion.identity);
            mcDoorway.transform.SetParent(this.transform);

            macExitDoorSpawnPoints.Remove(lcSpawnPoint);
        }
        else
        {
            Debug.Log("Exit Door Spawn points = 0");
        }
    }

    //Sets all children game objects of this chambers color alpha to provided value.
    public void SetChamberTransparency(float pfNormalizedAlpha, Transform pcParent = null)
    {
        Color lcAlpha = Color.white;
        Transform lcParent = (pcParent != null) ? pcParent : transform;

        foreach (Transform lcChild in lcParent)
        {
            if(lcChild.gameObject.name != "Door")
            {
                //Recursively set children opacity
                SetChamberTransparency(pfNormalizedAlpha, lcChild);

                //Try to get color components of this object
                if (lcChild.gameObject.TryGetComponent<Tilemap>(out Tilemap lcTilemap))
                {
                    lcAlpha = lcTilemap.color;
                    lcAlpha.a = pfNormalizedAlpha;
                    lcTilemap.color = lcAlpha;
                }

                if (lcChild.gameObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer lcSpriteRenderer))
                {
                    lcAlpha = lcSpriteRenderer.color;
                    lcAlpha.a = pfNormalizedAlpha;
                    lcSpriteRenderer.color = lcAlpha;
                }

                if (lcChild.gameObject.TryGetComponent<Light2D>(out Light2D lcLight))
                {
                    lcAlpha = lcLight.color;
                    lcAlpha.a = pfNormalizedAlpha;
                    lcLight.color = lcAlpha;
                }
            }
        }
    }

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

        Transform lcPassThroughPlatformsTransform = transform.Find("PassThroughPlatforms");

        if (lcPassThroughPlatformsTransform != null)
        {
            mcChamberPassThroughPlatformsTilemap = lcPassThroughPlatformsTransform.GetComponent<Tilemap>();
            mcPassThroughPlatformEffector = lcPassThroughPlatformsTransform.GetComponent<PlatformEffector2D>();
        }
        else
        {
            Debug.Log("Chamber Does not include pass through platforms component");
        }

        //Get Exit Cover tile sets
        Transform lcExitTileSets = transform.Find("Exits");
        if (lcExitTileSets != null)
        {
            int lnExitIndex = 0;

            //Tile covers are ordered in the same as the chamber exits enumeration
            foreach (Transform child in lcExitTileSets)
            {
                macExits[lnExitIndex] = child.gameObject;
                macExits[lnExitIndex].SetActive(false);
                lnExitIndex++;
            }
        }
        else
        {
            Debug.Log("ExitTiles not found");
        }

        mcChamberCameraColliderCopy = Instantiate(mcChamberCameraCollider, mcChamberCameraCollider.transform.position, mcChamberCameraCollider.transform.rotation);

        transform.gameObject.SetActive(true);

        if (!mbStartRoom)
        {
            //transform.gameObject.SetActive(false);
        }

        //Set chamber size based on tag
        //Start Room only has left/right exit
        if (mbStartRoom)
        { 
            macChamberGrid[ChamberExits.eeRight] = new Vector2(0, 0);
            macChamberGrid[ChamberExits.eeLeft] = new Vector2(0, 0);
        }

        switch (meChamberSize)
        {
            case ChamberSize.eeDefault:
                mcChamberDimensions = new Vector2(10, 7);
                break;
            case ChamberSize.eeLong:
                mcChamberDimensions = new Vector2(20, 7);
                break;
            case ChamberSize.eeTall:
                mcChamberDimensions = new Vector2(10, 14);
                break;
            case ChamberSize.eeLarge:
                mcChamberDimensions = new Vector2(20, 14);
                break;
        }


        mnPotentialRooms = maeSetAvailableExits.Count;

        maeAvailableExits = new List<ChamberExits>(maeSetAvailableExits);

        SetUpChamberExitOptions();

        //Add list of wall tiles that may be converted to exits
        GenerateExitTileLists();
    }

    public void SetUpChamberExitOptions()
    {
        //Dictionary of exits and lists of entrances that the exit may connect to
        macChamberExitOptions.Add(ChamberExits.eeLeft, new List<ChamberExits> { ChamberExits.eeRight, ChamberExits.eeMiddleRight });
        macChamberExitOptions.Add(ChamberExits.eeRight, new List<ChamberExits> { ChamberExits.eeLeft, ChamberExits.eeMiddleLeft });
        macChamberExitOptions.Add(ChamberExits.eeMiddleLeft, new List<ChamberExits> { ChamberExits.eeRight, ChamberExits.eeMiddleRight });
        macChamberExitOptions.Add(ChamberExits.eeMiddleRight, new List<ChamberExits> { ChamberExits.eeLeft, ChamberExits.eeMiddleLeft });
        macChamberExitOptions.Add(ChamberExits.eeBottomLeft, new List<ChamberExits> { ChamberExits.eeTop, ChamberExits.eeTopRight, ChamberExits.eeTopLeft });
        macChamberExitOptions.Add(ChamberExits.eeBottomRight, new List<ChamberExits> { ChamberExits.eeTop, ChamberExits.eeTopRight, ChamberExits.eeTopLeft });
        macChamberExitOptions.Add(ChamberExits.eeBottom, new List<ChamberExits> { ChamberExits.eeTop, ChamberExits.eeTopRight, ChamberExits.eeTopLeft });
        macChamberExitOptions.Add(ChamberExits.eeTopLeft, new List<ChamberExits> { ChamberExits.eeBottom, ChamberExits.eeBottomRight, ChamberExits.eeBottomLeft });
        macChamberExitOptions.Add(ChamberExits.eeTopRight, new List<ChamberExits> { ChamberExits.eeBottom, ChamberExits.eeBottomRight, ChamberExits.eeBottomLeft });
        macChamberExitOptions.Add(ChamberExits.eeTop, new List<ChamberExits> { ChamberExits.eeBottom, ChamberExits.eeBottomRight, ChamberExits.eeBottomLeft });
    }

    public void RepositionChamber(Vector2 pcNewPosition)
    {
        this.transform.position = pcNewPosition;

        //Determine Enemy spawn points
        DetectEnemySpawnPoints();

        //Detect any points where a door may spawn
        DetectExitDoorSpawnPoints();
    }

    public GameObject GetCameraCollider()
    {
        return mcChamberCameraColliderCopy;
    }

    private void OnTriggerEnter2D(Collider2D pcCollision)
    {
        if (pcCollision.name == "Player")
        {
            Debug.Log("Player Entered Chamber: " + transform.name);

            //Set Players current chamber to this chamber
            pcCollision.GetComponent<PlayerMovement>().ChangeChamber(this);

            SetChamberToActive();
        }
    }

    //Enables all adjacent chambers to this chamber
    public void SetChamberToActive()
    {
        foreach(var lcExitChamberKeyValue in macChamberGraph)
        {
            //Check if chamber on each exit is active
            if (!lcExitChamberKeyValue.Value.activeSelf)
            {
                //Set the adjacent chamber to active
                lcExitChamberKeyValue.Value.SetActive(true);

                Debug.Log("Enable chamber: " + lcExitChamberKeyValue.Value.name);
            }

            //Disable connected chambers adjacent chambers
            lcExitChamberKeyValue.Value.GetComponent<Chamber>().DisableAdjacentChambers(this.gameObject);
        }
    }

    //Disables all adjacent chambers aside from the provided one
    public void DisableAdjacentChambers(GameObject pcCurrentChamber)
    {
        //Set chambers not directly connected to this chamber to inactive
        foreach (var lcExitChamberKeyValue in macChamberGraph)
        {
            //Check if chamber on each exit is active
            if (lcExitChamberKeyValue.Value.activeSelf && lcExitChamberKeyValue.Value != pcCurrentChamber)
            {
                Debug.Log("Disable chamber: " + lcExitChamberKeyValue.Value.name);

                //lcExitChamberKeyValue.Value.SetActive(false);
            }
        }
    }

    public void PassThroughPlatform()
    {
        StartCoroutine(PassThroughPlatformCoroutine());
    }

    IEnumerator PassThroughPlatformCoroutine()
    {
        int BaseMask = mcPassThroughPlatformEffector.colliderMask;

        //Mask off player so they can drop from platforms
        mcPassThroughPlatformEffector.colliderMask = 258047;
        
        //Wait and allow player to use platform again
        yield return new WaitForSeconds(0.4f);
        mcPassThroughPlatformEffector.colliderMask = BaseMask;
    }

    //Method to determine if the chamber has any designated enemy spawn points
    private void DetectEnemySpawnPoints()
    {
        Transform lcSpawnPointTransform = transform.Find("SpawnPoints/EnemySpawnPoints");

        //If chamber contains enemy spawn tiles
        if (lcSpawnPointTransform != null)
        {
            mcEnemySpawnPointTilemap = lcSpawnPointTransform.GetComponent<Tilemap>();

            //Debug.Log("Found Spawn Point Transform");

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
                        location.y += 0.3f;
                        macEnemySpawnPoints.Add(location);
                        //Debug.Log("EnemySpawnPoint: X: " + location.x + " Y: " + location.y);
                    }
                }
            }
        }
    }

    //Method to determine if the chamber has any designated enemy spawn points
    private void DetectDestructibleSpawnPoints()
    {
        Transform lcSpawnPointTransform = transform.Find("SpawnPoints/DestructibleSpawnPoints");

        //If chamber contains enemy spawn tiles
        if (lcSpawnPointTransform != null)
        {
            mcDestructibleSpawnPointTilemap = lcSpawnPointTransform.GetComponent<Tilemap>();

            //Debug.Log("Found Spawn Point Transform");

            //Collect World coordinates from spawn points on map
            for (int x = mcDestructibleSpawnPointTilemap.cellBounds.xMin; x < mcDestructibleSpawnPointTilemap.cellBounds.xMax; x++)
            {
                for (int y = mcDestructibleSpawnPointTilemap.cellBounds.yMin; y < mcDestructibleSpawnPointTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int localLocation = new Vector3Int(
                        x: x,
                        y: y,
                        z: 0);

                    Vector3 location = mcDestructibleSpawnPointTilemap.CellToWorld(localLocation);
                    if (mcDestructibleSpawnPointTilemap.HasTile(localLocation))
                    {
                        location.y += 0.3f;
                        macDestructibleSpawnPoints.Add(location);
                        //Debug.Log("EnemySpawnPoint: X: " + location.x + " Y: " + location.y);
                    }
                }
            }
        }
    }

    //Method to determine if the chamber has any designated treasure spawn points
    private void DetectTreasureSpawnPoints()
    {
        Transform lcSpawnPointTransform = transform.Find("SpawnPoints/TreasureSpawnPoints");

        //If chamber contains treasure spawn tiles
        if (lcSpawnPointTransform != null)
        {
            mcTreasureSpawnPointTilemap = lcSpawnPointTransform.GetComponent<Tilemap>();

            //Debug.Log("Found Treasure Spawn Point Transform");

            //Collect World coordinates from spawn points on map
            for (int x = mcTreasureSpawnPointTilemap.cellBounds.xMin; x < mcTreasureSpawnPointTilemap.cellBounds.xMax; x++)
            {
                for (int y = mcTreasureSpawnPointTilemap.cellBounds.yMin; y < mcTreasureSpawnPointTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int localLocation = new Vector3Int(
                        x: x,
                        y: y,
                        z: 0);

                    Vector3 location = mcTreasureSpawnPointTilemap.CellToWorld(localLocation);
                    if (mcTreasureSpawnPointTilemap.HasTile(localLocation))
                    {
                        macTreasureSpawnPoints.Add(location);
                        //Debug.Log("TreasureSpawnPoint: X: " + location.x + " Y: " + location.y);
                    }
                }
            }
        }
    }

    //Method to determine if the chamber has any designated Door spawn points
    private void DetectExitDoorSpawnPoints()
    {
        Transform lcSpawnPointTransform = transform.Find("SpawnPoints/ExitDoorSpawnPoints");

        Debug.Log("DetectDoors on Chamber: " + transform.gameObject.name);

        //If chamber contains treasure spawn tiles
        if (lcSpawnPointTransform != null)
        {
            mcExitDoorSpawnPointTilemap = lcSpawnPointTransform.GetComponent<Tilemap>();
            mcExitDoorSpawnPointTilemap.CompressBounds();

            //Collect World coordinates from spawn points on map
            for (int x = mcExitDoorSpawnPointTilemap.cellBounds.xMin; x < mcExitDoorSpawnPointTilemap.cellBounds.xMax; x++)
            {
                for (int y = mcExitDoorSpawnPointTilemap.cellBounds.yMin; y < mcExitDoorSpawnPointTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int localLocation = new Vector3Int(
                        x: x,
                        y: y,
                        z: 0);

                    Vector3 location = mcExitDoorSpawnPointTilemap.CellToWorld(localLocation);
                    if (mcExitDoorSpawnPointTilemap.HasTile(localLocation))
                    {
                        macExitDoorSpawnPoints.Add(location);
                        Debug.Log("ExitDoorSpawnPoint: X: " + location.x + " Y: " + location.y);
                    }
                }
            }
        }
    }

    //Returns the list of spawn points for enemies in this chamber
    public List<Vector2> GetEnemySpawnPoints()
    {
        return macEnemySpawnPoints;
    }

    //Creates treasure chests in the chamber
    public void PopulateWithTreasure()
    {
        //Determine Treasure spawn points
        DetectTreasureSpawnPoints();

        int lnNumDestructibles = UnityEngine.Random.Range(0, macTreasureSpawnPoints.Count);

        for(int lnAddDestructible = 0; lnAddDestructible < lnNumDestructibles; lnAddDestructible++)
        {
            //If any designated spawn points exist in this chamber
            if (macTreasureSpawnPoints.Count != 0)
            {
                Vector2 lcSpawnPoint = macTreasureSpawnPoints[UnityEngine.Random.Range(0, macTreasureSpawnPoints.Count)];

                lcSpawnPoint.y += .5f;

                GameObject lcTreasureChest = Instantiate(mcTreasurePrefab, lcSpawnPoint, Quaternion.identity);
                lcTreasureChest.transform.SetParent(this.transform);

                macTreasureSpawnPoints.Remove(lcSpawnPoint);
            }
        }
    }

    public void PopulateWithDestructibles()
    {
        DetectDestructibleSpawnPoints();

        if(macDestructibleSpawnPoints.Count != 0)
        {
            Vector2 lcSpawnPoint = macDestructibleSpawnPoints[UnityEngine.Random.Range(0, macTreasureSpawnPoints.Count)];

            lcSpawnPoint.y += .5f;

            GameObject lcDestructible = Instantiate(mcDestructiblePrefab, lcSpawnPoint, Quaternion.identity);
            lcDestructible.transform.SetParent(this.transform);

            macDestructibleSpawnPoints.Remove(lcSpawnPoint);
        }
    }

    /*
     * METHOD: Returns the number of rooms this chamber may have
     */
    public int GetNumPotentialRooms()
    {
        return maeAvailableExits.Count;
    }

    //Return the list of available exits this chamber has
    public List<ChamberExits> GetAvailableExits()
    {
        return maeAvailableExits;
    }

    /**
     * METHOD: Creates Exit and connection to chamber
     */
    public bool CreateExit(ChamberExits peChamberExit, GameObject pcConnectedChamber = null)
    {
        bool lbExitCreated = false;

        if(macChamberExitDictionary[peChamberExit].Count != 0 && maeAvailableExits.Contains(peChamberExit))
        {
            lbExitCreated = true;

            //Block this exit from being used by another connection
            BlockExit(peChamberExit);

            if(pcConnectedChamber)
            {
                //Add Chamber connection
                macChamberGraph.Add(peChamberExit, pcConnectedChamber);
            }

            //Clear Covers for exits
            macExits[(int)peChamberExit].SetActive(true);

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
                    mcChamberWallsTilemap.SetTile(new Vector3Int((int)lcExit.x, (int)lcExit.y, 0), null);
                }
                //Else Wall tiles will be removed
                else
                {
                    mcChamberGroundTilemap.SetTile(new Vector3Int((int)lcExit.x, (int)lcExit.y, 0), null);
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
    public ChamberExits GetValidExit(ChamberExits peChamberEntrance, List<ChamberExits> paeNewChamberEntrances = null)
    {
        ChamberExits lbExit = ChamberExits.eeNone;

        List<ChamberExits> lacExits = new List<ChamberExits>();

        //If entrance is none, pick random exit from all potential exits
        if(peChamberEntrance == ChamberExits.eeNone)
        {
            //Match new chamber entrances to available exits
            List<ChamberExits> laeExitOptions;

            //Create list of potential exits from next chamber entrances
            foreach(ChamberExits leChamberExit in paeNewChamberEntrances)
            {
                laeExitOptions = macChamberExitOptions[leChamberExit];

                foreach(ChamberExits leExitOption in laeExitOptions)
                {
                    if(maeAvailableExits.Contains(leExitOption))
                    {
                        lacExits.Add(leExitOption);
                    }
                }
            }
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

    /**
     * Gets a random valid exit to this chamber based on provided entrance
     */
    public List<ChamberExits> GetAllValidExits(ChamberExits peChamberEntrance, List<ChamberExits> paeNewChamberEntrances = null)
    {
        List<ChamberExits> lacExits = new List<ChamberExits>();

        //If entrance is none, pick random exit from all potential exits
        if (peChamberEntrance == ChamberExits.eeNone)
        {
            //Match new chamber entrances to available exits
            List<ChamberExits> laeExitOptions;

            //Create list of potential exits from next chamber entrances
            foreach (ChamberExits leChamberExit in paeNewChamberEntrances)
            {
                laeExitOptions = macChamberExitOptions[leChamberExit];

                foreach (ChamberExits leExitOption in laeExitOptions)
                {
                    if (maeAvailableExits.Contains(leExitOption))
                    {
                        lacExits.Add(leExitOption);
                    }
                }
            }
        }
        //Check Whether new entrance is Top left or right
        else if (peChamberEntrance == ChamberExits.eeLeft || peChamberEntrance == ChamberExits.eeMiddleLeft)
        {
            if (maeAvailableExits.Contains(ChamberExits.eeRight))
            {
                lacExits.Add(ChamberExits.eeRight);
            }

            if (maeAvailableExits.Contains(ChamberExits.eeMiddleRight))
            {
                lacExits.Add(ChamberExits.eeMiddleRight);
            }
        }
        else if (peChamberEntrance == ChamberExits.eeRight || peChamberEntrance == ChamberExits.eeMiddleRight)
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

        //Randomize Order of list 
        var lacExitsCopy = new List<ChamberExits>();

        //Simple randomized shuffle
        while(lacExits.Count != 0)
        {
            int lnShuffleIdx = UnityEngine.Random.Range(0, lacExits.Count);
            lacExitsCopy.Add(lacExits[lnShuffleIdx]);
            lacExits.RemoveAt(lnShuffleIdx);
        }

        //Return list of potenial exits in random order
        return lacExitsCopy;
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
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-3, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-3, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-3, -3));
                    }
                    break;
                case ChamberExits.eeLeft:
                    if (meChamberSize == ChamberSize.eeDefault || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, -4));
                    }
                    else
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, -4));
                    }
                    break;
                case ChamberExits.eeMiddleLeft:
                    if (meChamberSize == ChamberSize.eeLarge)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, 3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, 5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-10, 6));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, 3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, 5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-11, 6));
                    }
                    else if(meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, 3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, 5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-8, 6));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, 3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, 5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-9, 6));
                    }
                    break;
                case ChamberExits.eeTopLeft:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-7, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-6, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-7, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-6, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                    }
                    break;
                case ChamberExits.eeTop:
                    if (meChamberSize == ChamberSize.eeDefault || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, 2 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, 2 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-3, 2 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, 3 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, 3 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(-3, 3 + ((meChamberSize == ChamberSize.eeTall) ? 7 : 0)));
                    }
                    break;
                case ChamberExits.eeTopRight:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(3, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(4, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(5, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(6, 2 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(3, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(4, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(5, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                        macChamberExitDictionary[leExits].Add(new Vector2(6, 3 + ((meChamberSize == ChamberSize.eeLarge) ? 7 : 0)));
                    }
                    break;
                case ChamberExits.eeRight:
                    if (meChamberSize == ChamberSize.eeDefault || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(1, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(1, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(1, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(1, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(2, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(2, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(2, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(2, -4));
                    }
                    else
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(9, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(9, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(9, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(9, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(10, -1));
                        macChamberExitDictionary[leExits].Add(new Vector2(10, -2));
                        macChamberExitDictionary[leExits].Add(new Vector2(10, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(10, -4));
                    }
                    break;
                case ChamberExits.eeMiddleRight:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeTall)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(1 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 3));
                        macChamberExitDictionary[leExits].Add(new Vector2(1 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(1 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 5));
                        macChamberExitDictionary[leExits].Add(new Vector2(1 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 6));
                        macChamberExitDictionary[leExits].Add(new Vector2(2 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 3));
                        macChamberExitDictionary[leExits].Add(new Vector2(2 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 4));
                        macChamberExitDictionary[leExits].Add(new Vector2(2 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 5));
                        macChamberExitDictionary[leExits].Add(new Vector2(2 + ((meChamberSize == ChamberSize.eeLarge) ? 8 : 0), 6));
                    }
                    break;
                case ChamberExits.eeBottomLeft:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(-7, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-6, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(-7, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-6, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(-7, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-6, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-5, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(-4, -5));
                    }
                    break;
                case ChamberExits.eeBottomRight:
                    if (meChamberSize == ChamberSize.eeLarge || meChamberSize == ChamberSize.eeLong)
                    {
                        macChamberExitDictionary[leExits].Add(new Vector2(3, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(4, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(5, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(6, -3));
                        macChamberExitDictionary[leExits].Add(new Vector2(3, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(4, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(5, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(6, -4));
                        macChamberExitDictionary[leExits].Add(new Vector2(3, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(4, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(5, -5));
                        macChamberExitDictionary[leExits].Add(new Vector2(6, -5));
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

    //Returns the dimensions of the chamber 
    public Vector2 GetChamberDimensions()
    {
        return mcChamberDimensions;
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
