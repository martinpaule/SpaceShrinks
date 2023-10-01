using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlanetScript : MonoBehaviour
{

    public Color owner = Color.white;

    public TextMeshPro myText;


    public float resources = 0;

    public float productionSpeed = 1;

    public GameObject attackObjectPF;
    public GameManagerScript gameManager;

    public List<PlanetScript> closestPlanets = new List<PlanetScript>();

    public float upgradeCost = 30;
    AudioSource myAudioSource; 
    public AudioClip playerTakeoverSound;


    public void SetOwner(Color newOwner)
    {
        owner = newOwner;
        GetComponent<SpriteRenderer>().color = owner;
        //set text color to black
        myText.color = Color.black;
    }

    void AIupdate(){

        //check if the ai can attack someone
        foreach (PlanetScript planet in closestPlanets){
            if(planet.owner != owner && resources > planet.resources * 1.2f && resources > 1 && Random.Range(0, 1000) == 50){
                SendAttack(this, planet, resources);
                return;
            }
        }

        if(resources >= upgradeCost && Random.Range(0, 1000) == 50){
            upgradeProductionSpeed();
            updateText();
        }

    }

    void getClosestPlanets(){

        closestPlanets.Clear();
        
        List<PlanetScript> diffColorPlanets = new List<PlanetScript>();
        List<PlanetScript> ThreeClosest = new List<PlanetScript>();

        //gets all planets of different color than this planet
        foreach (GameObject planet in gameManager.AllPlanets)
        {
            if(planet.GetComponent<PlanetScript>().owner != owner){
                diffColorPlanets.Add(planet.GetComponent<PlanetScript>());
            }
        }



        foreach (PlanetScript planet_ in diffColorPlanets){
            if(ThreeClosest.Count < 3){
                ThreeClosest.Add(planet_);
            }else{
                float distanceToHere = Vector2.Distance(transform.position, planet_.transform.position);
                for(int i = 0; i < 3; i++){
                    float inThreeDistance = Vector2.Distance(transform.position, ThreeClosest[i].transform.position);
                    if(distanceToHere < inThreeDistance){
                        ThreeClosest[i] = planet_;
                        break;
                    }
                }
            }
        }
        closestPlanets = ThreeClosest;
    }

    // This method is called whenever the mouse is clicked over the collider of the GameObject this script is attached to
    private void OnMouseDown()
    {
        Color playerColor = gameManager.playerColour;
        UImanager uiManager = GameObject.Find("Canvas").GetComponent<UImanager>();

        if(gameManager.selectingAttackTarget){

                SendAttack(uiManager.selectedPlanet.GetComponent<PlanetScript>(), this, uiManager.selectedPlanet.GetComponent<PlanetScript>().resources);


                gameManager.selectingAttackTarget = false;
                uiManager.selectedPlanet = null;
                Destroy(GameObject.Find("SelectTargetTXT(Clone)"));
                return;
        }


        if(owner == playerColor){
            if(uiManager.selectedPlanet == this.gameObject){
                uiManager.selectedPlanet = null;
                Destroy(uiManager.upgradeUI);
            }else{
                uiManager.selectedPlanet = this.gameObject;

                if(uiManager.upgradeUI){
                    Destroy(uiManager.upgradeUI);
                }
                uiManager.upgradeUI = Instantiate(uiManager.upgradeUI_PF, GameObject.Find("Canvas").transform);
                GameObject UpgradeButton = GameObject.Find("UpgradeButton");
                UpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "UPGRADE\n(" + upgradeCost.ToString() + ")";;

                //set position of UI to be right below the selected planet
                uiManager.upgradeUI.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, -70, 0);
            }
        }
    }

    void SendAttack(PlanetScript from, PlanetScript to, float resourcesToSend)
    {
        GameObject attackObject = Instantiate(attackObjectPF, from.transform.position, Quaternion.identity);
        GameObject atkTXT = Instantiate(from.GetComponentInChildren<TextMeshPro>().gameObject, attackObject.transform);
        attackObject.GetComponent<AttackObjectScript>().setupATKobject(to, from.owner, from.resources);
        from.resources = 0;
    }

    public void upgradeProductionSpeed()
    {

        if (resources >= upgradeCost)
        {
            resources -= upgradeCost;
            productionSpeed *= 1.5f;
            upgradeCost *= 2;
        }
    }

    public void receiveDamage(float damage, Color attacker)
    {

        if(attacker == owner){
            resources += damage;
            updateText();
            return;
        }

        resources -= damage;

        if (resources < 0)
        {
            if(owner != Color.white){
                productionSpeed = 1.0f;
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                upgradeCost = 30;
            }

            SetOwner(attacker);
            resources = -resources;

            if(owner == gameManager.playerColour){
                myAudioSource.clip = playerTakeoverSound;
                myAudioSource.Play();
            }

            foreach(GameObject planet in gameManager.AllPlanets){
                if(planet.GetComponent<PlanetScript>().owner == attacker){
                    planet.GetComponent<PlanetScript>().getClosestPlanets();
                }
            }
        }
        updateText();



        gameManager.gameWinLoseCheck();
    }



    public void setupPlanet()
    {
        myText = GetComponentInChildren<TextMeshPro>();

        switch (Random.Range(0, 3))
        {
            case 0:
                resources = 10;
                productionSpeed = 0.5f;
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                break;
            case 1:
                resources = 50;
                productionSpeed = 1.0f;
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                break;
            case 2:
                resources = 150;
                productionSpeed = 1.5f;
                transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                break;
        }
    }


    public void updateText()
    {

        string asString = ((int)resources).ToString();

        switch (asString.Length)
        {
            case 1:
                myText.fontSize = 9;
                break;
            case 2:
                myText.fontSize = 7;
                break;
            case 3:
                myText.fontSize = 5;
                break;
            case 4:
                myText.fontSize = 4;
                break;
        }

        myText.text = asString;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(owner == Color.white){
            myText.color = Color.gray;
        }else{
            getClosestPlanets();
        }
                myAudioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        if (owner != Color.white)
        {
            resources += productionSpeed * Time.deltaTime;
            updateText();

            if(owner != gameManager.playerColour){
                AIupdate();
            }

        }

    }
}
