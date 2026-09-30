using Deck;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //Game object for Hand image
    private GameObject mcHand;

    //Game object for thumb image
    private GameObject mcThumb;

    //Animator used to control Hand/Thumb
    private Animator mcHandAnimator;
    private Animator mcThumbAnimator;

    //The Power state of the hand
    private HandState meHandState;

    //Damage the current hand has
    private int mnHandPotentialDamage = 0;

    // Start is called before the first frame update
    void Start()
    {
        meHandState = HandState.eeHandPhase_1;

        //Get Hand image components
        mcHand = transform.GetChild(0).gameObject;

        //Get the Thumb portion of the hand
        if(mcHand != null)
        {
            mcThumb = mcHand.transform.GetChild(0).gameObject;
        }

        if (mcThumb != null)
        {
            mcHandAnimator = mcHand.GetComponent<Animator>();
            mcThumbAnimator = mcThumb.GetComponent<Animator>();
        }
        else
        {
            Debug.Log("Failed to find Hand components");
        }
    }

    //Adjusts the state of the hand based on the potential damage of the hand
    public void AdjustHandState(int pnHandPotentialDamage)
    {
        if(pnHandPotentialDamage >= 60)
        {
            meHandState = HandState.eeHandPhase_5;
        }
        else if(pnHandPotentialDamage < 10)
        {
            meHandState = HandState.eeHandPhase_1;
        }
        else
        {
            meHandState = (HandState)(pnHandPotentialDamage / 10);
        }

        Debug.Log("Hand State: " + meHandState);
    }

    //Triggers the hand to perform the draw animation 
    public void DrawCard(int pnHandDamage)
    {
        mnHandPotentialDamage = pnHandDamage;
        StartCoroutine(DrawCardCoroutine());
    }

    public void OpenMap()
    {
        mcHandAnimator.Play("Hand_Draw_Map");
    }

    public void CloseMap()
    {
        mcHandAnimator.Play("Hand_Remove_Map");
    }

    IEnumerator DrawCardCoroutine()
    {
        //Play and wait for animation to finish
        mcHandAnimator.Play("HandPhase_" + (int)meHandState + "_Draw");

        //If Hand Phase has thumb component
        if((int)meHandState < 3)
        {
            mcThumbAnimator.Play("HandPhase_" + (int)meHandState + "_Thumb_Draw");
        }
        else
        {
            mcThumbAnimator.Play("ThumbEmpty");
        }

        yield return null;
        while (mcHandAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        //Check for Hand state change
        AdjustHandState(mnHandPotentialDamage);

        //Set hand back to rest state
        mcHandAnimator.Play("HandPhase_" + (int)meHandState + "_Rest");
        if ((int)meHandState < 3)
        {
            mcThumbAnimator.Play("HandPhase_" + (int)meHandState + "_Thumb_Rest");
        }
        else
        {
            mcThumbAnimator.Play("ThumbEmpty");
        }
    }

    //Plays the hand and resets to the empty state
    public void PlayHand()
    {
        StartCoroutine(PlayHandCoroutine());
    }

    IEnumerator PlayHandCoroutine()
    {
        //Play and wait for animation to finish
        mcHandAnimator.Play("HandPhase_" + (int)meHandState + "_Play");
        //If Hand Phase has thumb component
        if ((int)meHandState < 3)
        {
            mcThumbAnimator.Play("HandPhase_" + (int)meHandState + "_Thumb_Play");
        }
        else
        {
            mcThumbAnimator.Play("ThumbEmpty");
        }

        yield return null;
        while (mcHandAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        //Set hand back to Default state
        mcHandAnimator.Play("HandEmpty");
        mcThumbAnimator.Play("ThumbEmpty");

        meHandState = HandState.eeHandPhase_1;
        mnHandPotentialDamage = 0;


    }
}
