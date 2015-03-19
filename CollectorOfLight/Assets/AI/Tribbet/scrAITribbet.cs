using UnityEngine;
using System.Collections;

public class scrAITribbet : MobileObstacleMoveScript
{

	const float JUMP_DISTANCE = 40.0f;
	const float JUMP_HEIGHT = 30.0f;
	const float JUMP_SPEED = 0.3f;
	Vector3 startPoint, controlPoint, jumpPoint;	// Where the tribbet starts and ends when jumping.
	float jumpAmount = 0.0f;
	bool jumping = false;

	const float TIME_UNTIL_JUMP_MAX = 4.0f;
	const float TIME_UNTIL_JUMP_MIN = 1.0f;
	float timeUntilNextJump = 0.0f;

	public AudioClip scream;

	void Start ()
	{
	}

	protected override void SpecificMovement ()
	{
		if (jumping)
		{
			audio.PlayOneShot(scream);
			jumpAmount += Time.deltaTime * JUMP_SPEED;
			if (jumpAmount >= 1.0f)
			{
				jumpAmount = 1.0f;
				jumping = false;
				MoveModelToLandscape = true;
			}

			// Move the base linearly to the next point.
			transform.position = Vector3.Lerp (new Vector3(startPoint.x, 0, startPoint.z), new Vector3(jumpPoint.x, 0, jumpPoint.z), jumpAmount);

			// Move the model's y through a curve.
			model.transform.position = Mathf.Pow (1 - jumpAmount, 2) * startPoint + 2 * jumpAmount * (1 - jumpAmount) * controlPoint + jumpAmount * jumpAmount * jumpPoint;

		}
		else
		{
			timeUntilNextJump -= Time.deltaTime;
			if (timeUntilNextJump <= 0.0f)
			{
				timeUntilNextJump = Random.Range(TIME_UNTIL_JUMP_MIN, TIME_UNTIL_JUMP_MAX);

				float angle = Random.Range (0, 360);
				transform.eulerAngles = new Vector3(0, angle, 0);
				startPoint = model.transform.position;
				jumpPoint = transform.position + new Vector3(Mathf.Sin (angle * Mathf.Deg2Rad), 0.0f, Mathf.Cos (angle * Mathf.Deg2Rad)) * JUMP_DISTANCE;
				jumpPoint.y = scrLandscape.Instance.GetHeight(jumpPoint.x, jumpPoint.z) + transform.localScale.y * 0.5f;

				controlPoint = startPoint + (jumpPoint - startPoint) * 0.5f;
				controlPoint += Vector3.up * JUMP_HEIGHT;

				jumpAmount = 0.0f;
				jumping = true;
				MoveModelToLandscape = false;
			}
		}
		
	}

	public override void Init (params object[] initParams)
	{
		Expired = false;

		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

		// Randomise the direction.
		transform.rotation = Quaternion.Euler(0, Random.Range (0, 360), 0);

		// Make sure speed is 0.
		standardSpeed = 0;

		float timeUntilNextJump = Random.Range (TIME_UNTIL_JUMP_MIN, TIME_UNTIL_JUMP_MAX + 1);
	}
}
