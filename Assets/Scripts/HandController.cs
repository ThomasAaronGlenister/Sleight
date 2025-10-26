using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //Game object for Hand image
    GameObject mcHand;

    //Game object for thumb image
    GameObject mcThumb;

    Animator mcHandAnimator;
    Animator mcThumbAnimator;

    // Start is called before the first frame update
    void Start()
    {
        //Get child image components
        mcHand = transform.GetChild(0).gameObject;
        mcThumb = transform.GetChild(1).gameObject;

        //Get Animators
        mcHandAnimator = mcHand.GetComponent<Animator>();
        mcThumbAnimator = mcThumb.GetComponent<Animator>();
    }

    public void AnimateHand()
    {
        mcHandAnimator.SetTrigger("Reset");
        mcThumbAnimator.SetTrigger("Reset");

        mcHandAnimator.SetTrigger("HandTrigger");
        mcThumbAnimator.SetTrigger("ThumbTrigger");

    }

    public void PlayHand()
    {
        mcHandAnimator.SetTrigger("Reset");
        mcThumbAnimator.SetTrigger("Reset");

        mcHandAnimator.SetTrigger("HandPlay");
        mcThumbAnimator.SetTrigger("ThumbPlay");

    }
}
