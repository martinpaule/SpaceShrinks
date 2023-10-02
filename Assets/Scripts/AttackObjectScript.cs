using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObjectScript : MonoBehaviour
{

    public PlanetScript targetPlanet;
    public Color myColor;

    float movementSpeed = 3.0f;

    float resources;

    public AudioClip attackSound;

    public void setupATKobject(PlanetScript target, Color ownerColor, float resourcesToTransfer){
        targetPlanet = target;
        myColor = ownerColor;
        //GetComponent<SpriteRenderer>().color = myColor;
        resources = resourcesToTransfer;
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioSource.PlayClipAtPoint(attackSound, Camera.main.transform.position);

    }

    // Update is called once per frame
    void Update()
    {

        if(targetPlanet == null){
            GameManagerScript gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            foreach(GameObject planet in gameManager.AllPlanets){
                if(planet.GetComponent<PlanetScript>().owner == myColor){
                    targetPlanet = planet.GetComponent<PlanetScript>();
                    break;
                }
            }
        }

        //move towards target planet
        transform.position = Vector2.MoveTowards(transform.position, targetPlanet.transform.position, movementSpeed * Time.deltaTime);

        //if close enough to target planet, transfer resources
        if(Vector2.Distance(transform.position, targetPlanet.transform.position) < 0.1f){
            Color oldTargetColor = targetPlanet.owner;
            targetPlanet.receiveDamage(resources, myColor);
            Color newTargetColor = targetPlanet.owner;

            GameManagerScript gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();   

           

            Destroy(gameObject);
        }

    }
}
