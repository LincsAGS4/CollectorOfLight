using UnityEngine;
using System.Collections;

public class scrPowerupGenerator : scrGenerator
{
	private const float DISTANCE_MAX = 20.0f;
	private const float DISTANCE_MIN = 5.0f;
	private const float DISTANCE_REDUCTION_PER_LIGHT = 1.0f;

	private scrPool pool;

	private void Start()
	{
		distanceRequired = DISTANCE_MAX;
		distanceOffset = 2.0f;

		pool = pools["Powerups"];
	}

	// ---- scrGenerator ----

	protected override void Generate ()
	{
		if (PlayerController.Instance.LightScore == 0) return;	// Do not generate if player has no light.

		// Find a random plant cluster.
		scrPlantCluster plantCluster;
		if (scrObstacleGenerator.Instance.GetPlantClusterInFrontOfPlayer(out plantCluster))
		{			
			// Choose an powerup to generate.
			scrPowerup.Powerup effect = (scrPowerup.Powerup)Random.Range (0, 4);

			// Generate the powerup at the centre of the plant cluster. No plants should be here, since they have a minimum radius away from the centre.
			scrPoolable powerup = pool.Create(plantCluster.transform.position.x, plantCluster.transform.position.z, effect);
		}

		distanceRequired = Mathf.Max(DISTANCE_MAX - DISTANCE_REDUCTION_PER_LIGHT * PlayerController.Instance.LightScore, DISTANCE_MIN);		// Reduce the distance required by the amount of light the player has.
	}
}
