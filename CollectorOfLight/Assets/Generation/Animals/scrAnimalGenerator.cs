 using UnityEngine;
using System.Collections;

public class scrAnimalGenerator : scrGenerator
{
	public static scrAnimalGenerator Instance { get; private set; } 

	private const float DISTANCE_MAX = 60.0f;
	private const float DISTANCE_MIN = 20.0f;
	private const float DISTANCE_REDUCTION_PER_LIGHT = 1.0f;

	private void Start()
	{
		Instance = this;

		distanceRequired = DISTANCE_MAX;
		distanceOffset = 10.0f;
	}

	// ---- scrGenerator ----

	protected override void Generate ()
	{
		if (PlayerController.Instance.LightScore == 0) return;	// Do not generate if player has no light.

		// Check for a free position.
		Vector2 position;
		if (GetFreePosition(out position, maxHeight: scrLandscape.Instance.HighestPoint.y - (scrLandscape.Instance.HighestPoint.y - scrLandscape.Instance.LowestPoint.y) * 0.2f))
		{			
			// Choose an animal to generate.
			string animal = "";
			switch (Random.Range (0, 5))
			{
			case 0:
				animal = "Ellek";
				break;
			case 1:
				animal = "Piggem";
				break;
			case 2:
				animal = "Quirrel";
				break;
			case 3:
				animal = "Snool";
				break;
			case 4:
				animal = "Tribbet";
				break;
			}

			for (int i = 0, herd = Random.Range (1, 8); i < herd; ++i)
			{
				float angle = (float)i / herd * Mathf.PI * 2;
				pools[animal].Create (position.x + Random.Range (4, 10) * Mathf.Sin (angle), position.y + Random.Range (4, 10) * Mathf.Cos (angle));
			}
		}

		distanceRequired = Mathf.Max(DISTANCE_MAX - DISTANCE_REDUCTION_PER_LIGHT * PlayerController.Instance.LightScore, DISTANCE_MIN);		// Reduce the distance required by the amount of light the player has.
	}
}
