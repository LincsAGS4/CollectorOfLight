using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scrObstacleManager : MonoBehaviour
{
	const float OBSTACLE_SPACING = 10.0f;	// Space between possible obstacle positions.

	private const int LIGHT_LENGTH_MIN = 5;
	private const int LIGHT_LENGTH_MAX = 15;
	private float distanceToNextLight = 0;
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

		DecreaseDistances(transform.forward);

	}

	public void DecreaseDistances(Vector3 velocity)
	{
		// Decrease light distance.
		distanceToNextLight -= velocity.magnitude * Time.deltaTime;
		if (distanceToNextLight <= 0)
		{
			distanceToNextLight = 0.05f; // number is debug, maybe random between min and max values.

			// Create a series of light-orbs somewhere in the distance in the direction the player is moving.
			Vector2 lightPosition;
			if (RetrieveFreePositionInDirection(new Vector2(velocity.x, velocity.z), scrLandscape.HALF_GRID_SCALE / 2, out lightPosition))
		    {
				CreateLightSeries(lightPosition.x, lightPosition.y, Random.Range (LIGHT_LENGTH_MIN, LIGHT_LENGTH_MAX + 1), 5);
			}
		}

	}

	bool RetrieveFreePositionInDirection(Vector2 direction, float minDistance, out Vector2 outPosition)
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

				if (Vector2.Distance(position, scrLandscape.Instance.GetCentre()) >= minDistance &&	// Check that the position is further than the minimum distance.
					Vector2.Angle (position - scrLandscape.Instance.GetCentre(), direction) < 45 &&	// Check that the position is within a 45 degree arc from the direction.
				    !occupied.ContainsKey(position)) 												// Check that the position is not already occupied.
				{
					free.Add(position);
				}
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
		Vector2 direction = new Vector2(x, z) - scrLandscape.Instance.GetCentre();
		direction.Normalize();

		float bend = Random.Range (-0.2f, 0.2f); // Amount the direction bends per orb.

		// Make sure it is possible to produce the length of string required.
		if (length > lightPool.Remaining)
		{
			// There aren't enough orbs available for a string, so cease creation.
			if (lightPool.Remaining < LIGHT_LENGTH_MIN)
				return;

			length = lightPool.Remaining;
		}

		Vector2 position = new Vector2(x, z);
		scrLightOrb previous = null;
		for (int i = 0; i < length; ++i)
		{
			// Create an orb.
			scrLightOrb current = (scrLightOrb)lightPool.Create(position.x, position.y, i, length);

			// Link the orb to the previous orb.
			current.SetInitialLink(previous);

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
