using UnityEngine;
using System.Collections;

public class scrAIQuirrel : MobileObstacleMoveScript
{
	const float FLEE_RADIUS_START = 10.0f;	// Will start fleeing when player enters this range.
	const float FLEE_RADIUS_END = 20.0f;	// Will stop fleeing when out of this range from the player.

	float turnDelay = 3.0f;
	float turnTimer = 0.0f;

	float pauseDelay = 3.0f;
	float pauseTimer = 0.0f;

	float targetAngle;

	enum State
	{
		RUNNING,
		PAUSING,
		FLEEING
	}
	State state = State.RUNNING;

	void Start ()
	{
		Init ();
	}

	protected override void SpecificMovement ()
	{
		if (state != State.FLEEING && Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= FLEE_RADIUS_START)
		{
			state = State.FLEEING;
		}

		switch (state)
		{
		case State.RUNNING:

			// Increase or decrease the speed to the standard speed.
			if (currentSpeed < standardSpeed)
			{
				currentSpeed += Time.deltaTime * standardSpeed;
				if (currentSpeed > standardSpeed)
					currentSpeed = standardSpeed;
			}
			else if (currentSpeed > standardSpeed)
			{
				currentSpeed -= Time.deltaTime * standardSpeed;
				if (currentSpeed < standardSpeed)
					currentSpeed = standardSpeed;
			}

			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler (0, targetAngle, 0), 40 * Time.deltaTime);

			turnTimer += Time.deltaTime;
			if (turnTimer >= turnDelay)
			{
				turnTimer = 0.0f;

				// Set the angle to turn to.
				targetAngle = transform.eulerAngles.y + Random.Range (-180, 180);
			}

			pauseTimer += Time.deltaTime;
			if (pauseTimer >= pauseDelay)
			{
				pauseTimer = 0.0f;

				state = State.PAUSING;
			}

			break;
		case State.PAUSING:

			// Decrease the speed to zero.
			currentSpeed -= Time.deltaTime * standardSpeed * 2;
			if (currentSpeed <= 0)
				currentSpeed = 0;

			pauseTimer += Time.deltaTime;
			if (pauseTimer >= pauseDelay)
			{
				pauseTimer = 0.0f;

				state = State.RUNNING;
			}

			break;
		case State.FLEEING:

			Vector3 direction = transform.position - PlayerController.Instance.transform.position;

			// If out of the alert radius, stop running away.
			if (direction.magnitude > FLEE_RADIUS_END)
			{
				state = State.PAUSING;
				break;
			}

			// Run away from the player.
			targetAngle = Mathf.Rad2Deg * Mathf.Atan2 (direction.x, direction.z);

			transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler (0, targetAngle, 0), 80 * Time.deltaTime);

			// Increase the speed to twice the standard speed.
			currentSpeed += Time.deltaTime * standardSpeed;
			if (currentSpeed >= standardSpeed * 2)
				currentSpeed = standardSpeed * 2;

			break;
		}

	}

	public override void Init (params object[] initParams)
	{
		// Randomise the direction.
		transform.rotation = Quaternion.Euler(0, Random.Range (0, 360), 0);
	}
}
