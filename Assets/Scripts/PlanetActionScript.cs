using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlanetActionScript : MonoBehaviour
{

    public GameManagerScript gameManager;
    public GameObject ChooseTargetUItxt;
    public UImanager uiManager;

    public void upgradeProductionSpeed(){

        uiManager.selectedPlanet.GetComponent<PlanetScript>().upgradeProductionSpeed();
    }

    public void chooseTarget(){
        gameManager.selectingAttackTarget = true;
        GameObject atkTXT = Instantiate(ChooseTargetUItxt, GameObject.Find("Canvas").transform);
        atkTXT.GetComponentInChildren<TextMeshProUGUI>().color = gameManager.playerColour;

        //set color of atkTXT to be the same as player color

        Destroy(gameObject);    
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();
    }

    // Update is called once per frame
    void Update()
    {
        
        GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(uiManager.selectedPlanet.transform.position) + new Vector3(0, -70, 0);


    }
}
