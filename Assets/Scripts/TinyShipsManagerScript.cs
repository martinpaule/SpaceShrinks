using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyShipsManagerScript : MonoBehaviour
{

    

    public class Ship{

        public int level;

        public bool active;

        public GameObject shipObject;
        public Vector2 directionVector;
        public Vector2 desiredLocalPosition;
        public float speed;
        public float turningStrength;

        public Ship(int level_){
            level = level_;
            active = false;
            shipObject = null;
            directionVector = Vector2.zero;
            desiredLocalPosition = Vector2.zero;
            speed = 0f;
            turningStrength = 0f;
        }
    }

    public bool attacking = false;

    public GameObject ShipLevelOnePF;
    public GameObject ShipLevelTwoPF;

    public GameObject ShipLevelThreePF;

    List<Ship> shipsLevelOne = new List<Ship>();
    List<Ship> shipsLevelTwo = new List<Ship>();

    List<Ship> shipsLevelThree = new List<Ship>();

    List<Ship> shipsLevelOnePool = new List<Ship>();
    List<Ship> shipsLevelTwoPool = new List<Ship>();
    List<Ship> shipsLevelThreePool = new List<Ship>();

    int totalResourceNumber = 0;

    void setShipRandomDesiredLocalPosition(Ship ship){

        Vector2 randomPosition = Random.insideUnitSphere * 2.0f;
        ship.desiredLocalPosition = randomPosition;
    }
    //explanation: each ship represents an amount of resources, levels 1-3 being 1,10,100 resources respectively

    public void setAllShipsColor(Color setColor){

        foreach(Ship ship in shipsLevelOne){
            ship.shipObject.GetComponent<SpriteRenderer>().color = setColor;
        }
        foreach(Ship ship in shipsLevelTwo){
            ship.shipObject.GetComponent<SpriteRenderer>().color = setColor;
        }
        foreach(Ship ship in shipsLevelThree){
            ship.shipObject.GetComponent<SpriteRenderer>().color = setColor;
        }

        //set pool too
        foreach(Ship ship in shipsLevelOnePool){
            ship.shipObject.GetComponent<SpriteRenderer>().color = setColor;
        }
        foreach(Ship ship in shipsLevelTwoPool){
            ship.shipObject.GetComponent<SpriteRenderer>().color = setColor;
        }
        foreach(Ship ship in shipsLevelThreePool){
            ship.shipObject.GetComponent<SpriteRenderer>().color = setColor;
        }
    }

    public void SetupPooledObjects()
    {




        for(int i = 0; i < 9; i++){
            //Create pool of ships
            shipsLevelOnePool.Add(new Ship(1));
            shipsLevelTwoPool.Add(new Ship(2));
            shipsLevelThreePool.Add(new Ship(3));

            //Instantiate ships and set this as their parent
            shipsLevelOnePool[i].shipObject = Instantiate(ShipLevelOnePF, transform);
            shipsLevelTwoPool[i].shipObject = Instantiate(ShipLevelTwoPF, transform);
            shipsLevelThreePool[i].shipObject = Instantiate(ShipLevelThreePF, transform);
            
            //set initial local position to zero
            shipsLevelOnePool[i].shipObject.transform.localPosition = new Vector3(0, 0, -1);
            shipsLevelTwoPool[i].shipObject.transform.localPosition = new Vector3(0, 0, -1);
            shipsLevelThreePool[i].shipObject.transform.localPosition = new Vector3(0, 0, -1);

            //Set ships to inactive
            shipsLevelOnePool[i].active = false;
            shipsLevelTwoPool[i].active = false;
            shipsLevelThreePool[i].active = false;

            //turn off their sprite renderer components and all children too
            shipsLevelOnePool[i].shipObject.GetComponent<SpriteRenderer>().enabled = false;
            shipsLevelTwoPool[i].shipObject.GetComponent<SpriteRenderer>().enabled = false;
            shipsLevelThreePool[i].shipObject.GetComponent<SpriteRenderer>().enabled = false;

            //turn off child sprite renderer components
            for(int l = 0; l < 2; l++){

                shipsLevelOnePool[i].shipObject.transform.GetChild(l).GetComponent<SpriteRenderer>().enabled = false;
                shipsLevelTwoPool[i].shipObject.transform.GetChild(l).GetComponent<SpriteRenderer>().enabled = false;
                shipsLevelThreePool[i].shipObject.transform.GetChild(l).GetComponent<SpriteRenderer>().enabled = false;
            }
            

            //give ships random initial spawn and desired positions
            setShipRandomDesiredLocalPosition(shipsLevelOnePool[i]);
            setShipRandomDesiredLocalPosition(shipsLevelTwoPool[i]);
            setShipRandomDesiredLocalPosition(shipsLevelThreePool[i]);

            //set ships to their respective levels
            shipsLevelOnePool[i].level = 1;
            shipsLevelTwoPool[i].level = 2;
            shipsLevelThreePool[i].level = 3;

            //set random intial direction vectors
            shipsLevelOnePool[i].directionVector = (Vector2)Random.insideUnitSphere;
            shipsLevelTwoPool[i].directionVector = (Vector2)Random.insideUnitSphere;
            shipsLevelThreePool[i].directionVector = (Vector2)Random.insideUnitSphere;

            //set their movement speeds
            shipsLevelOnePool[i].speed = 0.9f;
            shipsLevelTwoPool[i].speed = 0.6f;
            shipsLevelThreePool[i].speed = 0.3f;
            
            //set turning strength
            shipsLevelOnePool[i].turningStrength = 8.0f;
            shipsLevelTwoPool[i].turningStrength = 10.0f;
            shipsLevelThreePool[i].turningStrength = 4.0f;
        }
    }

    void updateShipDirectionVector(Ship ship){
        //update the direction vector of the ship to point towards its desired position

        Vector2 directionVector = ship.directionVector;
        Vector2 desiredLocalPosition = ship.desiredLocalPosition;

        // Interpolation factor
        float t = Time.deltaTime * ship.speed * ship.turningStrength;
        t = Mathf.Clamp01(t);

        // Spherically interpolate between current position and target position
        ship.directionVector = Vector3.Slerp(directionVector.normalized, desiredLocalPosition.normalized, t);
    }

    void updateShipPositions(){

        //move all ships of level One to their desired positions
        foreach(Ship ship in shipsLevelOne){
            //update the direction vector of the ship to point towards its desired position
            //updateShipDirectionVector(ship);


            //keep rotating the ship along Z axis, like UFO using TurningStrength
            ship.shipObject.transform.Rotate(0, 0, ship.turningStrength * Time.deltaTime);


            Vector2 directionVector = ship.desiredLocalPosition - (Vector2)ship.shipObject.transform.localPosition;

            ship.shipObject.transform.localPosition += (Vector3)directionVector * ship.speed * Time.deltaTime;

            //if close, set new desired position
            if(Vector2.Distance(ship.shipObject.transform.localPosition, ship.desiredLocalPosition) < 0.1f && !attacking){
                setShipRandomDesiredLocalPosition(ship);
            }
        }

        //move all ships of level Two to their desired positions
        foreach(Ship ship in shipsLevelTwo){
            //update the direction vector of the ship to point towards its desired position
            //updateShipDirectionVector(ship);

            Vector2 directionVector = ship.desiredLocalPosition - (Vector2)ship.shipObject.transform.localPosition;

            //move the ship
            ship.shipObject.transform.localPosition += (Vector3)directionVector * ship.speed * Time.deltaTime;

            //if close, set new desired position
            if(Vector2.Distance(ship.shipObject.transform.localPosition, ship.desiredLocalPosition) < 0.1f && !attacking){
                setShipRandomDesiredLocalPosition(ship);

                Vector2 newDirectionVector = ship.desiredLocalPosition - (Vector2)ship.shipObject.transform.localPosition;
                //set rotation of the ship triangle (meaning vector up) to be facign the direction of the desired position
                ship.shipObject.transform.GetChild(0).transform.up = newDirectionVector;
            }
        }
        //move all ships of level Two to their desired positions
        foreach(Ship ship in shipsLevelThree){
            //update the direction vector of the ship to point towards its desired position
            //updateShipDirectionVector(ship);


            //keep rotating the ship along Z axis, like UFO using TurningStrength
            ship.shipObject.transform.Rotate(0, 0, ship.turningStrength * Time.deltaTime);
            //move the ship
            Vector2 directionVector = ship.desiredLocalPosition - (Vector2)ship.shipObject.transform.localPosition;

            ship.shipObject.transform.localPosition += (Vector3)directionVector * ship.speed * Time.deltaTime;

            //if close, set new desired position
            if(Vector2.Distance(ship.shipObject.transform.localPosition, ship.desiredLocalPosition) < 0.1f && !attacking){
                setShipRandomDesiredLocalPosition(ship);
            }
        }
    }

    public void setResourcesAmount(int newAmount){

        totalResourceNumber = newAmount;
        
        int correctLevelThreeAmount = totalResourceNumber / 100;
        int correctLevelTwoAmount = (totalResourceNumber - correctLevelThreeAmount * 100) / 10;
        int correctLevelOneAmount = totalResourceNumber - correctLevelThreeAmount * 100 - correctLevelTwoAmount * 10;

        int currentLevelThreeAmount = shipsLevelThree.Count;
        int currentLevelTwoAmount = shipsLevelTwo.Count;
        int currentLevelOneAmount = shipsLevelOne.Count;

        int levelThreeDifference = correctLevelThreeAmount - currentLevelThreeAmount;
        int levelTwoDifference = correctLevelTwoAmount - currentLevelTwoAmount;
        int levelOneDifference = correctLevelOneAmount - currentLevelOneAmount;

        if(levelThreeDifference == 0 && levelTwoDifference == 0 && levelOneDifference == 0){
            return;
        }

        //add/remove level Three ships
        if(levelThreeDifference > 0){
            for(int i = 0; i < levelThreeDifference; i++){
                shipsLevelThree.Add(shipsLevelThreePool[shipsLevelThreePool.Count - 1]);
                shipsLevelThreePool.RemoveAt(shipsLevelThreePool.Count - 1);

                //set ship to active
                shipsLevelThree[shipsLevelThree.Count - 1].active = true;
                //turn on sprite renderer
                shipsLevelThree[shipsLevelThree.Count - 1].shipObject.GetComponent<SpriteRenderer>().enabled = true;
                //turn on for child aswell
                
                //turn off child sprite renderer components
                shipsLevelThree[shipsLevelThree.Count - 1].shipObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                shipsLevelThree[shipsLevelThree.Count - 1].shipObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;

            }
        }else if(levelThreeDifference < 0){
            for(int i = 0; i < Mathf.Abs(levelThreeDifference); i++){
                shipsLevelThreePool.Add(shipsLevelThree[shipsLevelThree.Count - 1]);
                shipsLevelThree.RemoveAt(shipsLevelThree.Count - 1);

                //set ship to inactive
                shipsLevelThreePool[shipsLevelThreePool.Count - 1].active = false;
                //turn off sprite renderer
                shipsLevelThreePool[shipsLevelThreePool.Count - 1].shipObject.GetComponent<SpriteRenderer>().enabled = false;
                //turn off for child aswell
                shipsLevelThreePool[shipsLevelThreePool.Count - 1].shipObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                shipsLevelThreePool[shipsLevelThreePool.Count - 1].shipObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        //add/remove level Two ships
        if(levelTwoDifference > 0){
            for(int i = 0; i < levelTwoDifference; i++){
                shipsLevelTwo.Add(shipsLevelTwoPool[shipsLevelTwoPool.Count - 1]);
                shipsLevelTwoPool.RemoveAt(shipsLevelTwoPool.Count - 1);

                //set ship to active
                shipsLevelTwo[shipsLevelTwo.Count - 1].active = true;
                //turn on sprite renderer
                shipsLevelTwo[shipsLevelTwo.Count - 1].shipObject.GetComponent<SpriteRenderer>().enabled = true;
                //turn on for child aswell
                shipsLevelTwo[shipsLevelTwo.Count - 1].shipObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                shipsLevelTwo[shipsLevelTwo.Count - 1].shipObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;

            }
        }else if(levelTwoDifference < 0){
            for(int i = 0; i < Mathf.Abs(levelTwoDifference); i++){
                shipsLevelTwoPool.Add(shipsLevelTwo[shipsLevelTwo.Count - 1]);
                shipsLevelTwo.RemoveAt(shipsLevelTwo.Count - 1);

                //set ship to inactive
                shipsLevelTwoPool[shipsLevelTwoPool.Count - 1].active = false;
                //turn off sprite renderer
                shipsLevelTwoPool[shipsLevelTwoPool.Count - 1].shipObject.GetComponent<SpriteRenderer>().enabled = false;
                //turn off for child aswell
                shipsLevelTwoPool[shipsLevelTwoPool.Count - 1].shipObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                shipsLevelTwoPool[shipsLevelTwoPool.Count - 1].shipObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
            }
        }


        //add/remove level One ships
        if(levelOneDifference > 0){
            for(int i = 0; i < levelOneDifference; i++){
                shipsLevelOne.Add(shipsLevelOnePool[shipsLevelOnePool.Count - 1]);
                shipsLevelOnePool.RemoveAt(shipsLevelOnePool.Count - 1);

                //set ship to active
                shipsLevelOne[shipsLevelOne.Count - 1].active = true;
                //turn on sprite renderer
                shipsLevelOne[shipsLevelOne.Count - 1].shipObject.GetComponent<SpriteRenderer>().enabled = true;
                //turn on for child aswell
                shipsLevelOne[shipsLevelOne.Count - 1].shipObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                shipsLevelOne[shipsLevelOne.Count - 1].shipObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
            }
        }else if(levelOneDifference < 0){
            for(int i = 0; i < Mathf.Abs(levelOneDifference); i++){
                shipsLevelOnePool.Add(shipsLevelOne[shipsLevelOne.Count - 1]);
                shipsLevelOne.RemoveAt(shipsLevelOne.Count - 1);

                //set ship to inactive
                shipsLevelOnePool[shipsLevelOnePool.Count - 1].active = false;
                //turn off sprite renderer
                shipsLevelOnePool[shipsLevelOnePool.Count - 1].shipObject.GetComponent<SpriteRenderer>().enabled = false;
                //turn off for child aswell
                shipsLevelOnePool[shipsLevelOnePool.Count - 1].shipObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                shipsLevelOnePool[shipsLevelOnePool.Count - 1].shipObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //SetupPooledObjects();
    }

    
    // Update is called once per frame
    void Update()
    {
        updateShipPositions();

    }


}
