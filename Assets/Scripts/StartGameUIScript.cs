using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUIScript : MonoBehaviour
{

    GameManagerScript gameManager;
    GameSetupScript gameSetup;

    CameraManager cameraManager;

    public Slider enemiesSlider;
    public Slider planetsSlider;

    public AudioClip normalClickSound;
    public void StartGame(){
        Vector2 playerFirstPos = gameSetup.SpawnPlanetsAssignColours(3.0f,(int)planetsSlider.value, Mathf.Pow(planetsSlider.value,0.5f) * 5.0f ,(int)enemiesSlider.value + 1, gameManager.playerColour);
        //get owner of this script, canvas element
        AudioSource.PlayClipAtPoint(normalClickSound, Camera.main.transform.position);
        Destroy(gameObject);
        cameraManager.startInitialZooming(playerFirstPos);
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        gameSetup = GameObject.Find("GameManager").GetComponent<GameSetupScript>();
        cameraManager = GameObject.Find("Main Camera").GetComponent<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
