using UnityEngine;
using System.Collections;

public class scrLightOrb : scrPoolable
{
	const float LEVITATE_HEIGHT = 0.4f;	// Height above the ground the light bobs up and down.
	const float BOB_HEIGHT = 0.2f;	// Maximum height of a bob.
	const float BOB_RATE = 0.5f;	// Rate at which the light bobs up and down.

	private scrLightOrb previous, next;
	private int seriesIndex;	// The index of this orb in its series. Since the indexes will remain the same after being initially set, it will be easy to tell the state of the series through recursion if necessary.
	private int seriesLength;	// The length of the series.

	public Vector3 SpawnPosition { get; private set; }
	public Vector3 AnchorPosition;
	private LineRenderer line;

	void Awake ()
	{
		line = GetComponent<LineRenderer>();
	}

	protected override void Update ()
	{
		// Bob up and down.
		transform.position = new Vector3(AnchorPosition.x, AnchorPosition.y + LEVITATE_HEIGHT + BOB_HEIGHT * Mathf.Sin (AnchorPosition.x + AnchorPosition.y + AnchorPosition.z + Time.time * BOB_RATE), AnchorPosition.z);
	
		// Draw a line to the next orb as long as the next orb is the next in the series, otherwise the series has been broken at some point.
		if (next != null && next.seriesIndex == seriesIndex + 1)
		{
			line.SetPosition(0, transform.position);
			line.SetPosition(1, next.transform.position);
		}

		base.Update();
	}

	void OnTriggerEnter(Collider other)
	{
		// Remove this orb from the series by connecting the next and previous orbs.
		if (next != null)
			next.previous = previous;

		if (previous != null)
		{
			previous.next = next;

			// Clear vertices from previous node's line renderer so that it doesn't render anything.
			previous.line.SetVertexCount(0);
		}

		// Deactivate.
		Expired = true;
	}
	

	public void SetInitialLink(scrLightOrb link)
	{
		if (link != null)
		{
			// Link the previous orb to this orb.
			previous = link;
			link.next = this;
			link.line.SetVertexCount(2);
		}
	}

	// ---- scrPoolable ----

	/// <param name="initParams">float x, float z, int seriesIndex, int seriesLength</param>
	public override void Init(params object[] initParams)
	{
		Expired = false;

		transform.localScale = Vector3.one;

		float x = (float)initParams[0];
		float z = (float)initParams[1];
		SpawnPosition = AnchorPosition = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);
		transform.position = new Vector3(AnchorPosition.x, AnchorPosition.y + LEVITATE_HEIGHT + BOB_HEIGHT * Mathf.Sin (AnchorPosition.x + AnchorPosition.y + Time.time * BOB_RATE), AnchorPosition.z);

		seriesIndex = (int)initParams[2];
		seriesLength = (int)initParams[3];

		next = null;
		previous = null;

		line.SetVertexCount(0);
	}

	protected override void ExpireWhenOutOfBounds()
	{
		// If this orb is not out of bounds, the whole series is not out of bounds.
		if (scrLandscape.Instance.Contains(transform.position))
			return;
		
		// Move backwards until the first in the series is found.
		scrLightOrb first = this;
		while (first.previous != null)
			first = first.previous;
		
		// Move forwards until the end of the series.
		scrLightOrb orb = first;
		while (orb != null)
		{
			//  The series is not out of bounds if just one of them is in bounds. Also skip the check for this orb as has already been checked.
			if (orb != this && scrLandscape.Instance.Contains(orb.transform.position))
				return;
			
			orb = orb.next;
		}
		
		// The function didn't return early, so the series is fully out of bounds. Deactivate all orbs in the series from the first to the last.
		while (first != null)
		{
			first.Expired = true;		
			first = first.next;
		}
	}

}
