using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;


public class PlanetActionScript : MonoBehaviour
{

    public GameManagerScript gameManager;
    public GameObject ChooseTargetUItxt;
    public UImanager uiManager;

    public AudioClip PositiveClickSound;

    public AudioClip NegativeClickSound;

    public AudioClip normalClickSound;
    GameObject UpgradeButton;

    Color UpgradeuButtonColor;



    public void upgradeProductionSpeed(){

        float resources = uiManager.selectedPlanet.GetComponent<PlanetScript>().resources;

        uiManager.selectedPlanet.GetComponent<PlanetScript>().upgradeProductionSpeed();

        if(resources == uiManager.selectedPlanet.GetComponent<PlanetScript>().resources){
            AudioSource.PlayClipAtPoint(NegativeClickSound, Camera.main.transform.position);
            UpgradeButton.GetComponent<Image>().color = Color.red;
        }else{
            AudioSource.PlayClipAtPoint(PositiveClickSound, Camera.main.transform.position);
            UpgradeButton.GetComponent<Image>().color = Color.green;

        }

        
        UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "UPGRADE\n(" + uiManager.selectedPlanet.GetComponent<PlanetScript>().upgradeCost.ToString() + ")";;
    }

    public void chooseTarget(){
        gameManager.selectingAttackTarget = true;
        GameObject atkTXT = Instantiate(ChooseTargetUItxt, GameObject.Find("Canvas").transform);
        atkTXT.GetComponentInChildren<TextMeshProUGUI>().color = gameManager.playerColour;
        AudioSource.PlayClipAtPoint(normalClickSound, Camera.main.transform.position);

        Slider atkPercSlider = GameObject.Find("AttackPercSlider").GetComponent<Slider>();

        //set color of atkTXT to be the same as player color
        gameManager.troopAttackPercentage = (int)atkPercSlider.value;
        Destroy(gameObject);    
    }

    // Start is called before the first frame update
    void Start()
    {
        UpgradeButton = GameObject.Find("UpgradeButton");
        UpgradeuButtonColor = UpgradeButton.GetComponent<Image>().color;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();
        AudioSource.PlayClipAtPoint(normalClickSound, Camera.main.transform.position);

        //set z of position to 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
        GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(uiManager.selectedPlanet.transform.position) + new Vector3(0, -70, 0);

        Image UpgImg = UpgradeButton.GetComponent<Image>();
        if(UpgImg.color != UpgradeuButtonColor){
            UpgImg.color = Color.Lerp(UpgImg.color, UpgradeuButtonColor, 4.0f * Time.deltaTime);

            //check if its really close to the original color
            if(Vector4.Distance(UpgImg.color, UpgradeuButtonColor) < 0.1f){
                UpgImg.color = UpgradeuButtonColor;
            }
        }

    }

    //if player clicked anywhere on me (my Panel), debug ClickedOnMe
    
}
