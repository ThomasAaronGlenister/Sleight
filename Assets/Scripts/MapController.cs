using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public GameObject mcMap;
    private GameObject mcMapBackground;

    //ChamberIcon Template prefab assigned in the editor
    public GameObject mcChamberIconTemplate;

    private bool mbMapShown = false;

    private List<GameObject> macChamberIcons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //Get the Map background
        mcMapBackground = transform.GetChild(0).gameObject;
    }

    public void AddChamberToMap(List<Vector2> pacGridSectors, GameObject pcChamber)
    {
        Color randomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

        Chamber lcNewChamber = pcChamber.GetComponent<Chamber>();

        foreach (Vector2 sector in pacGridSectors) 
        {
            //Create set of rotational display cards 
            GameObject lcChamberIconObject = Instantiate(
                mcChamberIconTemplate, new Vector3(0 + (50 * sector.x), 0 + (50 * sector.y)), Quaternion.identity);

            lcChamberIconObject.GetComponent<Image>().color = randomColor;

            lcChamberIconObject.transform.SetParent(mcMap.transform, false);

            if (pacGridSectors[0].x == 0 && pacGridSectors[0].y == 0)
            {
                lcChamberIconObject.SetActive(true);
            }

            lcNewChamber.AddMapIcons(lcChamberIconObject);

            macChamberIcons.Add(lcChamberIconObject);
        }
    }

    public void MapSwitch()
    {
        if (mbMapShown)
        {
            mcMap.SetActive(false);
            mcMapBackground.SetActive(false);

        }
        else
        {
            mcMap.SetActive(true);
            mcMapBackground.SetActive(true);
        }

        mbMapShown = !mbMapShown;
    }


}
