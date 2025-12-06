using Cinemachine;
using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfinerUpdater : MonoBehaviour
{
    CinemachineConfiner mcCinemachineConfiner;

    private List<PolygonCollider2D> macConfiners = new();

    private void Start()
    {
        //Get the Cinemachine Confiner 
        mcCinemachineConfiner = GameObject.Find("Virtual Camera").GetComponent<CinemachineConfiner>();

        //Get the Confiner size children
        foreach (Transform lcConfiner in transform)
        {
            macConfiners.Add(lcConfiner.gameObject.GetComponent<PolygonCollider2D>());
        }
    }

    public void UpdateConfiner(ChamberSize peChamberSize)
    {

        if(mcCinemachineConfiner != null)
        {
            mcCinemachineConfiner.m_BoundingShape2D = macConfiners[(int)peChamberSize];
        }
        else
        {
            Debug.Log("Cinemachine Confiner not attached to ConfinerUpdater");
        }
    }

}
