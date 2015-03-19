using UnityEngine;
using System.Collections;

public class scrAIEllek : MobileObstacleMoveScript
{
	float FOLLOW_OFFSET = 3.0f;
	float FOLLOW_RADIUS = 10.0f;
	float FOLLOW_DURATION_MIN = 10.0f;
	float FOLLOW_DURATION_MAX = 20.0f;
	float followDuration;
	float followTimer = 0.0f;

	float turnDelay = 6.0f;
	float turnTimer = 0.0f;

	float targetAngle;

	enum State
	{
		RUNNING,
		FOLLOWING
	}
	State state = State.FOLLOWING;

	void Start ()
	{
	}

	protected override void SpecificMovement ()
	{
		switch (state)
		{
		case State.RUNNING:

			if (followTimer > 0)
			{
				followTimer -= Time.deltaTime;
			}
			else if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= FOLLOW_RADIUS)
			{
				state = State.FOLLOWING;
				followDuration = Random.Range (FOLLOW_DURATION_MIN, FOLLOW_DURATION_MAX);
				break;
			}

			turnTimer += Time.deltaTime;
			if (turnTimer >= turnDelay)
			{
				turnTimer = 0.0f;

				// Set the angle to turn to.
				targetAngle = transform.eulerAngles.y + Random.Range (-180, 180);
			}
			break;
		case State.FOLLOWING:

			followTimer += Time.deltaTime;
			if (followTimer >= followDuration)
			{
				state = State.RUNNING;
				break;
			}

			// Increase speed to be faster or slower than the player's speed depending on whether in front or behind.
			if (Vector3.Dot(transform.position - PlayerController.Instance.transform.position, PlayerController.Instance.transform.forward) < 0) // Behind
			{
				currentSpeed += Time.deltaTime * standardSpeed;
				if (currentSpeed > PlayerController.Instance.ellekSystem.currentSpeed * 1.2f)
					currentSpeed = PlayerController.Instance.ellekSystem.currentSpeed * 1.2f;

				// Get a position next to the player.
				Vector3 target = PlayerController.Instance.transform.position + (Vector3)(PlayerController.Instance.Velocity2D * Time.deltaTime);
				if (Vector3.Dot (transform.position - PlayerController.Instance.transform.position, PlayerController.Instance.transform.right) < 0)	// Left
					target -= PlayerController.Instance.transform.right * FOLLOW_OFFSET;
				else
					target += PlayerController.Instance.transform.right * FOLLOW_OFFSET;
				
				// Run towards that position.
				Vector3 direction = target - transform.position;
				targetAngle = Mathf.Rad2Deg * Mathf.Atan2 (direction.x, direction.z);
			}
			else
			{
				currentSpeed -= Time.deltaTime * standardSpeed;
				if (currentSpeed < PlayerController.Instance.ellekSystem.currentSpeed * 0.8f)
					currentSpeed = PlayerController.Instance.ellekSystem.currentSpeed * 0.8f;

				// Run in the same direction as the player.
				targetAngle = Mathf.Rad2Deg * Mathf.Atan2 (PlayerController.Instance.transform.forward.x, PlayerController.Instance.transform.forward.z);
			}


			break;
		}

		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler (0, targetAngle, 0), 45 * Time.deltaTime);
	}

	public override void Init (params object[] initParams)
	{
		Expired = false;

		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

		// Randomise the direction.
		transform.rotation = Quaternion.Euler(0, Random.Range (0, 360), 0);
	}
}
