using UnityEngine;
using System.Collections;

public class scrAISnool : MobileObstacleMoveScript
{
	public Material MatBabyBlue;
	public Material MatPowderPink;	
	public Transform Shell;
	public Transform NotShell;

	private float hideTimer = 0.0f;
	private float hideDelay = 1.0f;

	void Start ()
	{
	}

	void OnCollisionEnter(Collision other)
	{
		if (hideTimer == 0)
			transform.Rotate (0, 180, 0);
	}
	
	protected override void SpecificMovement ()
	{

		if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 20.0f)
		{
			hideTimer += Time.deltaTime;
			if (hideTimer > hideDelay)
				hideTimer = hideDelay;
		}
		else
		{			
			hideTimer -= Time.deltaTime;
			if (hideTimer < 0)
				hideTimer = 0;
		}

		float t = hideTimer / hideDelay;
		currentSpeed = Mathf.Lerp (standardSpeed, 0, t);
		NotShell.localScale = Vector3.one * (1 - t);

		
	}

	public override void Init (params object[] initParams)
	{
		Expired = false;

		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

		// Randomise the colour.
		NotShell.renderer.material = Random.Range (0, 2) == 0 ? MatBabyBlue : MatPowderPink;
	
		// Randomise the direction.
		transform.rotation = Quaternion.Euler(0, Random.Range (0, 360), 0);

		// Randomise the speed.
		standardSpeed = Random.Range (1.0f, 5.0f);
	}
}
