using UnityEngine;
using System.Collections;

public sealed class scrLightGenerator : scrGenerator
{
	public static scrLightGenerator Instance { get; private set; }

	private const int LENGTH_MIN = 5;	// Minimum length of a line of light.
	private const int LENGTH_MAX = 15;	// Maximum length of a line of light.

	private scrPool pool;	// Only pool used.

	private void Start()
	{
		Instance = this;
		distanceRequired = 2.0f;
		distanceOffset = 1.0f;
		pool = pools["Light"];
	}
	
	public void GenerateLine(float x, float z, int length, float spacing)
	{
		Vector2 direction = new Vector2(x, z) - scrLandscape.Instance.GetCentre();
		direction.Normalize();
		
		float bend = Random.Range (-0.1f, 0.1f); // Amount the direction bends per orb.
		
		// Make sure it is possible to produce the length of string required.
		if (length > pool.Remaining)
		{
			// There aren't enough orbs available for a string, so cease creation.
			if (pool.Remaining < LENGTH_MIN)
				return;
			
			length = pool.Remaining;
		}
		
		Vector2 position = new Vector2(x, z);
		scrLightOrb previous = null;
		for (int i = 0; i < length; ++i)
		{
			// Create an orb.
			scrLightOrb current = (scrLightOrb)pool.Create(position.x, position.y, i, length);
			
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
	
	// ---- scrGenerator ----

	protected override void Generate ()
	{
		// Create a series of light-orbs somewhere in the distance in the direction the player is moving.
		Vector2 lightPosition;
		if (GetFreePosition(out lightPosition))
		{
			GenerateLine(lightPosition.x, lightPosition.y, Random.Range (LENGTH_MIN, LENGTH_MAX + 1), 5);
		}
	}

}
