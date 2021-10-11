using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private InputManager inputMan;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform cameraTarget;

    public Camera cam;

    [Header("Variables")]
    public float mouseLeftBound;
    public float mouseRightBound;
    public float mouseUpBound;
    public float mouseDownBound;
    public float moveAheadMax;
    public float mouseAheadMax;
    public float xChange;
    public float yChange;
    public float camVelX;
    public float camVelY;

    public bool isTrackingMovement;
    public States.CameraMode currMode;

    private Vector3 mousePos;
    private Vector3 targetPos;

    /*============================================================================
     * DEFAULT UNITY METHODS
     ============================================================================*/
    private void Start()
    {
        inputMan = GameObject.Find("[MANAGER]").GetComponent<InputManager>();
        cam = GetComponent<Camera>();
        playerPrefab = GameObject.Find("PlayerPrefab");
        SetCameraTarget("PlayerPrefab");
        SyncScreen();
        SetCameraMode(States.CameraMode.Shooter);
    }

    private void FixedUpdate()
    {
        xChange = yChange = 0.0f;
        targetPos = cameraTarget.position;
        CursorBiasUpdate();
        if (cameraTarget.CompareTag("Player"))
        {
            //CharMoveBiasUpdate()
            
            switch (currMode)
            {
                case (States.CameraMode.Platformer):
                    transform.position = targetPos + new Vector3(0, 0, -8.0f) + new Vector3(xChange, yChange, 0);
                    break;
                case (States.CameraMode.Shooter):
                    transform.position = new Vector3(targetPos.x, 10.0f, targetPos.z);
                    break;
                case (States.CameraMode.RPG):
                    break;
            }
        }

        //Incorporate averageing of mouse position, movement, character position later
        
    }

    /*============================================================================
     * GAMEPLAY UPDATE METHODS
     ============================================================================*/
    private void CursorBiasUpdate()
    {
        Vector3 playerScreenPos = cam.WorldToScreenPoint(playerPrefab.transform.position);
        playerScreenPos = new Vector3(playerScreenPos.x, playerScreenPos.y, 0);
        Vector3 cursorScreenPos = new Vector3(inputMan.inputMX, inputMan.inputMY, 0);
        Vector3 dirVec = cursorScreenPos - playerScreenPos;
        float adjustedDist = Mathf.Clamp(dirVec.magnitude, 0, mouseAheadMax);

        Vector3 res = dirVec.normalized * adjustedDist;

        Debug.DrawLine(playerPrefab.transform.position, playerPrefab.transform.position + res, Color.red);
    }

    private void CharMoveBiasUpdate()
    {
        if (isTrackingMovement)
        {
            Vector3 currVel = playerPrefab.GetComponent<Rigidbody>().velocity;
            float xDir = Mathf.Abs(currVel.x) > 0 ? currVel.x/Mathf.Abs(currVel.x) : 0;// * 0.1f);
            float yDir = Mathf.Abs(currVel.y) > 0 ? currVel.y/Mathf.Abs(currVel.y) : 0;// * 0.1f);

            Vector3 lookAheadPos = new Vector3(xDir, yDir, 0).normalized * moveAheadMax;
            if (transform.position.x != lookAheadPos.x)
            {
                xChange = xDir;// * camVelX;
                yChange = yDir;// * camVelY;
            }
        }
    }

    /*============================================================================
     * CAMERA METHODS
     ============================================================================*/
    private void SetCameraTarget(string targetStr)
    {
        cameraTarget = GameObject.Find(targetStr).transform;
        isTrackingMovement = targetStr == "PlayerPrefab";
    }

    public void SetCameraMode(States.CameraMode mode)
    {
        currMode = mode;
        switch (currMode)
        {
            case (States.CameraMode.Platformer):
                cam.orthographic = true;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case (States.CameraMode.Shooter):
                cam.orthographic = false;
                transform.rotation = Quaternion.Euler(90, 0, 0);
                break;

        }
    }

    /*============================================================================
     * MISC METHODS
     ============================================================================*/
    [ContextMenu("Reset to Default")]
    private void SetDefaultValues()
    {
        moveAheadMax = 15.0f;
        mouseAheadMax = 450.0f;
        xChange = 0.0f;
        yChange = 0.0f;
        camVelX = 0.1f;
        camVelY = 0.1f;

        isTrackingMovement = true;
        SetCameraMode(States.CameraMode.Platformer);
    }

    private void SyncScreen()
    {
        int xShift = 500, yShift = 300;
        mouseLeftBound = Screen.width/2 - xShift;
        mouseRightBound = Screen.width/2 + xShift;
        mouseUpBound = Screen.height/2 + yShift;
        mouseDownBound = Screen.height/2 - yShift;
    }
}