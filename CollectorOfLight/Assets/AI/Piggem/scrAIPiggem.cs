using UnityEngine;
using System.Collections;

public class scrAIPiggem : MobileObstacleMoveScript
{
	float targetAngle;

	void Start ()
	{
		ragdollDuration = 10.0f;
		Init ();
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
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetAngle, 0), 10 * Time.deltaTime);
		
	}

	public override void Init (params object[] initParams)
	{
		// Randomise the colour.
		//model.renderer.material = Random.Range (0, 2) == 0 ? MatBabyBlue : MatPowderPink;

		ChangeDirection();
		targetAngle = transform.eulerAngles.y;
	}
}
