using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Deck;
using System;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Linq;
using static Unity.VisualScripting.Member;
using System.Drawing;
using static UnityEditor.PlayerSettings;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEditor.PackageManager;

public class LevelManager : MonoBehaviour
{
    //Number of levels in the game
    private int mnNumLevels = 3;

    //Rate in degrees at which the level rotates 
    public float mfLevelRotationDegrees = 10f;

    //Lists of boss chambers organized by level
    private List<GameObject>[] macLevelChambers;

    //Lists of boss chambers organized by level
    private List<GameObject>[] macLevelBossChamber;

    //Lists of Special chambers organized by level
    private List<GameObject>[] macLevelSpecialChamber;

    //Level Grid used to check overlap and reference map
    private LevelGrid[] mcLevelGrid;

    //List of Levels created in this game
    private GameObject[] macLevels;

    //Tilesets used to show map
    private Tilemap[] macLevelMapTileSets;

    //Chambers for Current level
    private List<GameObject>[] macCurrentLevelChambers;

    GameObject mcCurrentChamber = null;

    public Animator mcTransition;

    //Number of chambers to be added to level
    private int mnNumChambersInLevel = 5;

    //Number of special chambers to be added to level
    private int mnNumSpecialChambersInLevel = 1;

    //reference to camera used to change camera bounds
    public ConfinerUpdater mcConfinerUpdater;

    //2D translation from base point based on Chamber exit
    private Dictionary<int, Vector2> macChamberExitTranslations
        = new Dictionary<int, Vector2>();

    //MapController access
    public MapController MapControllerAccess;

    //Enemy manager used to create enemys in scene
    [SerializeField] private EnemyManager mcEnemyManager;

    //Set of Chamber offsets
    Vector2[,,,] macChamberPositionOffsets = new Vector2[(int)ChamberSize.eeLarge + 1, (int)ChamberSize.eeLarge + 1, (int)ChamberExits.eeNone, (int)ChamberExits.eeNone];

    //TileSet of the World map
    [SerializeField] private GameObject mcWorldMapTilesetPrefab;

    //Virtual Camera object used to track player
    [SerializeField] private GameObject mcPlayerCamera;

    // Start is called before the first frame update
    void Start()
    {
        macLevelChambers = new List<GameObject>[mnNumLevels];
        macLevelBossChamber = new List<GameObject>[mnNumLevels];
        macLevelSpecialChamber = new List<GameObject>[mnNumLevels];
        mcLevelGrid = new LevelGrid[mnNumLevels];
        macCurrentLevelChambers = new List<GameObject>[mnNumLevels];
        macLevels = new GameObject[mnNumLevels];

        for (int lnNumLevels = 0; lnNumLevels < mnNumLevels; lnNumLevels++)
        {
            GameObject lcLevel = transform.Find("Level_" + (lnNumLevels + 1)).GameObject();

            //Load chambers for each level
            LoadChambers(lnNumLevels, lcLevel);
            LoadBossChambers(lnNumLevels, lcLevel);
            LoadSpecialChambers(lnNumLevels, lcLevel);
        }

        //Creates translations from base exits
        InitializeChamberExitTranslations();

        mcConfinerUpdater = GameObject.Find("Confiners").GetComponent<ConfinerUpdater>();

        MapControllerAccess = GameObject.Find("MapController").GetComponent<MapController>();

        if (MapControllerAccess == null )
        {
            Debug.Log("MapController not Found");
        }

        //Load all enemy and enemy attacks before building level
        mcEnemyManager.Initialize();

        Vector3 mcLevelExit = GenerateLevel(0, Vector3.zero);
        mcLevelExit = GenerateLevel(1, mcLevelExit, -90);
        GenerateLevel(2, mcLevelExit, -180);
    }

    /**
     * METHOD: Creates a Level Layout made of Level Chambers
     */
    private Vector3 GenerateLevel(int pnLevel, Vector3 pcLevelShift, float pfRotation = 0)
    {
        mcLevelGrid[pnLevel] = new LevelGrid();

        if(pnLevel == 1)
        {
            mcLevelGrid[pnLevel].BlockGridAxis(Vector2.left);
        }

        if (pnLevel == 2)
        {
            mcLevelGrid[pnLevel].BlockGridAxis(Vector2.right);
        }

        Transform lcLevel = transform.Find("Level_" + (pnLevel + 1));

        //Add level to list of levels
        macLevels[pnLevel] = lcLevel.gameObject;

        //Set level pool based on provided level index
        List<GameObject> lacLevelPool = new List<GameObject>(macLevelChambers[pnLevel]);

        macCurrentLevelChambers[pnLevel] = new List<GameObject>();

        if (lacLevelPool.Count != 0)
        {
            //Add Start Chamber, Should Always be first in heirarchy 
            macCurrentLevelChambers[pnLevel].Add(lacLevelPool[0]);

            //Set start chamber to current chamber
            mcCurrentChamber = macCurrentLevelChambers[pnLevel][0];

            //Get Collection of random chambers to add to level
            for (int lnNumChambers = 1; lnNumChambers < mnNumChambersInLevel; lnNumChambers++)
            {
                int lbRandomRoom = UnityEngine.Random.Range(1, (lacLevelPool.Count));

                //Add the random room and remove from the pool
                macCurrentLevelChambers[pnLevel].Add(lacLevelPool[lbRandomRoom]);
                lacLevelPool.RemoveAt(lbRandomRoom);
            }

            //Add Boss Room to end of Chamber list 
            macCurrentLevelChambers[pnLevel].Add(macLevelBossChamber[pnLevel][UnityEngine.Random.Range(0, macLevelBossChamber[pnLevel].Count)]);

            //Initialize each chamber before it is placed in level
            foreach (GameObject lcLevelChamber in macCurrentLevelChambers[pnLevel])
            {
                lcLevelChamber.GetComponent<Chamber>().InitializeChamber();
                //Debug.Log("Initialize Chamber");
            }

            //Begin with Start Chamber
            Chamber lcChamber = macCurrentLevelChambers[pnLevel][0].GetComponent<Chamber>();

            //Add Start chamber to grid 
            mcLevelGrid[pnLevel].AddChamberToGrid(0, 0, macCurrentLevelChambers[pnLevel][0]);

            MapControllerAccess.AddChamberToMap(new List<Vector2>() { new Vector2(0,0) }, macCurrentLevelChambers[pnLevel][0]);

            int lnNumFailedExits = 0;
            int lnNextChamberToAdd = 1;

            //for each chamber in level that needs to be added
            for (int lnNumChambers = 0; lnNumChambers < mnNumChambersInLevel + 1; lnNumChambers++)
            {
                lcChamber = macCurrentLevelChambers[pnLevel][lnNumChambers].GetComponent<Chamber>();

                //TODO: refactor treasure populate

                //mcEnemyManager.PopulateWithEnemies(lcChamber);

                //lcChamber.PopulateWithDestructibles();

                int lnNumPotentialRooms = lcChamber.GetNumPotentialRooms();

                //for each potential exit
                for (int lnNumExits = 0; lnNumExits < lnNumPotentialRooms; lnNumExits++)
                {
                    //Room will be generated based on chance
                    if (AllocateChamber(lnNumFailedExits, lnNumPotentialRooms) && lnNextChamberToAdd < mnNumChambersInLevel + 1)
                    {
                        //Get next chamber to add
                        Chamber lcAttachedChamber = macCurrentLevelChambers[pnLevel][lnNextChamberToAdd].GetComponent<Chamber>();

                        //Get Next available exit from this chamber
                        //ChamberExits leChamberExit = lcChamber.GetValidExit(ChamberExits.eeNone, lcAttachedChamber.GetAvailableExits());

                        ChamberExits leChamberExit = ChamberExits.eeNone;
                        ChamberExits leNewChamberEntrance = ChamberExits.eeNone;

                        List <ChamberExits> lacAvailableExitsBetweenChambers = lcChamber.GetAllValidExits(ChamberExits.eeNone, lcAttachedChamber.GetAvailableExits());

                        foreach(ChamberExits lePotentialExit in lacAvailableExitsBetweenChambers)
                        {
                            //Determine Grid point from chamber exit chosen
                            Vector2 lcExitPoint = lcChamber.GetExitPoint(lePotentialExit);

                            //Get random entrance to new chamber that coincides with random exit of current chamber
                            List <ChamberExits> leNewChamberEntrances = lcAttachedChamber.GetAllValidExits(lePotentialExit);

                            foreach(ChamberExits lePotentialEntrance in leNewChamberEntrances)
                            {
                                //Debug.Log("Check Coordinates   Exit: " + lePotentialExit + " Entrance: " + lePotentialEntrance);
                                //Check if Level Grid allows for this new chamber at this point
                                if (AssignNextChamberCoordinates(pnLevel, (int)lcExitPoint.x, (int)lcExitPoint.y, lePotentialEntrance, macCurrentLevelChambers[pnLevel][lnNextChamberToAdd]))
                                {
                                    leChamberExit = lePotentialExit;
                                    leNewChamberEntrance = lePotentialEntrance;
                                    break;
                                }
                            }

                            if(leChamberExit != ChamberExits.eeNone)
                            {
                                break;
                            }
                        }

                        if (leChamberExit == ChamberExits.eeNone)
                        {
                            Debug.Log("Failed to find valid exit for Chamber: " + lcChamber.name + "To Chamber " + lcAttachedChamber.gameObject + " Count " + lcAttachedChamber.GetAvailableExits().Count);
                        }

                        //Determine Grid point from chamber exit chosen
                        //Vector2 lcExitPoint = lcChamber.GetExitPoint(leChamberExit);

                        //Check if randomized chamber can be added to grid
                        if (leNewChamberEntrance != ChamberExits.eeNone)
                        {
                            //Create Exit to the Next chamber in the set
                            lcChamber.CreateExit(leChamberExit, macCurrentLevelChambers[pnLevel][lnNextChamberToAdd]);

                            //Create exit on added chamber
                            lcAttachedChamber.CreateExit(leNewChamberEntrance, macCurrentLevelChambers[pnLevel][lnNumChambers]);

                            //Set chamber Entrance/Exit connections
                            lcChamber.ConnectExits(leChamberExit, leNewChamberEntrance);
                            lcAttachedChamber.ConnectExits(leNewChamberEntrance, leChamberExit);

                            //Shift new chamber 2D position to connect it to base chamber
                            PositionNewChamber(lcChamber.transform.position, lcChamber.GetChamberSize(), leChamberExit, lcAttachedChamber.GetChamberSize(), 
                                leNewChamberEntrance, macCurrentLevelChambers[pnLevel][lnNextChamberToAdd]);

                            Debug.Log("New Chamber: " + macCurrentLevelChambers[pnLevel][lnNextChamberToAdd].name + " Entrance: " + leNewChamberEntrance + 
                                "  Base Chamber: " + lcChamber.gameObject.name  + " Exit: " + leChamberExit);

                            //Update Camera Confiner to add new chamber
                            mcConfinerUpdater.AddPolygonCameraCollider(macCurrentLevelChambers[pnLevel][lnNextChamberToAdd].GetComponent<Chamber>().GetCameraCollider(),
                                macCurrentLevelChambers[pnLevel][lnNextChamberToAdd].transform.position);

                            lnNextChamberToAdd++;
                        }
                        else
                        {
                            Debug.Log("Failed to create connections: " + macCurrentLevelChambers[pnLevel][lnNumChambers].name + " to Chamber " + macCurrentLevelChambers[pnLevel][lnNextChamberToAdd].name + " Exit: " + leChamberExit);
                            lnNumFailedExits++;
                        }
                    }
                    else
                    {
                        lnNumFailedExits++;
                    }

                    //If added chambers goes over max chambers stop generation
                    if(lnNextChamberToAdd >= mnNumChambersInLevel + 1)
                    {
                        break;
                    }

                }

                lnNumFailedExits = 0;
            }
        }

        //Add special chambers to level
        for (int lnNumSpecialChambers = 0; lnNumSpecialChambers < mnNumSpecialChambersInLevel; lnNumSpecialChambers++)
        {
            //Get Random special chamber associated with this level
            GameObject lcSpecialChamber = macLevelSpecialChamber[pnLevel][UnityEngine.Random.Range(0, macLevelSpecialChamber[pnLevel].Count)];

            //Create map tileset for main chambers of the level
            GameObject lcSpecialChambersTileSet = Instantiate(mcWorldMapTilesetPrefab, Vector3.zero, lcSpecialChamber.transform.rotation, lcSpecialChamber.transform);
            //lcSpecialChamber.GetComponent<Chamber>().InitializeChamber();

            //Pick random chamber in level and add special chamber to it
            int lnChamberIndex = UnityEngine.Random.Range(1, (mnNumChambersInLevel));

            //Link level chamber and side chamber together
            macCurrentLevelChambers[pnLevel][lnChamberIndex].GetComponent<Chamber>().SetSideChamber(lcSpecialChamber.GetComponent<Chamber>());
            lcSpecialChamber.GetComponent<Chamber>().SetSideChamber(macCurrentLevelChambers[pnLevel][lnChamberIndex].GetComponent<Chamber>());

            //Add Door to selected level chamber
            macCurrentLevelChambers[pnLevel][lnChamberIndex].GetComponent<Chamber>().CreateDoorway();

            Vector3 lcDoorwayPos = macCurrentLevelChambers[pnLevel][lnChamberIndex].GetComponent<Chamber>().GetDoorPosition();

            Debug.Log("DoorWay Position: " + lcDoorwayPos);

            //Add Ground/Wall tiles to map tileset
            UpdateMapTileGrid(new List<GameObject>() { lcSpecialChamber }, lcSpecialChambersTileSet.GetComponent<Tilemap>());

            //Reposition special chamber to position of Door in level chamber
            lcSpecialChamber.transform.localPosition = new Vector3(-4, (lcDoorwayPos.y + 3), (lcDoorwayPos.x));

            Debug.Log("Created Doorway on chamber: " + macCurrentLevelChambers[pnLevel][lnChamberIndex].name);
        }

        //Get Boss chamber Doorway to Next level
        macCurrentLevelChambers[pnLevel][mnNumChambersInLevel].GetComponent<Chamber>().CreateDoorway();

        Debug.Log("LEVEL " + (pnLevel + 1) + " SHIFT: " + pcLevelShift);

        if(pnLevel != 0)
        {
            //Open doors on start room
            macCurrentLevelChambers[pnLevel][0].GetComponent<Chamber>().CreateExit(ChamberExits.eeLeft);
            macCurrentLevelChambers[pnLevel][0].GetComponent<Chamber>().CreateExit(ChamberExits.eeRight);
        }

        //Create map tileset for main chambers of the level
        GameObject lcMainChambersTileSet = Instantiate(mcWorldMapTilesetPrefab, Vector3.zero, Quaternion.identity);

        //Add Ground/Wall tiles to map tileset
        UpdateMapTileGrid(macCurrentLevelChambers[pnLevel], lcMainChambersTileSet.GetComponent<Tilemap>());

        lcMainChambersTileSet.transform.SetParent(lcLevel);

        //Reposition Level based off of previous levels exit
        if (pcLevelShift != Vector3.zero)
        {
            //Set level to inactive 
            lcLevel.gameObject.SetActive(false);

            //Rotate level about Y-axis
            lcLevel.Rotate(0, pfRotation, 0);
            lcLevel.localPosition = AdjustedLevelPlacement(pcLevelShift, pfRotation);

            //Connect level exits to level entrances
            //Connect Last Chamber (Boss Room) of previous level to start room (0) of current level
            macCurrentLevelChambers[pnLevel - 1][(macCurrentLevelChambers[pnLevel - 1].Count - 1)].GetComponent<Chamber>().SetSideChamber
                (macCurrentLevelChambers[pnLevel][0].GetComponent<Chamber>());

            macCurrentLevelChambers[pnLevel][0].GetComponent<Chamber>().SetSideChamber
                (macCurrentLevelChambers[pnLevel - 1][(macCurrentLevelChambers[pnLevel - 1].Count - 1)].GetComponent<Chamber>());
        }

        Vector3 lcLevelExitPos = macCurrentLevelChambers[pnLevel][mnNumChambersInLevel].GetComponent<Chamber>().GetDoorPosition();

        Debug.Log("LEVEL " + (pnLevel + 1) + " Exit position: " + lcLevelExitPos);

        //Pathfinding rescan on new level
        AstarPath.active.Scan();

        return lcLevelExitPos;
    }

    private Vector3 AdjustedLevelPlacement(Vector3 pcPreviousLevelDoorPos, float pfLevelRotation)
    {
        Vector3 lcReturnPosition = new Vector3(pcPreviousLevelDoorPos.x, (pcPreviousLevelDoorPos.y + 3), pcPreviousLevelDoorPos.z);

        if (pfLevelRotation == 90)
        {
            lcReturnPosition = new Vector3(lcReturnPosition.x, lcReturnPosition.y, (lcReturnPosition.z - 10));
        }
        if (pfLevelRotation == -90)
        {
            lcReturnPosition = new Vector3(lcReturnPosition.x, lcReturnPosition.y, (lcReturnPosition.z + 10));
        }
        else if(pfLevelRotation == -180)
        {
            lcReturnPosition = new Vector3((lcReturnPosition.x + 4), lcReturnPosition.y, lcReturnPosition.z);
        }
        else if (pfLevelRotation == 180)
        {
            lcReturnPosition = new Vector3((lcReturnPosition.x - 4), lcReturnPosition.y, lcReturnPosition.z);
        }

        return lcReturnPosition;
    }

    private void UpdateMapTileGrid(List<GameObject> pacLevelChambers, Tilemap pcLevelTilemap)
    {
        pcLevelTilemap.size = new Vector3Int(40, 40, 40);
        pcLevelTilemap.ResizeBounds();

        //List of tilemaps 
        List<Tilemap> lacTilemaps = new List<Tilemap>();

        foreach (GameObject lcChamber in pacLevelChambers)
        {
            //Get tilemap for ground and wall children
            lacTilemaps.Add(lcChamber.transform.Find("Ground").GetComponent<Tilemap>());
            lacTilemaps.Add(lcChamber.transform.Find("Walls").GetComponent<Tilemap>());

            foreach (Transform lcExitChildren in lcChamber.transform.Find("Exits"))
            {
                if(lcExitChildren.gameObject.activeSelf)
                {
                    lacTilemaps.Add(lcExitChildren.Find("ExitGround").GetComponent<Tilemap>());
                    lacTilemaps.Add(lcExitChildren.Find("ExitWalls").GetComponent<Tilemap>());
                }
            }

            foreach (Tilemap lcTilemap in lacTilemaps)
            {
                for (int x = lcTilemap.cellBounds.xMin; x < lcTilemap.cellBounds.xMax; x++)
                {
                    for (int y = lcTilemap.cellBounds.yMin; y < lcTilemap.cellBounds.yMax; y++)
                    {
                        Vector3Int localLocation = new Vector3Int(
                            x: x,
                            y: y,
                            z: 0);

                        Vector3Int offsetPos = new Vector3Int(localLocation.x + (int)lcChamber.transform.position.x,
                            localLocation.y + (int)lcChamber.transform.position.y, 0);

                        if (lcTilemap.GetTile(localLocation) != null)
                        {
                            //Debug.Log("Chamber " + lcTilemap.transform.parent.name + " " + lcTilemap.transform.name + " PlaceTile: X: " + offsetPos.x + " Y: " + offsetPos.y + " Tile: " + lcTilemap.GetTile(localLocation).name);
                            pcLevelTilemap.SetTile(offsetPos, lcTilemap.GetTile(localLocation));
                        }
                    }
                }
            }

            lacTilemaps.Clear();
        }

    }

    private bool AssignNextChamberCoordinates(int pnLevel, int pnBaseChamberX, int pnBaseChamberY, 
         ChamberExits peNewChamberEntrance, GameObject pcNewChamber)
    {
        bool lbValidPlacement = true;

        ChamberSize leNewChamberSize = pcNewChamber.GetComponent<Chamber>().GetChamberSize();

        List<Vector2> lacGridSectors = new List<Vector2>();

        //Initial point using base chamber
        lacGridSectors.Add(new Vector2(pnBaseChamberX, pnBaseChamberY) + new Vector2(0, 0)
            + macChamberExitTranslations[(int)leNewChamberSize * 10 + (int)peNewChamberEntrance]);

        if (leNewChamberSize == ChamberSize.eeLarge)
        {
            lacGridSectors.Add(new Vector2(pnBaseChamberX, pnBaseChamberY) + new Vector2(1, 0)
                + macChamberExitTranslations[(int)leNewChamberSize * 10 + (int)peNewChamberEntrance]);
            lacGridSectors.Add(new Vector2(pnBaseChamberX, pnBaseChamberY) + new Vector2(0, 1)
                + macChamberExitTranslations[(int)leNewChamberSize * 10 + (int)peNewChamberEntrance]);
            lacGridSectors.Add(new Vector2(pnBaseChamberX, pnBaseChamberY) + new Vector2(1, 1)
                + macChamberExitTranslations[(int)leNewChamberSize * 10 + (int)peNewChamberEntrance]);
        }
        else if(leNewChamberSize == ChamberSize.eeLong)
        {
            lacGridSectors.Add(new Vector2(pnBaseChamberX, pnBaseChamberY) + new Vector2(1, 0)
                + macChamberExitTranslations[(int)leNewChamberSize * 10 + (int)peNewChamberEntrance]);
        }
        else if (leNewChamberSize == ChamberSize.eeTall)
        {
            lacGridSectors.Add(new Vector2(pnBaseChamberX, pnBaseChamberY) + new Vector2(0, 1)
                + macChamberExitTranslations[(int)leNewChamberSize * 10 + (int)peNewChamberEntrance]);
        }

        //Check whether new Chamber overlaps with existing chamber
        foreach (Vector2 lcNewChamberPoint in lacGridSectors)
        {
            if (mcLevelGrid[pnLevel].IsGridSectionFilled((int)lcNewChamberPoint.x, (int)lcNewChamberPoint.y))
            {
                lbValidPlacement = false;
                break;
            }
        }

        //if placement valid, then cover grid locations
        if(lbValidPlacement)
        {
            //Add grid points for chamber to maintain
            pcNewChamber.GetComponent<Chamber>().AddGridPoints(lacGridSectors);

            MapControllerAccess.AddChamberToMap(lacGridSectors, pcNewChamber);

            foreach (Vector2 lcNewChamberPoint in lacGridSectors)
            {
                mcLevelGrid[pnLevel].AddChamberToGrid((int)lcNewChamberPoint.x, (int)lcNewChamberPoint.y, pcNewChamber);

                //Debug.Log("Add Grid Point    x: " + lcNewChamberPoint.x + "   y: " + lcNewChamberPoint.y);
            }
        }

        return lbValidPlacement;
    }

    /**
     * Moves the new chamber position relative to the base chamber 
     */
    private void PositionNewChamber(Vector2 pcBaseChamberPosition, ChamberSize peBaseChamberSize, ChamberExits peBaseChamberExit, ChamberSize peNewChamberSize,
         ChamberExits peNewChamberEntrance, GameObject pcNewChamber)
    {
       
        Vector2 lcNewChamberPositon = pcBaseChamberPosition;

        int lnBaseOffsetX = 0;
        int lnBaseOffsetY = 0;

        //Adjust 2D position from the size of the base chamber and the entrance of the new chamber.

        if(peNewChamberEntrance == ChamberExits.eeMiddleLeft || peNewChamberEntrance == ChamberExits.eeLeft)
        {
            lnBaseOffsetX = 14;
        }

        if (peNewChamberEntrance == ChamberExits.eeMiddleRight || peNewChamberEntrance == ChamberExits.eeRight)
        {
            lnBaseOffsetX = -14;
        }

        if (peNewChamberEntrance == ChamberExits.eeBottomLeft || peNewChamberEntrance == ChamberExits.eeBottom || peNewChamberEntrance == ChamberExits.eeBottomRight)
        {
            lnBaseOffsetY = 8;
        }

        if (peNewChamberEntrance == ChamberExits.eeTopLeft || peNewChamberEntrance == ChamberExits.eeTop || peNewChamberEntrance == ChamberExits.eeTopRight)
        {
            lnBaseOffsetY = -8;
        }

        if (((peBaseChamberSize == ChamberSize.eeLarge || peBaseChamberSize == ChamberSize.eeLong) && (peNewChamberEntrance == ChamberExits.eeMiddleLeft || peNewChamberEntrance == ChamberExits.eeLeft))
            || ((peNewChamberSize == ChamberSize.eeLarge || peNewChamberSize == ChamberSize.eeLong) && (peNewChamberEntrance == ChamberExits.eeMiddleRight || peNewChamberEntrance == ChamberExits.eeRight)))
        {
            if (lnBaseOffsetX < 0)
            {
                lnBaseOffsetX -= 8;
            }
            else
            {
                lnBaseOffsetX += 8;
            }
        }

        if (((peBaseChamberSize == ChamberSize.eeLarge || peBaseChamberSize == ChamberSize.eeTall) && (peNewChamberEntrance == ChamberExits.eeBottom || peNewChamberEntrance == ChamberExits.eeBottomRight || peNewChamberEntrance == ChamberExits.eeBottomLeft))
            || ((peNewChamberSize == ChamberSize.eeLarge || peNewChamberSize == ChamberSize.eeTall) && (peNewChamberEntrance == ChamberExits.eeTop || peNewChamberEntrance == ChamberExits.eeTopLeft || peNewChamberEntrance == ChamberExits.eeTopRight)))
        {
            if(lnBaseOffsetY < 0)
            {
                lnBaseOffsetY -= 7;
            }
            else
            {
                lnBaseOffsetY += 7;
            }
        }

        if((peNewChamberEntrance == ChamberExits.eeLeft && peBaseChamberExit == ChamberExits.eeMiddleRight) ||
            (peNewChamberEntrance == ChamberExits.eeRight && peBaseChamberExit == ChamberExits.eeMiddleLeft))
        {
            lnBaseOffsetY += 7;
        }

        if ((peNewChamberEntrance == ChamberExits.eeMiddleRight && peBaseChamberExit == ChamberExits.eeLeft) ||
            (peNewChamberEntrance == ChamberExits.eeMiddleLeft && peBaseChamberExit == ChamberExits.eeRight))
        {
            lnBaseOffsetY -= 7;
        }


        //Top bottom entrance offsets
        if ((peNewChamberEntrance == ChamberExits.eeBottom && peBaseChamberExit == ChamberExits.eeTopLeft) ||
            (peNewChamberEntrance == ChamberExits.eeTop && peBaseChamberExit == ChamberExits.eeBottomLeft))
        {
            lnBaseOffsetX -= 2;
        }

        if ((peNewChamberEntrance == ChamberExits.eeTopLeft && peBaseChamberExit == ChamberExits.eeBottom) ||
            (peNewChamberEntrance == ChamberExits.eeBottomLeft && peBaseChamberExit == ChamberExits.eeTop))
        {
            lnBaseOffsetX += 2;
        }

        if ((peNewChamberEntrance == ChamberExits.eeBottom && peBaseChamberExit == ChamberExits.eeTopRight) ||
            (peNewChamberEntrance == ChamberExits.eeTop && peBaseChamberExit == ChamberExits.eeBottomRight))
        {
            lnBaseOffsetX += 8;
        }

        if ((peNewChamberEntrance == ChamberExits.eeTopRight && peBaseChamberExit == ChamberExits.eeBottom) ||
            (peNewChamberEntrance == ChamberExits.eeBottomRight && peBaseChamberExit == ChamberExits.eeTop))
        {
            lnBaseOffsetX -= 8;
        }

        if ((peNewChamberEntrance == ChamberExits.eeTopRight && peBaseChamberExit == ChamberExits.eeBottomLeft) ||
            (peNewChamberEntrance == ChamberExits.eeBottomRight && peBaseChamberExit == ChamberExits.eeTopLeft))
        {
            lnBaseOffsetX -= 10;
        }

        if ((peNewChamberEntrance == ChamberExits.eeBottomLeft && peBaseChamberExit == ChamberExits.eeTopRight) ||
            (peNewChamberEntrance == ChamberExits.eeTopLeft && peBaseChamberExit == ChamberExits.eeBottomRight))
        {
            lnBaseOffsetX += 10;
        }


        lcNewChamberPositon.x += lnBaseOffsetX;
        lcNewChamberPositon.y += lnBaseOffsetY;

        pcNewChamber.GetComponent<Chamber>().RepositionChamber(lcNewChamberPositon);

        //pcNewChamber.transform.gameObject.SetActive(false);
    }

    /*
     * METHOD: Returns whether or not to create chamber based on probability
     */
    private bool AllocateChamber(int pnNumFailures, int pnPotentialRooms)
    {
        bool lbCreateChamber = false;

        //Probability of chamber creation increases with failures
        float lfRoomProbability = (1 / (float)pnPotentialRooms) + ((1 / (float)pnPotentialRooms) * (float)pnNumFailures);

        //Room will be generated based on chance
        if (UnityEngine.Random.Range(0f, 1f) < lfRoomProbability)
        {
            lbCreateChamber = true;
        }
        else
        {
            //Debug.Log("AllocateChamber: Failed " + pnNumFailures + " " + pnPotentialRooms);
        }

            return lbCreateChamber;
    }

    /**
     * METHOD: Loads all chambers for a specified level
     */
    private void LoadChambers (int pnLevel, GameObject pcLevel)
    {
        //Get the desired level game object 
        GameObject lcLevelGameObject = pcLevel.transform.Find("Level_Chambers").GameObject();

        macLevelChambers[pnLevel] = new List<GameObject>();

        //Add all Chambers to LevelChambers
        foreach (Transform lcChamber in lcLevelGameObject.transform)
        {
            lcChamber.position = Vector2.zero;
            macLevelChambers[pnLevel].Add(lcChamber.gameObject);
        }
    }

    private void LoadBossChambers(int pnLevel, GameObject pcLevel)
    {
        //Get Child List of boss rooms for this level
        GameObject lcLevelBossRoomList = pcLevel.transform.Find("Level_BossRooms").GameObject();

        macLevelBossChamber[pnLevel] = new List<GameObject>();

        //Add all Chambers to LevelChambers
        foreach (Transform lcChamber in lcLevelBossRoomList.transform)
        {
            lcChamber.position = Vector2.zero;
            macLevelBossChamber[pnLevel].Add(lcChamber.gameObject);
        }
    }

    private void LoadSpecialChambers(int pnLevel, GameObject pcLevel)
    {
        //Get Child List of boss rooms for this level
        GameObject lcLevelSpecialRoomList = pcLevel.transform.Find("Level_SpecialRooms").GameObject();

        macLevelSpecialChamber[pnLevel] = new List<GameObject>();

        //Add all Chambers to LevelChambers
        foreach (Transform lcChamber in lcLevelSpecialRoomList.transform)
        {
            macLevelSpecialChamber[pnLevel].Add(lcChamber.gameObject);
        }
    }

    //Changes the orientation of the world map 
    public void EnterNewArea(Transform pcPlayer, Chamber pcChamber, Transform pcDoor)
    {
        StartCoroutine(EnterNewAreaCoroutine(pcPlayer, pcChamber, pcDoor));
    }

    private IEnumerator EnterNewAreaCoroutine(Transform pcPlayer, Chamber pcChamber, Transform pcDoor)
    {
        //Freeze Time/ physics calculations
        Time.timeScale = 0f;

        //Local copy of levels list
        GameObject[] lacLevels = macLevels;

        Chamber lcSideChamber = pcChamber.GetSideChamber();

        PlayerMovement lcPlayerControl = pcPlayer.GetComponent<PlayerMovement>();   

        pcChamber.DisableAdjacentChambers(pcChamber.transform.gameObject);

        bool lbMainDoorway = pcChamber.HasDoorway();

        Vector2 lcInitialPosition = pcPlayer.position;
        Vector2 lcAdjustedPosition = new Vector2(0.1f, lcInitialPosition.y);

        Vector3 lcCameraInitialPosition = mcPlayerCamera.transform.position;
        Vector3 lcCameraAdjustedPosition = new Vector3(0.1f, lcCameraInitialPosition.y, lcCameraInitialPosition.z);

        float elapsedTime = 0;
        float waitTime = 1f;
        float DoorOpenTime = 0.35f;

        //Rotation may only be 90 or -90 degrees
        float mfRotation = Mathf.Round((lcSideChamber.transform.parent.transform.localEulerAngles.y != 0) ? 
            lcSideChamber.transform.parent.transform.localEulerAngles.y : lcSideChamber.transform.parent.transform.parent.localEulerAngles.y);
        mfRotation = (mfRotation - ((mfRotation > 180) ? 360 : 0)) * -1;

        //Play player animation to enter door
        if (lbMainDoorway) { lcPlayerControl.PlayEnterDoor(); }

        Vector3 lcPivotPosition = pcDoor.position;

        //Open the Door
        while (elapsedTime < DoorOpenTime)
        {
            pcDoor.localRotation = Quaternion.Slerp(pcDoor.transform.localRotation, Quaternion.Euler(0f, 90, 0f), (elapsedTime / DoorOpenTime));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        pcDoor.GetComponent<SpriteRenderer>().sortingLayerName = "Player";

        float lfPlayPositionXOffset = ((lbMainDoorway && mfRotation == 90) || (!lbMainDoorway && mfRotation == 90)) ? 0.5f : -0.5f;

        Debug.Log("mfRotation: " + mfRotation + " PlayPosXOffset: " + lfPlayPositionXOffset);

        pcChamber.SetChamberTransparency(0);
        lcSideChamber.SetChamberTransparency(1);

        elapsedTime = 0;

        Transform lcPlayerFollowPoint = mcPlayerCamera.GetComponent<CinemachineVirtualCamera>().Follow;

        //Play player animation to rotate
        if (lbMainDoorway) { lcPlayerControl.PlayTurn((mfRotation < 0)); }

        float lfWorldRotationSum = 0;

        while (elapsedTime < waitTime)
        {
            //Change opacity of chambers as shift occurs
            //pcChamber.SetChamberTransparency(Mathf.Lerp(0.9f, -0.2f, (elapsedTime / waitTime)));
            //lcSideChamber.SetChamberTransparency(Mathf.Lerp(0f, 1f, (elapsedTime / waitTime)));

            //Lerp player towards entrance of rotated space
            pcPlayer.position = Vector2.Lerp(lcInitialPosition, new Vector2(pcDoor.position.x + lfPlayPositionXOffset, lcInitialPosition.y), (elapsedTime / waitTime));
            //mcPlayerCamera.transform.position = Vector3.Lerp(lcCameraInitialPosition, new Vector2(pcDoor.position.x - 0.5f, lcCameraInitialPosition.y), (elapsedTime / waitTime));

            float lfWorldRotation = (Time.unscaledDeltaTime / waitTime) * 90;

            if(lfWorldRotationSum + lfWorldRotation > 90)
            {
                lfWorldRotation = 90 - lfWorldRotationSum;
            }

            foreach (GameObject lcLevel in lacLevels)
            {
                lcLevel.transform.RotateAround(lcPivotPosition, Vector3.up * ((mfRotation == -90) ? -1 : 1), lfWorldRotation);
            }

            lfWorldRotationSum += lfWorldRotation;

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        pcPlayer.position = new Vector2(pcDoor.position.x + lfPlayPositionXOffset, lcInitialPosition.y);

        pcDoor.GetComponent<SpriteRenderer>().sortingLayerName = "BackGround3";
        elapsedTime = 0;

        //Close the door
        while (elapsedTime < DoorOpenTime)
        {
            pcDoor.localRotation = Quaternion.Slerp(pcDoor.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), (elapsedTime / DoorOpenTime));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        //unFreeze Time/ physics calculations
        Time.timeScale = 1;

        //Update player to set to side chamber
        pcPlayer.GetComponent<PlayerMovement>().ChangeChamber(lcSideChamber);
        if (lbMainDoorway) { lcPlayerControl.SetPlayerAnimatorScaledTime(); }
    }

    private void InitializeChamberExitTranslations()
    {
        foreach (ChamberSize leSizes in Enum.GetValues(typeof(ChamberSize)))
        {
            foreach (ChamberExits leExits in Enum.GetValues(typeof(ChamberExits)))
            {
                switch (leExits)
                {
                    case ChamberExits.eeBottom:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(0, 1);
                        break;
                    case ChamberExits.eeLeft:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(1, 0);
                        break;
                    case ChamberExits.eeMiddleLeft:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(1, -1);
                        break;
                    case ChamberExits.eeTopLeft:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(0, -2 + 
                            ((leSizes == ChamberSize.eeLong) ? 1 : 0));
                        break;
                    case ChamberExits.eeTop:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(0, -1);
                        break;
                    case ChamberExits.eeTopRight:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(-1, -2 + 
                            ((leSizes == ChamberSize.eeLong) ? 1 : 0));
                        break;
                    case ChamberExits.eeRight:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(-2 + 
                            ((leSizes == ChamberSize.eeTall || leSizes == ChamberSize.eeDefault) ? 1 : 0), 0);
                        break;
                    case ChamberExits.eeMiddleRight:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(-2 + 
                            ((leSizes == ChamberSize.eeTall) ? 1 : 0), -1);
                        break;
                    case ChamberExits.eeBottomLeft:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(0, 1);
                        break;
                    case ChamberExits.eeBottomRight:
                        macChamberExitTranslations[(int)leSizes * 10 + (int)leExits] = new Vector2(-1, 1);
                        break;

                }
            }
        }
    }
}
