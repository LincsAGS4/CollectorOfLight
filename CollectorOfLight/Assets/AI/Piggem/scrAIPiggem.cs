using UnityEngine;
using System.Collections;

public class scrAIPiggem : MobileObstacleMoveScript
{
	public Material[] SkinMats;

	float turnDelay = 3.0f;
	float turnTimer = 0.0f;
	float targetAngle;

	void Start ()
	{
		ragdollDuration = 10.0f;
	}

	void ChangeDirection()
	{
		targetAngle = transform.eulerAngles.y + Random.Range (-180, 180);
		standardSpeed = Random.Range (3.0f, 8.0f);
		currentSpeed = standardSpeed;
	}

	void OnCollisionEnter(Collision other)
	{
		if (!RagdollActive)
		{
			ChangeDirection();
			Ragdollize();
		}
	}
	
	protected override void SpecificMovement ()
	{
		turnTimer += Time.deltaTime;
		if (turnTimer >= turnDelay)
		{
			turnTimer = 0.0f;
			
			// Set the angle to turn to.
			targetAngle = transform.eulerAngles.y + Random.Range (-180, 180);
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle, 0), 10 * Time.deltaTime);	
	}

	public override void Init (params object[] initParams)
	{
		Expired = false;

		// Randomise the colour.
		model.renderer.material = SkinMats[Random.Range (0, SkinMats.Length)];

		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

		ChangeDirection();
		targetAngle = transform.eulerAngles.y;
	}
}
