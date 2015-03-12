using UnityEngine;
using System.Collections;

public class scrLog : scrPoolable
{
	public float scaleMin = 1.0f;
	public float scaleMax = 2.0f;
	
	// ---- scrPoolable ----
	
	/// <param name="initParams">float x, float z</param>
	public override void Init(params object[] initParams)
	{
		Expired = false;
		
		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);
		
		// Orient to the ground.
		transform.up = scrLandscape.Instance.GetNormal(x, z);

		// Push away from the ground by the y scale so that the log is sitting above it rather than half inside it.
		transform.position += transform.up * transform.localScale.y;
		
		// Randomise yaw.
		transform.Rotate(Vector3.up, Random.Range (0, 360), Space.Self);
		
		// Randomise scale.
		float scale = Random.Range (scaleMin, scaleMax);
		transform.localScale = new Vector3(scale, scale, scale);
	}

}
