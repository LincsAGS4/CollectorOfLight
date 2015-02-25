using UnityEngine;
using System.Collections;

public class scrRock : scrPoolable
{
	public float scaleMin = 1.0f;
	public float scaleMax = 10.0f;
	bool physicsActive = false;

	protected override void Update ()
	{
		if (!physicsActive)
		{
			physicsActive = scrLandscape.Instance.PhysContains(transform.position);
			if (physicsActive)
				scrLandscape.Instance.GetNormal(transform.position.x, transform.position.z);
			else
				scrLandscape.Instance.GetNormalFromNoise(transform.position.x, transform.position.z, 1.0f);
		}

		base.Update ();
	}

	// ---- scrPoolable ----
	
	/// <param name="initParams">float x, float z</param>
	public override void Init(params object[] initParams)
	{
		Expired = false;
		physicsActive = false;
		
		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

		// Orient to the ground.
		transform.up = scrLandscape.Instance.GetNormal(x, z);

		// Randomise yaw.
		transform.Rotate(Vector3.up, Random.Range (0, 360), Space.Self);

		// Randomise scale.
		float scale = Random.Range (scaleMin, scaleMax);
		transform.localScale = new Vector3(scale, scale, scale);
	}
}
