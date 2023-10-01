using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpHomeScript : MonoBehaviour
{
    // Start is called before the first frame update

    public void jumpHome(){
        CameraManager camManagr = GameObject.Find("Main Camera").GetComponent<CameraManager>();
        GameManagerScript gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

        Vector2 zoomToPos = new Vector2(0,0);

        //find PlanetScript object with owner that equals to gameManager.playerColour
        foreach(GameObject planet in gameManager.AllPlanets){
            if(planet.GetComponent<PlanetScript>().owner == gameManager.playerColour){
                zoomToPos = planet.transform.position;
                break;
            }
        }

        camManagr.startInitialZooming(zoomToPos,false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
