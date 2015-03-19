using UnityEngine;
using System.Collections;

public class scrGemGenerator : scrGenerator
{
	private scrPool pool;

	private void Start()
	{
		distanceRequired = 200;
		distanceOffset = 100;

		pool = pools["Gems"];
	}

	// ---- scrGenerator ----

	protected override void Generate ()
	{
		// Find a random position.
		Vector2 position;
		if (GetFreePosition(out position, clearDistance: 2))
			pool.Create(position.x, position.y);
	}
}
