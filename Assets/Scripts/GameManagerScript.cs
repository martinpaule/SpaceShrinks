using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    public List<GameObject> AllPlanets = new List<GameObject>();

    public Color playerColour = Color.red;

    public bool selectingAttackTarget = false;

    // Start is called before the first frame update
    AudioSource winLoseAudioSource;

    public AudioClip winSound;
    public AudioClip loseSound;

    public AudioClip NormalClickSound;

    public int troopAttackPercentage;

    bool gameWinCheck(){
        foreach (GameObject planet in AllPlanets)
        {
            if(planet.GetComponent<PlanetScript>().owner != playerColour && planet.GetComponent<PlanetScript>().owner != Color.white){
                return false;
            }
        }
        return true;
    }

    bool gameLoseCheck(){
        foreach (GameObject planet in AllPlanets)
        {
            if(planet.GetComponent<PlanetScript>().owner == playerColour){
                return false;
            }
        }
        return true;
    }

    public void gameWinLoseCheck(){
        if(gameWinCheck()){
            UImanager uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();
            uiManager.toggleEndgameUI(true);
            uiManager.toggleExit();
            winLoseAudioSource.clip = winSound;
            winLoseAudioSource.Play();
        }else if(gameLoseCheck()){
            UImanager uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();
            uiManager.toggleEndgameUI(false);
            uiManager.toggleExit();
            winLoseAudioSource.clip = loseSound;
            winLoseAudioSource.Play();
        }
    }

    void Start()
    {
        winLoseAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
