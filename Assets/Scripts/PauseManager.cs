using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    //Pause bool set when player pauses
    private bool mbIsPaused = false;

    /**
     * Sets pause flag or freeze frames
     */
    public void SetFreeze(float pfPauseTime = 0f)
    {
        if (pfPauseTime != 0)
        {
            StartCoroutine(FreezeFrames(pfPauseTime));
        }
        else
        {
            mbIsPaused = !mbIsPaused;
            Time.timeScale = mbIsPaused ? 0f : 1f;
        }
    }

    //Freezes the time of the game for set period of time
    private IEnumerator FreezeFrames(float pfFreezeTime)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(pfFreezeTime);

        //Only unFreeze if pause not set
        if (!mbIsPaused )
        {
            Time.timeScale = 1f;
        }
    }
}
