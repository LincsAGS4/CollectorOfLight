using UnityEngine;
using System.Collections;

public class scrObstacleGenerator : scrGenerator
{
	private const float DISTANCE_MAX = 2.0f;
	private const float DISTANCE_MIN = 0.1f;
	private const float DISTANCE_REDUCTION_PER_LIGHT = 1.0f;

	private const int GENERATIONS_MIN = 5;
	private const int GENERATIONS_MAX = 10;
	private const float GENERATION_INCREASE_PER_LIGHT = 0.1f;	// Since generations are int, values < 1 increment generations when >1 light is collected.

	private const int GENERATIONS_UNTIL_LARGE_TREE_MIN = 5;
	private const int GENERATIONS_UNTIL_LARGE_TREE_MAX = 10;
	private int generationsUntilLargeTree = GENERATIONS_UNTIL_LARGE_TREE_MAX;

	private const int GENERATIONS_UNTIL_LOG_MIN = 2;
	private const int GENERATIONS_UNTIL_LOG_MAX = 6;
	private int generationsUntilLog = GENERATIONS_UNTIL_LOG_MAX;

	private void Start()
	{
		distanceRequired = DISTANCE_MAX;
		distanceOffset = 10.0f;

		scrPlantCluster.InitStatic();
	}

	// ---- scrGenerator ----

	protected override void Generate ()
	{
		if (PlayerController.Instance.LightScore == 0) return;	// Do not generate if player has no light.

		// Check for a free position.
		Vector2 position;
		for (int i = 0; i < Mathf.Min (GENERATIONS_MIN + GENERATION_INCREASE_PER_LIGHT, GENERATIONS_MAX); ++i) 
		{
			--generationsUntilLargeTree;
			--generationsUntilLog;

			if (generationsUntilLargeTree <= 0)
			{
				if (GetFreePosition(out position, scrLandscape.Instance.HighestPoint.y - (scrLandscape.Instance.HighestPoint.y - scrLandscape.Instance.LowestPoint.y) * 0.2f, scrLandscape.Instance.MaxHeight, 200))
				{
					pools["Large Trees"].Create (position.x, position.y);
					generationsUntilLargeTree = Random.Range (GENERATIONS_UNTIL_LARGE_TREE_MIN, GENERATIONS_UNTIL_LARGE_TREE_MAX + 1);
					continue;
				}
			}

			if (generationsUntilLog <= 0)
			{
				// Create logs from the bottom of the hill to half way up.
				if (GetFreePosition(out position, (scrLandscape.Instance.HighestPoint.y - scrLandscape.Instance.LowestPoint.y) * 0.6f))
				{
					pools["Logs"].Create (position.x, position.y);
					generationsUntilLog = Random.Range (GENERATIONS_UNTIL_LOG_MIN, GENERATIONS_UNTIL_LOG_MAX + 1);
					continue;
				}
			}

			// Anything else.
			if (GetFreePosition(out position, maxHeight: scrLandscape.Instance.HighestPoint.y - (scrLandscape.Instance.HighestPoint.y - scrLandscape.Instance.LowestPoint.y) * 0.2f))
			{			
				// Choose an obstacle to generate.
				scrPoolable obstacle = null;
				switch (Random.Range (0, 3))
				{
				case 0:
					obstacle = pools["Plant Clusters"].Create (position.x, position.y, 16);
					break;
				case 1:
					obstacle = pools["Trees and Shrubs"].Create (position.x, position.y);
					break;
				case 2:
					obstacle = pools["Pillars and Rocks"].Create(position.x, position.y);
					break;
				}
			}
		}

		distanceRequired = Mathf.Max(DISTANCE_MAX - DISTANCE_REDUCTION_PER_LIGHT * PlayerController.Instance.LightScore, DISTANCE_MIN);		// Reduce the distance required by the amount of light the player has.
	}
}
