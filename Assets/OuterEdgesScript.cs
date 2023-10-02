using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(EdgeCollider2D))]
public class OuterEdgesScript : MonoBehaviour
{
    public float outerRadius = 1f;
    public float innerRadius = 0.5f;
    public int segments = 64;

    public float shrinkSpeed = 10.0f;

    public bool startedGame = false;

    bool shrinking = false;

    float shrinkingCounter = 40.0f;

    public GameObject shrinkingTimerUI_PF;

    GameObject shrinkingTimerUI;
    GameManagerScript gameManagerScript;

    public ParticleSystem PlanetExplodeParticle;

    public AudioClip ExplosionSound;

    public AudioClip ZoneDroneSound;

    public AudioClip ZoneWarningSound;

    bool playedWarningSound = false;


    public void initialSetup(Vector2 playerPos){
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        GameSetupScript gameSetupScript = GameObject.Find("GameManager").GetComponent<GameSetupScript>();
        this.transform.position = gameSetupScript.planetsCentre;

        float distanceOfPlayerFromCentre = Vector2.Distance(playerPos, gameSetupScript.planetsCentre);
        innerRadius = distanceOfPlayerFromCentre + 10.0f;
        outerRadius = distanceOfPlayerFromCentre + 40.0f;

        shrinkingTimerUI = Instantiate(shrinkingTimerUI_PF, GameObject.Find("Canvas").transform);

        startedGame = true;
        UpdateDonut();
    }

    void Start()
    {
        
    }

    void Update()
    {

        if(!startedGame){
            return;
        }

        if(!shrinking){
            shrinkingCounter -= Time.deltaTime;

            shrinkingTimerUI.GetComponent<TextMeshProUGUI>().text = "SpaceShrink in "+ shrinkingCounter.ToString("F2") + "s";

            if(shrinkingCounter <= 5.0f && !playedWarningSound){
                AudioSource.PlayClipAtPoint(ZoneWarningSound, Camera.main.transform.position);
                playedWarningSound = true;
            }

            if(shrinkingCounter <= 0.0f){
                shrinking = true;
                shrinkingCounter = 3.0f;
                shrinkingTimerUI.GetComponent<TextMeshProUGUI>().text = "!SPACE SHRINKING!";

                //turn on zonedrone sound from my audio source
                AudioSource myAudioSource = GetComponent<AudioSource>();
                myAudioSource.clip = ZoneDroneSound;
                myAudioSource.loop = true;
                myAudioSource.Play();
            }
        }else{

            shrinkingCounter -= Time.deltaTime;
            
            if(shrinkingCounter <= 0.0f){
                shrinking = false;
                playedWarningSound = false;
                shrinkingCounter = 40.0f;
                AudioSource myAudioSource = GetComponent<AudioSource>();
                myAudioSource.Stop();
            }

            if(innerRadius > 3.0f){
                innerRadius-= shrinkSpeed * Time.deltaTime;
                outerRadius-= shrinkSpeed * Time.deltaTime;
                UpdateDonut();
            }


            foreach(GameObject planet in gameManagerScript.AllPlanets){
                if(Vector2.Distance(planet.transform.position, this.transform.position) > innerRadius){
                    ParticleSystem particle = Instantiate(PlanetExplodeParticle, planet.transform.position, Quaternion.identity);
                    particle.transform.position += new Vector3(0, 0, -1);
                    particle.transform.rotation = Quaternion.Euler(90, 0, 0);
                    particle.Play();
                    AudioSource.PlayClipAtPoint(ExplosionSound, planet.transform.position);
                    GameObject planetRef = planet;
                    gameManagerScript.AllPlanets.Remove(planetRef);
                    Destroy(planetRef);
                    gameManagerScript.gameWinLoseCheck();
                }
            }


        }

        

        

    }

    public void UpdateDonut()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[segments * 6];
        Vector2[] edgePoints = new Vector2[segments + 1];  // +1 to close the loop

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            vertices[i * 2] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * outerRadius;
            vertices[i * 2 + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * innerRadius;

            triangles[i * 6] = i * 2;
            triangles[i * 6 + 1] = (i * 2 + 2) % (segments * 2);
            triangles[i * 6 + 2] = i * 2 + 1;
            triangles[i * 6 + 3] = i * 2 + 1;
            triangles[i * 6 + 4] = (i * 2 + 2) % (segments * 2);
            triangles[i * 6 + 5] = (i * 2 + 3) % (segments * 2);

            // Setting points for the edge collider
            edgePoints[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * outerRadius;
        }

        // Close the loop for the edge collider
        edgePoints[segments] = edgePoints[0];

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
