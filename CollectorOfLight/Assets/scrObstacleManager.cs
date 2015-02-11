using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scrObstacleManager : MonoBehaviour
{
	const float OBSTACLE_SPACING = 10.0f;	// Space between possible obstacle positions.

	private const int LIGHT_LENGTH_MIN = 5;
	private const int LIGHT_LENGTH_MAX = 15;
	private float lightSpawnTimer = 0;
	private float lightSpawnDelay = 10;
	private scrPool lightPool;

	private Dictionary<Vector2, GameObject> occupied = new Dictionary<Vector2, GameObject>();	// All occupied positions in world space.

	void Start ()
	{
		// Get the pool components.
		scrPool[] pools = GetComponents<scrPool>();
		foreach (scrPool pool in pools)
		{
			if (pool.Name == "Light")
				lightPool = pool;
		}
	}

	void Update ()
	{
		lightSpawnTimer += Time.deltaTime;
		if (lightSpawnTimer >= lightSpawnDelay)
		{
			lightSpawnTimer = 0;

			Vector2 position;
			if (RetrieveFreePosition(out position))
			{
				CreateLightSeries(position.x, position.y, Random.Range (LIGHT_LENGTH_MIN, LIGHT_LENGTH_MAX + 1), 5);
			}
		}
	}

	
	bool RetrieveFreePosition(out Vector2 outPosition)
	{
		float left = scrLandscape.Instance.GetLeft();
		float right = scrLandscape.Instance.GetRight();
		float front = scrLandscape.Instance.GetFront();
		float back = scrLandscape.Instance.GetBack();

		// Get all possible points.
		Vector2 position;
		List<Vector2> free = new List<Vector2>();

		for (float x = left; x < right; x += OBSTACLE_SPACING)
		{
			for (float z = back; z < front; z += OBSTACLE_SPACING)
			{
				position = new Vector2(x, z);

				// If the point is in the dictionary, do not add it to the free list.
				if (!occupied.ContainsKey(position))
					free.Add(position);
			}
		}

		if (free.Count == 0)
		{
			outPosition = Vector2.zero;
			return false;
		}
		else
		{
			// Output a random free point.
			outPosition = free[Random.Range (0, free.Count)];
			return true;
		}
	}

	void CreateLightSeries(float x, float z, int length, float spacing)
	{
		//float angle = Random.Range (0, 360);
		//Vector2 direction = new Vector2(Mathf.Sin (angle), Mathf.Cos (angle)); // Initial direction through which the orbs will be generated.
		Vector2 direction = new Vector2(x, z) - scrLandscape.Instance.GetCentre();
		direction.Normalize();

		float bend = Random.Range (-0.2f, 0.2f); // Amount the direction bends per orb.

		// Make sure it is possible to produce the length of string required.
		if (length > lightPool.Available)
		{
			// There aren't enough orbs available for a string, so cease creation.
			if (lightPool.Available < LIGHT_LENGTH_MIN)
				return;

			length = lightPool.Available;
		}

		Vector2 position = new Vector2(x, z);
		scrLightOrb previous = null;
		for (int i = 0; i < length; ++i)
		{
			// Create an orb.
			scrLightOrb current = (scrLightOrb)lightPool.Create(position.x, position.y);

			// Link to the previous orb.
			current.Previous = previous;
			if (previous != null)
				previous.Next = current;

			// Prepare for the next orb.
			previous = current;

			// Move to the next position.
			position += direction * spacing;

			// Bend in the direction perpendicular to the current direction.
			Vector2 perp = new Vector2(-direction.y, direction.x);
			direction += perp * bend;
			direction.Normalize();
		}
	}
}
