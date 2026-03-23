using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private CinemachineVirtualCamera mcVirtualCamera;
    private CinemachineBasicMultiChannelPerlin mcNoise;

    private void Awake()
    {
        mcVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        if(mcVirtualCamera)
        {
            mcNoise = mcVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            if(!mcNoise)
            {
                Debug.Log("Virtual Camera Perlin Not found");
            }
        }
        else
        {
            Debug.Log("Virtual Camera Not found");
        }
    }

    /**
     * Zooms Camera for specified range for a provided amount of time
     */
    public void SetZoom(float pfZoomTime = 0f)
    {
        if (pfZoomTime != 0)
        {
            StartCoroutine(FreezeFrames(pfZoomTime));
        }
    }

    //Freezes the time of the game for set period of time
    private IEnumerator FreezeFrames(float pfZoomTime)
    {
        this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 3;
        yield return new WaitForSecondsRealtime(pfZoomTime);
        this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 4.16f;
    }

    public void ScreenShake(float pfShakeDuration, float pfShakeAmplitude, float pfShakeFreq)
    {
        if (pfShakeDuration != 0)
        {
            StartCoroutine(ShakeScreen(pfShakeDuration, pfShakeAmplitude, pfShakeFreq));
        }
    }

    private IEnumerator ShakeScreen(float pfShakeTime, float pfShakeAmplitude, float pfShakeFreq)
    {
        mcNoise.m_AmplitudeGain = 0.5f;
        mcNoise.m_FrequencyGain = 60;

        yield return new WaitForSeconds(pfShakeTime);

        mcNoise.m_AmplitudeGain = 0;
        mcNoise.m_FrequencyGain = 0;
    }
}
