using UnityEngine;
using System.Collections;

public class scrObstacleGenerator : scrGenerator
{
	public static scrObstacleGenerator Instance { get; private set; } 

	private const float DISTANCE_MAX = 20.0f;
	private const float DISTANCE_MIN = 2.0f;
	private const float DISTANCE_REDUCTION_PER_LIGHT = 1.0f;

	private const int GENERATIONS_MIN = 1;
	private const int GENERATIONS_MAX = 5;
	private const float GENERATION_INCREASE_PER_LIGHT = 0.1f;	// Since generations are int, values < 1 increment generations when >1 light is collected.

	private const int GENERATIONS_UNTIL_LARGE_TREE_MIN = 5;
	private const int GENERATIONS_UNTIL_LARGE_TREE_MAX = 10;
	private int generationsUntilLargeTree = GENERATIONS_UNTIL_LARGE_TREE_MAX;

	private const int GENERATIONS_UNTIL_LOG_MIN = 2;
	private const int GENERATIONS_UNTIL_LOG_MAX = 6;
	private int generationsUntilLog = GENERATIONS_UNTIL_LOG_MAX;

	private void Start()
	{
		Instance = this;

		distanceRequired = DISTANCE_MAX;
		distanceOffset = 10.0f;

		scrPlantCluster.InitStatic();
	}

	public bool GetPlantClusterInFrontOfPlayer(out scrPlantCluster plantCluster)
	{
		foreach (scrPoolable p in pools["Plant Clusters"].GetAllActive())
		{
			Vector2 position = new Vector2(p.transform.position.x, p.transform.position.z);
			if(Vector2.Distance(position, scrLandscape.Instance.GetCentre()) >= scrLandscape.PHYS_GRID_SCALE &&	// Check that the position is further than a minimum distance.
			   Vector2.Angle (position - scrLandscape.Instance.GetCentre(), PlayerController.Instance.Velocity2D) < 45 &&  // Check that the position is within a 45 degree arc from the direction.
			   !Physics.CheckSphere(p.transform.position, SPACING, 1 << LayerMask.NameToLayer("Powerup")))	// Check that the position is not occupied.
			{
				plantCluster = (scrPlantCluster)p;
				return true;
			}
		}
		plantCluster = null;
		return false;
	}

	// ---- scrGenerator ----

	protected override void Generate ()
	{
		//if (PlayerController.Instance.LightScore == 0) return;	// Do not generate if player has no light.

		// Check for a free position.
		Vector2 position;
		for (int i = 0; i < Mathf.Min (GENERATIONS_MIN + GENERATION_INCREASE_PER_LIGHT, GENERATIONS_MAX); ++i) 
		{
			--generationsUntilLargeTree;
			--generationsUntilLog;

			if (generationsUntilLargeTree <= 0)
			{
				if (GetFreePosition(out position, scrLandscape.Instance.HighestPoint.y - (scrLandscape.Instance.HighestPoint.y - scrLandscape.Instance.LowestPoint.y) * 0.3f, scrLandscape.Instance.MaxHeight, 20))
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
				switch (Random.Range (0, 4))
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
                case 3:
                    obstacle = pools["Ramps"].Create(position.x, position.y);
                    break;
				}
			}
		}

		distanceRequired = Mathf.Max(DISTANCE_MAX - DISTANCE_REDUCTION_PER_LIGHT * PlayerController.Instance.LightScore, DISTANCE_MIN);		// Reduce the distance required by the amount of light the player has.
	}
}
