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

public class LevelManager : MonoBehaviour
{

    //List of chambers for level 1
    private List<GameObject> macLevel1Chambers = new();

    //List of chambers for level 2
    private List<GameObject> macLevel2Chambers = new();

    //List of chambers for level 3
    private List<GameObject> macLevel3Chambers = new();

    //Chambers for Current level
    private List<GameObject> CurrentLevelChambers = new();

    GameObject mcCurrentChamber = null;

    public Animator mcTransition;

    //Number of chambers to be added to level
    private int mnNumChambersInLevel = 5;

    //reference to camera used to change camera bounds
    public ConfinerUpdater mcConfinerUpdater;

    //2D translation from base point based on Chamber exit
    private Dictionary<int, Vector2> macChamberExitTranslations
        = new Dictionary<int, Vector2>();

    //Level Grid used to check overlap and reference map
    private LevelGrid mcLevelGrid;

    //MapController access
    public MapController MapControllerAccess;

    //Enemy manager used to create enemys in scene
    [SerializeField] private EnemyManager mcEnemyManager;

    //Set of Chamber offsets
    Vector2[,,,] macChamberPositionOffsets = new Vector2[(int)ChamberSize.eeLarge + 1, (int)ChamberSize.eeLarge + 1, (int)ChamberExits.eeNone, (int)ChamberExits.eeNone];


    // Start is called before the first frame update
    void Start()
    {
        mcLevelGrid = new LevelGrid();

        //Load chambers for level one
        LoadChambers(0);

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

        GenerateLevel(0);
    }

    /**
     * METHOD: Creates a Level Layout made of Level Chambers
     */
    private void GenerateLevel(int pnLevel)
    {

        CurrentLevelChambers.Clear();
        List<GameObject> lacLevelPool = new();

        //Debug.Log("Generate Level: " + pnLevel);

        //TODO Add different level pools
        if (pnLevel == 0)
        {
            lacLevelPool = macLevel1Chambers;
        }

        if (lacLevelPool.Count != 0)
        {
            //Add Start Chamber, Should Always be first in heirarchy 
            CurrentLevelChambers.Add(lacLevelPool[0]);

            //Set start chamber to current chamber
            mcCurrentChamber = CurrentLevelChambers[0];

            //Get Collection of random chambers to add to level
            for (int lnNumChambers = 1; lnNumChambers < mnNumChambersInLevel; lnNumChambers++)
            {
                int lbRandomRoom = UnityEngine.Random.Range(1, (lacLevelPool.Count));

                //Debug.Log("Add Chamber: " + lbRandomRoom);

                //Add the random room and remove from the pool
                CurrentLevelChambers.Add(lacLevelPool[lbRandomRoom]);
                lacLevelPool.RemoveAt(lbRandomRoom);
            }

            //Initialize each chamber before it is placed in level
            foreach (GameObject lcLevelChamber in CurrentLevelChambers)
            {
                lcLevelChamber.GetComponent<Chamber>().InitializeChamber();
                //Debug.Log("Initialize Chamber");
            }

            ChamberExits leEntrance = ChamberExits.eeNone;

            //Begin with Start Chamber
            Chamber lcChamber = CurrentLevelChambers[0].GetComponent<Chamber>();

            //Add Start chamber to grid
            mcLevelGrid.AddChamberToGrid(0, 0, CurrentLevelChambers[0]);

            MapControllerAccess.AddChamberToMap(new List<Vector2>() { new Vector2(0,0) }, CurrentLevelChambers[0]);

            int lnNumFailedExits = 0;
            int lnNextChamberToAdd = 1;

            //for each chamber in level that needs to be added
            for (int lnNumChambers = 0; lnNumChambers < mnNumChambersInLevel; lnNumChambers++)
            {
                lcChamber = CurrentLevelChambers[lnNumChambers].GetComponent<Chamber>();

                //TODO: refactor treasure populate

                mcEnemyManager.PopulateWithEnemies(lcChamber);

                //lcChamber.PopulateWithTreasure();
                if (lnNumChambers != 0)
                {
                    //lcChamber.PopulateWithEnemies();
                }

                //for each potential exit
                for (int lnNumExits = 0; lnNumExits < lcChamber.GetNumPotentialRooms(); lnNumExits++)
                {
                    //Room will be generated based on chance
                    if (AllocateChamber(lnNumFailedExits, lcChamber.GetNumPotentialRooms()) && lnNextChamberToAdd < mnNumChambersInLevel)
                    {
                        //Get Next available exit from this chamber
                        ChamberExits leChamberExit = lcChamber.GetValidExit(leEntrance);

                        //Determine Grid point from chamber exit chosen
                        Vector2 lcExitPoint = lcChamber.GetExitPoint(leChamberExit);

                        //Get added chamber 
                        Chamber lcAttachedChamber = CurrentLevelChambers[lnNextChamberToAdd].GetComponent<Chamber>();

                        //Get random entrance to new chamber that coincides with random exit of current chamber
                        ChamberExits leNewChamberEntrance = lcAttachedChamber.GetValidExit(leChamberExit);

                        //Check if randomized chamber can be added to grid
                        if (leNewChamberEntrance != ChamberExits.eeNone &&
                            AssignNextChamberCoordinates((int)lcExitPoint.x, (int)lcExitPoint.y, leNewChamberEntrance, CurrentLevelChambers[lnNextChamberToAdd]))
                        {
                            //Create Exit to the Next chamber in the set
                            lcChamber.CreateExit(leChamberExit, CurrentLevelChambers[lnNextChamberToAdd]);

                            //Create exit on added chamber
                            lcAttachedChamber.CreateExit(leNewChamberEntrance, CurrentLevelChambers[lnNumChambers]);

                            //Set chamber Entrance/Exit connections
                            lcChamber.ConnectExits(leChamberExit, leNewChamberEntrance);
                            lcAttachedChamber.ConnectExits(leNewChamberEntrance, leChamberExit);

                            //Shift new chamber 2D position to connect it to base chamber
                            PositionNewChamber(lcChamber.transform.position, lcChamber.GetChamberSize(), leChamberExit, lcAttachedChamber.GetChamberSize(), 
                                leNewChamberEntrance, CurrentLevelChambers[lnNextChamberToAdd]);

                            Debug.Log("New Chamber: " + CurrentLevelChambers[lnNextChamberToAdd].name + " Entrance: " + leNewChamberEntrance + 
                                "  Base Chamber: " + lcChamber.gameObject.name  + " Exit: " + leChamberExit);

                            mcConfinerUpdater.AddPolygonCameraCollider(CurrentLevelChambers[lnNextChamberToAdd].GetComponent<Chamber>().GetCameraCollider(),
                                CurrentLevelChambers[lnNextChamberToAdd].transform.position);

                            //Debug.Log("Chamber: " + CurrentLevelChambers[lnNumChambers].ToString() + "Create Exit " + leChamberExit + 
                            //   " -----> Chamber: " + CurrentLevelChambers[lnNextChamberToAdd].ToString() + "Create Entrance " + leNewChamberEntrance);

                            lnNextChamberToAdd++;
                        }
                        else
                        {
                            lnNumFailedExits++;
                        }
                    }
                    else
                    {
                        lnNumFailedExits++;
                    }

                    //If added chambers goes over max chambers stop generation
                    if(lnNextChamberToAdd >= mnNumChambersInLevel)
                    {
                        break;
                    }

                }

                lnNumFailedExits = 0;
            }
        }

        //Pathfinding rescan on new level
        AstarPath.active.Scan();
    }

    private bool AssignNextChamberCoordinates(int pnBaseChamberX, int pnBaseChamberY, 
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
            if(mcLevelGrid.IsGridSectionFilled((int)lcNewChamberPoint.x, (int)lcNewChamberPoint.y))
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
                mcLevelGrid.AddChamberToGrid((int)lcNewChamberPoint.x, (int)lcNewChamberPoint.y, pcNewChamber);

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
                lnBaseOffsetX -= 7;
            }
            else
            {
                lnBaseOffsetX += 7;
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

        pcNewChamber.transform.gameObject.SetActive(true);
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

        return lbCreateChamber;
    }

    /**
     * METHOD: Loads all chambers for a specified level
     */
    private void LoadChambers (int pnLevel)
    {
        //Get the desired level game object 
        GameObject lcLevelGameObject = transform.GetChild(pnLevel).gameObject;

        //Add all Chambers to LevelChambers
        foreach (Transform lcChamber in lcLevelGameObject.transform)
        {
            switch(pnLevel)
            {
                case 0:
                    lcChamber.position = Vector2.zero;
                    macLevel1Chambers.Add(lcChamber.gameObject);
                    break;
                default:
                    break;
            }
        }

        //Debug.Log("Loaded " + macLevel1Chambers.Count + " Chamber(s) for level " + (pnLevel + 1));
    }

    /*
     * METHOD: Loads the next tilemap
     */
    public (ChamberSize, ChamberExits) ChangeChamber(ChamberExits peChamberExit)
    {
        ChamberSize leNextChamberSize = ChamberSize.eeDefault;
        ChamberExits leChamberEntrance = ChamberExits.eeNone;

        if (mcCurrentChamber != null)
        {
            Chamber lcCurrentChamber = mcCurrentChamber.GetComponent<Chamber>();

            GameObject lcNextChamber = lcCurrentChamber.GetConnectedChamber(peChamberExit);

            leNextChamberSize = lcNextChamber.GetComponent<Chamber>().GetChamberSize();

            leChamberEntrance = lcCurrentChamber.GetConnectedChamberEntrance(peChamberExit);

            Debug.Log("Exit " + peChamberExit + " Entering " + leChamberEntrance);

            StartCoroutine(ChangeChamberCoroutine(leNextChamberSize, lcNextChamber));
        }
        else
        {
            Debug.Log("Current Chamber not set");
        }

        return (leNextChamberSize, leChamberEntrance);
    }

    IEnumerator ChangeChamberCoroutine(ChamberSize peChamberSize, GameObject pcNextChamber)
    {
        mcTransition.SetTrigger("Start");

        yield return new WaitForSeconds(0.5f);

        Chamber lcCurrentChamber = mcCurrentChamber.GetComponent<Chamber>();
        Chamber lcNextChamber = pcNextChamber.GetComponent<Chamber>();

        mcCurrentChamber.SetActive(false);

        lcCurrentChamber.Exit();

        //Reset Camera Confiner to size of next room
        mcConfinerUpdater.UpdateConfiner(peChamberSize);

        pcNextChamber.gameObject.SetActive(true);

        //Pathfinding rescan on new chamber for enemy AI
        AstarPath.active.Scan();

        lcNextChamber.Enter();

        yield return new WaitForSeconds(0.2f);

        mcTransition.SetTrigger("End");

        mcCurrentChamber = pcNextChamber;
    }

    //Creates list of chamber position offsets 
    private void InitializeChamberPositionOffsets()
    {
        int lnBaseOffsetX = 0;
        int lnBaseOffsetY = 0;

        for (int lnBaseChamberSize = 0; lnBaseChamberSize < (int)ChamberSize.eeLarge + 1; lnBaseChamberSize++)
        {
            for (int lnNewChamberSize = 0; lnNewChamberSize < (int)ChamberSize.eeLarge + 1; lnNewChamberSize++)
            {
                for (int lnBaseChamberExit = 0; lnBaseChamberExit < (int)ChamberExits.eeNone; lnBaseChamberExit++)
                {
                    for (int lnNewChamberEntrance = 0; lnNewChamberEntrance < (int)ChamberExits.eeNone; lnNewChamberEntrance++)
                    {
                        macChamberPositionOffsets[lnBaseChamberSize, lnNewChamberSize, lnBaseChamberExit, lnNewChamberEntrance] = new Vector2(0, 0);
                    }
                }
            }
        }
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
