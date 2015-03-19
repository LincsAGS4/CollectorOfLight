using UnityEngine;
using System.Collections;

public class scrFireflies : MonoBehaviour
{
	const float CHANGE_DELAY_MAX = 1.5f;
	const float CHANGE_DELAY_MIN = 0.5f;
	float changeDelay = CHANGE_DELAY_MAX;
	float changeTimer = 0.0f;

	const float SPEED_MAX = 6.0f;
	const float SPEED_MIN = 1.0f;

	public float Radius = 0.5f;

	void Update () 
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(particles);

		changeTimer += Time.deltaTime;
		if (changeTimer >= changeDelay)
		{
			changeTimer = 0.0f;
			changeDelay = Random.Range (CHANGE_DELAY_MIN, CHANGE_DELAY_MAX);

			int amountToChange = Random.Range (5, 20);
			int startIndex = Random.Range (0, particles.Length - amountToChange - 1);
			for (int i = 0; i < amountToChange; ++i)
			{
				particles[startIndex + i].velocity = Random.rotation.eulerAngles.normalized * Random.Range (SPEED_MIN, SPEED_MAX);
			}
		}

		for (int i = 0; i < particles.Length; ++i)
		{
			if (Vector3.Distance(particles[i].position, transform.position) > Radius)
				particles[i].velocity = ((transform.position + new Vector3(Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f)).normalized * 0.5f) -
				                         particles[i].position).normalized * Random.Range (SPEED_MIN, SPEED_MAX);
		}
		
		particleSystem.SetParticles(particles, particles.Length);
	}
}
