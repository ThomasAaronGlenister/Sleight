using Cinemachine;
using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq; // Required for Concat

public class ConfinerUpdater : MonoBehaviour
{
    CinemachineConfiner mcCinemachineConfiner;

    private List<PolygonCollider2D> macConfiners = new();

    public GameObject mcConfiningShape;

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

    public void AddPolygonCameraCollider(GameObject pcCameraCollider, Vector2 pcConfinerPosition)
    {
        //Set new camera collider to child of confiner
        pcCameraCollider.transform.SetParent(mcConfiningShape.transform);

        pcCameraCollider.transform.position = pcConfinerPosition;

        Debug.Log(pcCameraCollider.name + " position: " + pcConfinerPosition + " pos2: " + pcCameraCollider.transform.position);

        //Set Collider to be used by composite parent camera collider
        pcCameraCollider.GetComponent<PolygonCollider2D>().usedByComposite = true;
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
