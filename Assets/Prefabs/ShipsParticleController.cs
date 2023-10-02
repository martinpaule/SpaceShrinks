using System.Collections.Generic;
using UnityEngine;


public class ShipsParticleController : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public int numberOfParticles = 10;
    public float sphericalRadius = 5f;
    public float movementSpeed = 0.1f;
    public float turningStrength = 0.1f;  // higher value for quicker turns, lower for gradual turns

    private List<ParticleSystem.Particle> particles;
    private List<Vector3> targetPositions;
    private List<float> t;


void Start()
{
    particles = new List<ParticleSystem.Particle>(numberOfParticles);
    targetPositions = new List<Vector3>(new Vector3[numberOfParticles]);
    t = new List<float>(new float[numberOfParticles]);


    for (int i = 0; i < numberOfParticles; i++)
    {
        Vector2 randomPosition = Random.insideUnitSphere * sphericalRadius;

        // Declare a new variable to store for the ith particle
        ParticleSystem.Particle newParticle = new ParticleSystem.Particle();

        newParticle.position = new Vector3(randomPosition.x, randomPosition.y, 0f);  // Set the position
        newParticle.startSize = 1f;  // Set the size or any other properties
        newParticle.remainingLifetime = Mathf.Infinity;

        // Add the particle to the list
        particles.Add(newParticle);

        // Assign initial target positions
        targetPositions.Add((Vector2)(Random.insideUnitSphere) * sphericalRadius);
        t.Add(0f);
    }
    particleSystem.SetParticles(particles.ToArray(), numberOfParticles);
}

    void Update()
    {
        for (int i = 0; i < numberOfParticles; i++)
        {
            // Interpolation factor
            t[i] += Time.deltaTime * movementSpeed * turningStrength;
            t[i] = Mathf.Clamp01(t[i]);

            // Spherically interpolate between current position and target position
            //particles[i].position = Vector3.Slerp(particles[i].position.normalized, targetPositions[i].normalized, t[i]) * sphericalRadius;

            //declare a new variable to store for the ith particle
            ParticleSystem.Particle editedParticle = particles[i];

            editedParticle.position = Vector3.Slerp(particles[i].position.normalized, targetPositions[i].normalized, t[i]) * sphericalRadius;
            editedParticle.position = Vector3.Slerp(particles[i].position, targetPositions[i], t[i]) * sphericalRadius;
            editedParticle.position = (Vector2)editedParticle.position;
            particles[i] = editedParticle;

            // If particle has reached or almost reached target position, assign a new target position
            if (t[i] >= 1f)
            {
                targetPositions[i] = (Vector2)(Random.insideUnitSphere) * sphericalRadius;
                t[i] = 0f;  // Reset interpolation factor
            }
        }
        particleSystem.SetParticles(particles.ToArray(), particles.Count);
    }

public void SetNewParticleAmount(int newAmount)
    {
        int currentAmount = particles.Count;
        if (newAmount > currentAmount)
        {
            // Add additional particles
            for (int i = currentAmount; i < newAmount; i++)
            {
                
                Vector2 randomPosition = Random.insideUnitCircle * sphericalRadius;
                ParticleSystem.Particle newParticle = new ParticleSystem.Particle
                {
                    position = randomPosition,
                    startSize = 1f,
                    remainingLifetime = Mathf.Infinity
                };
                particles.Add(newParticle);
                targetPositions.Add((Vector2)(Random.insideUnitSphere) * sphericalRadius);
                t.Add(0f);
            }
        }
        else if (newAmount < currentAmount)
        {
            // Remove excess particles
            particles.RemoveRange(newAmount, currentAmount - newAmount);
            targetPositions.RemoveRange(newAmount, currentAmount - newAmount);
            t.RemoveRange(newAmount, currentAmount - newAmount);
        }

        // Update the particle system
        particleSystem.SetParticles(particles.ToArray(), particles.Count);
        numberOfParticles = particles.Count;
    }
}

