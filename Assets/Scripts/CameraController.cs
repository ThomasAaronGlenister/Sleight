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

    //Total Time Zoom in is applied for
    public float mfZoomTime = 0;

    //Time taken to zoom in and out
    private float mfZoomInOutTime = 0.04f;
    public float mfElapsedTime = 0f;

    private float mfBaseOrthographicSize = 4.16f;
    private float mfZoomOrthographicSize = 3f;
    public bool mbZoomIn = false;
    private bool mbZoomOut = false;

    private float mfTimeDiff = 0;

    private bool mbShakeRoutineIsRunning = false;

    private void Awake()
    {
        mcVirtualCamera = GetComponent<CinemachineVirtualCamera>();

        mfBaseOrthographicSize = this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;

        if (mcVirtualCamera)
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

    private void Update()
    {
        if(mfZoomTime != 0)
        {
            if (mbZoomIn)
            {
                mfElapsedTime += Time.deltaTime;

                //Zoom in
                mfTimeDiff = (mfElapsedTime) / (mfZoomInOutTime);
                this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize
                    = Mathf.Lerp(mfBaseOrthographicSize, mfZoomOrthographicSize, mfTimeDiff);

                //Once finished zooming in wait for middle period
                if(mfElapsedTime >= mfZoomInOutTime)
                {
                    mbZoomIn = false;
                    mfElapsedTime = 0;
                }
            }

            //Wait period
            if(!mbZoomIn && !mbZoomOut)
            {
                mfElapsedTime += Time.unscaledDeltaTime;

                Time.timeScale = 0.3f;

                //Wait zoomed in for period outside of zoom in/out time
                if (mfElapsedTime >= mfZoomTime - (mfZoomInOutTime * 2))
                {
                    mbZoomOut = true;
                    mfElapsedTime = 0;

                    Time.timeScale = 1f;
                }
            }

            if (mbZoomOut)
            {
                mfElapsedTime += Time.deltaTime;

                //Zoom out
                mfTimeDiff = (mfElapsedTime) / (mfZoomInOutTime);
                this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize
                    = Mathf.Lerp(mfZoomOrthographicSize, mfBaseOrthographicSize, mfTimeDiff);

                //End Cycle once zoomed back out
                if (mfElapsedTime >= mfZoomInOutTime)
                {
                    mbZoomOut = false;
                    mfElapsedTime = 0;
                    mfZoomTime = 0;
                }
            }
        }
    }

    /**
     * Zooms Camera for specified range for a provided amount of time
     */
    public void SetZoom(float pfZoomTime = 0f, float pfZoomIntensity = 1f)
    {
        mfZoomTime = pfZoomTime;
        mfZoomOrthographicSize = mfBaseOrthographicSize * pfZoomIntensity;
        mbZoomIn = true;
    }

    public void ScreenShake(float pfShakeDuration, float pfShakeAmplitude, float pfShakeFreq)
    {
        if (pfShakeDuration != 0 && !mbShakeRoutineIsRunning)
        {
            StartCoroutine(ShakeScreen(pfShakeDuration, pfShakeAmplitude, pfShakeFreq));
        }
    }

    private IEnumerator ShakeScreen(float pfShakeTime, float pfShakeAmplitude, float pfShakeFreq)
    {
        mbShakeRoutineIsRunning = true;

        mcNoise.m_AmplitudeGain = pfShakeAmplitude;
        mcNoise.m_FrequencyGain = pfShakeFreq;

        yield return new WaitForSecondsRealtime(pfShakeTime);

        mcNoise.m_AmplitudeGain = 0;
        mcNoise.m_FrequencyGain = 0;

        mbShakeRoutineIsRunning = false;
    }
}
