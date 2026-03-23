using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingObjectControl : MonoBehaviour
{
    Rigidbody2D mcEndpointBody;

    [SerializeField] private float mfForceMultiplier = 1.0f;

    private float mfForceX = 1;

    // Start is called before the first frame update
    void Start()
    {
        //Get Rigid body on Endpoint of hanging object
        var lcEndpoint = transform.Find("Endpoint");

        if(lcEndpoint)
        {
            mcEndpointBody = lcEndpoint.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.Log("HangingObject: Couldnt find endpoint");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HangingObject: Add Force");
        mcEndpointBody.AddForce(new Vector2(mfForceX, 0.1f) * mfForceMultiplier, ForceMode2D.Impulse);

        mfForceX *= -1;
    }
}
