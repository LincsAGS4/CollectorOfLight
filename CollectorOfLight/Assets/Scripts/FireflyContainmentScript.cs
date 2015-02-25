using UnityEngine;
using System.Collections;

public class FireflyContainmentScript : MonoBehaviour 
{
    /*This script contains the fireflies within a limited area by setting their direction
     *towards the original emitter of particles when they exceed a certain distance from said 
     *emitter.*/

    public ParticleSystem particleSyst;
    public float maxDistance;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSyst.maxParticles];
        particleSyst.GetParticles(particles);

        //for each particle in system...
        for (int i = 0; i < particleSyst.maxParticles; i++)
        {
            //find vector between particle and system
            Vector3 displacementVector = particleSyst.transform.position - particles[i].position;

            //if dist between particle and system > maxDistance
            if (displacementVector.magnitude > maxDistance)
            {
                //set particle velocity = that vector
                particles[i].velocity = displacementVector;
                
                //OPTIONAL - slightly change vector so that fireflies don't bounce back and forth along the same 
            }
        }

        particleSyst.SetParticles(particles, particleSyst.maxParticles);
	}
}
