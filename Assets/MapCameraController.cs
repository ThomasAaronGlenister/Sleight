using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    //Map camera game object
    [SerializeField] private GameObject mcMapCamera;

    private Camera mcCameraComponent;

    private float mfBaseOrthographicSize;

    [SerializeField] private float mfCameraMoveSpeed = 1;
    [SerializeField] private float mfCameraRotateSpeed = 1;
    [SerializeField] private float mfCameraZoomSpeed = 1;

    private Vector2 mcCameraMovementVector = Vector2.zero;
    private Vector2 mcCameraRotateVector = Vector2.zero;

    private bool mbZoomEngage = false;
    private bool mbZoomIn = false;

    private Vector3 mcInitialCameraPosition;
    private Quaternion mcInitialCameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        //Set Base Components
        mcCameraComponent = mcMapCamera.GetComponent<Camera>();
        mfBaseOrthographicSize = mcCameraComponent.orthographicSize;
        mcInitialCameraPosition = mcMapCamera.transform.localPosition;
        mcInitialCameraRotation = mcMapCamera.transform.localRotation;
    }

    private void Update()
    {
        transform.localPosition += (Vector3)mcCameraMovementVector * mfCameraMoveSpeed * Time.deltaTime;

        mcCameraRotateVector *= mfCameraRotateSpeed;
        transform.localRotation *= Quaternion.Euler(0f, mcCameraRotateVector.x, 0f);

        if (mbZoomEngage)
        {
            //Zoom in/out camera based on time and zoom speed
            //mcCameraComponent.orthographicSize += mfCameraZoomSpeed * Time.deltaTime * ((mbZoomIn) ? -1 : 1);

            transform.localPosition += Vector3.forward * mfCameraZoomSpeed * Time.deltaTime * ((mbZoomIn) ? -1 : 1);
        }
    }

    public void ShiftCamera(Vector2 pcMovementVector)
    {
        mcCameraMovementVector = pcMovementVector;
    }

    public void RotateCamera(Vector2 pcRotateVector)
    {
        mcCameraRotateVector = pcRotateVector;
    }

    public void ZoomCamera(bool pbZoomEngage, bool pbZoomIn)
    {
        mbZoomEngage = pbZoomEngage;
        mbZoomIn = pbZoomIn;
    }
}
