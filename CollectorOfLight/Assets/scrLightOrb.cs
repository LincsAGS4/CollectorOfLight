using UnityEngine;
using System.Collections;

public class scrLightOrb : scrPoolable
{
	const float LEVITATE_HEIGHT = 4.0f;	// Height above the ground the light bobs up and down.
	const float BOB_HEIGHT = 1.0f;	// Maximum height of a bob.
	const float BOB_RATE = 0.5f;	// Rate at which the light bobs up and down.

	public scrLightOrb Previous { get; set; }
	public scrLightOrb Next { get; set; }

	private Vector3 spawnPosition;
	private LineRenderer line;

	void Awake ()
	{
		line = GetComponent<LineRenderer>();
	}

	void Update ()
	{
		// Bob up and down.
		transform.position = new Vector3(spawnPosition.x, spawnPosition.y + LEVITATE_HEIGHT + BOB_HEIGHT * Mathf.Sin (spawnPosition.x + spawnPosition.y + spawnPosition.z + Time.time * BOB_RATE), spawnPosition.z);
	
		// Draw a line to the next orb.
		if (Next != null)
		{
			line.SetPosition(0, transform.position);
			line.SetPosition(1, Next.transform.position);
		}
	
	}

	void OnTriggerEnter(Collider other)
	{
		// Break the line.
		if (Next != null)
			Next.Previous = null;

		if (Previous != null)
		{
			Previous.Next = null;

			// Clear vertices from previous node's line renderer so that it doesn't render anything.
			Previous.line.SetVertexCount(0);
		}

		line.SetVertexCount(0);

		gameObject.SetActive(false);
	}

	// ---- IPoolable ----

	/// <param name="initParams">float x, float z</param>
	public override void Init(params object[] initParams)
	{
		float x = (float)initParams[0];
		float z = (float)initParams[1];
		spawnPosition = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);
		transform.position = new Vector3(spawnPosition.x, spawnPosition.y + LEVITATE_HEIGHT + BOB_HEIGHT * Mathf.Sin (spawnPosition.x + spawnPosition.y + Time.time * BOB_RATE), spawnPosition.z);

		Next = null;
		Previous = null;

		line.SetVertexCount(2);
	}

}
