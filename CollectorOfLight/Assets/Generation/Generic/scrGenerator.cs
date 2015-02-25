using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(scrPool))]
public abstract class scrGenerator : MonoBehaviour
{
	#region Static
	public const float SPACING = 100.0f;	// Space between possible generation positions.
	protected static HashSet<Vector2> occupied = new HashSet<Vector2>();	// All occupied positions in world space.

	public static bool GetFreePosition(out Vector2 outPosition, float minHeight = float.MinValue, float maxHeight = float.MaxValue, float clearDistance = SPACING * 0.5f)
	{
		float left = scrLandscape.Instance.GetLeft();
		float right = scrLandscape.Instance.GetRight();
		float front = scrLandscape.Instance.GetFront();
		float back = scrLandscape.Instance.GetBack();
		
		// Get all possible points.
		Vector2 position;
		List<Vector2> free = new List<Vector2>();
		
		for (float x = left; x < right; x += SPACING)
		{
			for (float z = back; z < front; z += SPACING)
			{
				position = new Vector2(x, z);

				float height = scrLandscape.Instance.GetHeight(x, z);
				if (height >= minHeight && height <= maxHeight && // Check that the position gives a height within the allowed range.
					Vector2.Distance(position, scrLandscape.Instance.GetCentre()) >= scrLandscape.HALF_GRID_SCALE * 0.8f &&	// Check that the position is further than the minimum distance.
				    Vector2.Angle (position - scrLandscape.Instance.GetCentre(), scrPlayer.Instance.Velocity2D) < 45 &&	// Check that the position is within a 45 degree arc from the direction.
				    !Physics.CheckSphere(new Vector3(x, height, z), clearDistance))	// Check that the position isn't already taken by something (other than the ground).
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
	#endregion

	public bool Generating;

	protected float distanceRequired;	// Distance required for the player to move for something to spawn.
	protected float distanceOffset = 0;	// Boundary value to randomly increase/decrease the remaining distance by.
	protected float distanceRemaining = 0;	// Distance the player has left to move.
	protected Dictionary<string, scrPool> pools = new Dictionary<string, scrPool>();

	protected virtual void Awake()
	{
		scrPool[] ps = GetComponents<scrPool>();
		foreach (scrPool p in ps)
		{
			pools.Add (p.Identifier, p);
		}
	}

	protected virtual void Update()
	{
		if (Generating)
		{
			distanceRemaining -= scrPlayer.Instance.Speed2D * Time.deltaTime;

			if (distanceRemaining <= 0)
			{
				Generate();
				distanceRemaining = distanceRequired + Random.Range (-distanceOffset, distanceOffset);
			}
		}
	}

	protected abstract void Generate();
}