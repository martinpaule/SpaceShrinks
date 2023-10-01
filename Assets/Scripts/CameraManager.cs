using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public float zoomSpeed = 10.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 20.0f;

    Vector3 dragOrigin;

    Camera myCamera;

    float LMBheldDownTime = 0;

    bool initialZoomingToPlayer = false;

    Vector3 initialZoomingToPlayerTarget;

    public UImanager uiManager;
    GameSetupScript gameSetupScript;

    OuterEdgesScript outerEdgesScript;

    public void startInitialZooming(Vector2 positionToZoomTO, bool setZoomToMax = true){
        initialZoomingToPlayer = true;
        initialZoomingToPlayerTarget = new Vector3(positionToZoomTO.x, positionToZoomTO.y, transform.position.z);

        if(setZoomToMax){

            myCamera.orthographicSize = maxZoom;
        }
    }

    void handleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float newSize = Mathf.Clamp(myCamera.orthographicSize - scrollInput * zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        myCamera.orthographicSize = newSize;
    }
    
    void handleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LMBheldDownTime = 0;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        LMBheldDownTime += Time.deltaTime;
        if (LMBheldDownTime < 0.1f) return;

        /*if(uiManager.upgradeUI){
            Destroy(uiManager.upgradeUI);
            uiManager.upgradeUI = null;
            uiManager.selectedPlanet = null;
        }*/

        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 move = dragOrigin - currentMousePos;

        transform.position += new Vector3(move.x, move.y, 0);


        float distanceCameraFromCentre = Vector2.Distance(transform.position, gameSetupScript.planetsCentre);
        if(distanceCameraFromCentre > outerEdgesScript.innerRadius){
            Vector2 direction = (Vector2)transform.position - gameSetupScript.planetsCentre;
            transform.position = gameSetupScript.planetsCentre + direction.normalized * outerEdgesScript.innerRadius;
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }

        dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void zoomToPlayerGradually(){
        float zoomSpeed = 4.0f;
        float targetZoom = 5.0f;
        float currentZoom = myCamera.orthographicSize;
        float newZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);
        myCamera.orthographicSize = newZoom;

        //handle position of camera too, gradually moving to the pos of initialZoomingToPlayerTarget
        float posSpeed = 4.0f;
        Vector3 targetPos = initialZoomingToPlayerTarget;
        Vector3 currentPos = transform.position;
        Vector3 newPos = Vector3.Lerp(currentPos, targetPos, posSpeed*Time.deltaTime);
        transform.position = newPos;

        //if close, initialZoomingToPlayer to false
        if(Vector3.Distance(newPos, targetPos) < 0.1f){
            initialZoomingToPlayer = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myCamera = GetComponent<Camera>();
        uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();
        outerEdgesScript = GameObject.Find("OuterEdgesManager").GetComponent<OuterEdgesScript>();
        gameSetupScript = GameObject.Find("GameManager").GetComponent<GameSetupScript>();
    }

    // Update is called once per frame
    void Update()
    {

        if(initialZoomingToPlayer){
            zoomToPlayerGradually();
        }else{
            handleZoom();
            handleDrag();
        }

    }
}
