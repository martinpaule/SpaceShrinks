using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSetupScript : MonoBehaviour
{
    public GameObject prefab;  // Prefab to spawn
    public float radius = 3;  // Minimum distance between points
    public int count = 20;  // Desired number of points
    public float regionRadius = 20;  // Radius of the circle
    public Vector2 center = Vector2.zero;  // Center point of the circle

    public List<GameObject> planets = new List<GameObject>();

    public GameManagerScript gameManager;

    public Vector2 planetsCentre = new Vector2(0.0f,0.0f);

    public List<Color> basicColors = new List<Color>
    {
        Color.red,
        Color.green,    
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        new Color(1, 0.5f, 0),  // Orange
        new Color(0.5f, 0, 0.5f)  // Purple
    };

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

    }

    public Vector2 SpawnPlanetsAssignColours(float spaceBetweenPlanets,int numberOfPlanets,float gameAreaRadius, int numOfPlayers, Color PlayerColor){

        UImanager uim = GameObject.Find("Canvas").GetComponent<UImanager>();

        //disable and hide jump home button
        uim.jumpHomeButton.SetActive(true);

        Vector2 returnPos = Vector2.zero;
        int actualPlanetNum = 0;
        //generate planets
        Vector2[] samples = GenerateSamples(spaceBetweenPlanets, gameAreaRadius, numberOfPlanets);
        foreach (Vector2 sample in samples)
        {
            GameObject planetRef = Instantiate(prefab, new Vector3(sample.x, sample.y, 0), Quaternion.identity);
            planetRef.GetComponent<PlanetScript>().setupPlanet();
            planetRef.GetComponent<PlanetScript>().updateText();
            planetRef.GetComponent<PlanetScript>().gameManager = gameManager;
            planets.Add(planetRef);

            planetsCentre.x += sample.x;
            planetsCentre.y += sample.y;
            actualPlanetNum++;
        }

        planetsCentre.x /= actualPlanetNum;
        planetsCentre.y /= actualPlanetNum;

        //set owners
        List<GameObject> farthestPlanets = GetFarthestGameObjects(numOfPlayers);
        int i = 0;
        foreach (GameObject planet in farthestPlanets)
        {

            if(i == 0){
                planet.GetComponent<PlanetScript>().SetOwner(PlayerColor);
                basicColors.Remove(PlayerColor);
                returnPos = planet.transform.position;
            }else{
                Color chosenColor = basicColors[Random.Range(0, basicColors.Count)];
                planet.GetComponent<PlanetScript>().SetOwner(chosenColor);
                basicColors.Remove(chosenColor);
            }
            planet.GetComponent<PlanetScript>().resources = 0.0f;
            planet.GetComponent<PlanetScript>().productionSpeed = 1.0f;
            planet.GetComponent<PlanetScript>().transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            i++;
        }

        gameManager.AllPlanets = planets;

        OuterEdgesScript outerEdges = GameObject.Find("OuterEdgesManager").GetComponent<OuterEdgesScript>();
        outerEdges.initialSetup(returnPos);

        return returnPos;
    }


    public List<GameObject> GetFarthestGameObjects(int count)
    {
        if (count <= 0 || planets.Count == 0)
            return null;

        count = Mathf.Min(count, planets.Count);

        List<GameObject> selectedObjects = new List<GameObject>();
        float maxDistance = float.MinValue;
        GameObject first = null, second = null;

        // Find the pair of planets that are furthest apart
        foreach (var obj1 in planets)
        {
            foreach (var obj2 in planets)
            {
                if (obj1 != obj2)
                {
                    float distance = Vector3.Distance(obj1.transform.position, obj2.transform.position);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        first = obj1;
                        second = obj2;
                    }
                }
            }
        }

        if (first != null && second != null)
        {
            selectedObjects.Add(first);
            selectedObjects.Add(second);
        }

        // Iteratively find the next GameObject that is furthest from the already-selected planets
        for (int i = 2; i < count; i++)
        {
            float maxMinDistance = float.MinValue;
            GameObject nextObject = null;

            foreach (var obj in planets)
            {
                if (!selectedObjects.Contains(obj))
                {
                    float minDistance = float.MaxValue;
                    foreach (var selected in selectedObjects)
                    {
                        float distance = Vector3.Distance(obj.transform.position, selected.transform.position);
                        minDistance = Mathf.Min(minDistance, distance);
                    }

                    if (minDistance > maxMinDistance)
                    {
                        maxMinDistance = minDistance;
                        nextObject = obj;
                    }
                }
            }

            if (nextObject != null)
            {
                selectedObjects.Add(nextObject);
            }
        }

        return selectedObjects;
    }

    public Vector2[] GenerateSamples(float radius, float regionRadius, int count, int numSamplesBeforeRejection = 30)
    {
        float cellSize = radius / Mathf.Sqrt(2);  

        int gridSize = Mathf.CeilToInt(regionRadius * 2 / cellSize);
        int[,] grid = new int[gridSize, gridSize];
        List<Vector2> samples = new List<Vector2>();  
        List<Vector2> spawnPoints = new List<Vector2>();  

        spawnPoints.Add(center);  
        while (spawnPoints.Count > 0 && samples.Count < count)  
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);  
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;  
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));  
                Vector2 candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);  
                if (IsValid(candidate, regionRadius, cellSize, radius, samples, grid))  
                {
                    samples.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)((candidate.x + regionRadius) / cellSize), (int)((candidate.y + regionRadius) / cellSize)] = samples.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);  
            }
        }

        return samples.ToArray();
    }

    bool IsValid(Vector2 candidate, float regionRadius, float cellSize, float radius, List<Vector2> samples, int[,] grid)
    {
        if (Vector2.Distance(candidate, center) <= regionRadius)  
        {
            int cellX = (int)((candidate.x + regionRadius) / cellSize);
            int cellY = (int)((candidate.y + regionRadius) / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - samples[pointIndex]).sqrMagnitude;  
                        if (sqrDst < radius * radius)  
                        {
                            return false;
                        }
                    }
                }
            }
            return true;  
        }
        return false;
    }
}
