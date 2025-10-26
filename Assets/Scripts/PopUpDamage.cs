using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpDamage : MonoBehaviour
{
    public Vector2 mcInitialVelocity;
    public Rigidbody2D mcRigidbody;
    public float mfLifetime = 1.0f;
    public bool mbHitFromRight = false;

    // Start is called before the first frame update
    void Start()
    {
        if (mbHitFromRight)
            mcInitialVelocity.x *= -1;

        mcRigidbody.velocity = mcInitialVelocity;
        Destroy(gameObject, mfLifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
