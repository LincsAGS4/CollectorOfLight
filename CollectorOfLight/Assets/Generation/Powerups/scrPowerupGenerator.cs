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

		// Find a random obstacle position.
		Vector2 position;
		if (GetFreePosition(out position))
		{			
			// Choose an powerup to generate.
			scrPowerup.Powerup effect = (scrPowerup.Powerup)Random.Range (0, 4);

			// Generate the powerup.
			scrPoolable powerup = pool.Create(position.x, position.y, effect);
		}

		distanceRequired = Mathf.Max(DISTANCE_MAX - DISTANCE_REDUCTION_PER_LIGHT * PlayerController.Instance.LightScore, DISTANCE_MIN);		// Reduce the distance required by the amount of light the player has.
	}
}
