using UnityEngine;
using System.Collections;

public class scrGem : scrPoolable
{
	private static Color[] colours = new Color[] { Color.red, Color.magenta, Color.cyan, Color.green };
	const float LEVITATE_HEIGHT = 1.0f;
	const float BOB_HEIGHT = 0.3f;
	const float BOB_RATE = 1.0f;
	const float SPIN_RATE = 1.0f;

	private Vector3 spawnPosition;
	public Transform ChildBeam { get; private set; }

	void Start()
	{
		ChildBeam = transform.Find ("Beam");
	}

	protected override void Update ()
	{
		ChildBeam.transform.position = transform.position + new Vector3(0.0f, 72.0f, 0.0f);
		ChildBeam.transform.eulerAngles = new Vector3(0.0f, Camera.main.transform.eulerAngles.y, 0.0f);
		transform.eulerAngles = new Vector3(20.0f, SPIN_RATE * Time.time, 20.0f);
		base.Update ();
	}

	void OnTriggerEnter(Collider other)
	{		
		// Deactivate.
		Expired = true;
		collider.enabled = false;
	}

	// ---- scrPoolable ----
	
	/// <param name="initParams">float x, float z, int seriesIndex, int seriesLength</param>
	public override void Init(params object[] initParams)
	{
		Expired = false;
		collider.enabled = true;
		
		transform.localScale = Vector3.one;
		
		float x = (float)initParams[0];
		float z = (float)initParams[1];
		spawnPosition = transform.position = new Vector3(x, scrLandscape.Instance.GetHeightFromNoise(x, z), z);

		// Randomise the colour between the available colours.
		Color colour = Color.Lerp(colours[Random.Range (0, colours.Length)], colours[Random.Range (0, colours.Length)], Random.Range (0.25f, 0.75f));
		transform.Find ("Model").renderer.material.color = colour;
		particleSystem.startColor = colour + Color.white * 0.1f;
	}
}
