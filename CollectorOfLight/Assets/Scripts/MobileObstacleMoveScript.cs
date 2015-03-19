using UnityEngine;
using System.Collections;

public abstract class MobileObstacleMoveScript : scrPoolable 
{
	public GameObject model;		//The actual model

    public float currentSpeed;   //The current speed the creature is moving at
    public float adjustedSpeed;     //The speed of the creature adjusted to account for slopes
    public float standardSpeed;     /*The default speed of the creature, that it will return to after its speed is
                                     *altered (This is determined by the upgrade system)*/
    
    public float verticalAngle;

    protected Time time;              //Used to track time for exponential decays

	public bool MoveModelToLandscape = true;
	public bool RagdollActive = false;
	protected float ragdollDuration = 3.0f;
	protected float ragdollTimer = 0.0f;

	// Use this for initialization
	void Awake () 
    {
		if (standardSpeed == 0)
			standardSpeed = currentSpeed;
        
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
		base.Update();

		if (RagdollActive)
		{
			ragdollTimer += Time.deltaTime;
			if (ragdollTimer >= ragdollDuration)
			{
				RagdollActive = false;
				rigidbody.isKinematic = true;

				transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
				transform.rotation = Quaternion.identity;

			}

			return;
		}

        SpecificMovement();

		adjustedSpeed = currentSpeed * (1 + Vector3.Dot(transform.forward, model.transform.up) * 0.5f);

    }

	void FixedUpdate()
	{		
		if (MoveModelToLandscape && !RagdollActive)
		{
			// Move the rigidbody forwards.
			rigidbody.MovePosition(rigidbody.position + transform.forward * adjustedSpeed * Time.fixedDeltaTime);

			// Set the y of the model.
			model.transform.position = new Vector3(model.transform.position.x, scrLandscape.Instance.GetHeightFromNoise(rigidbody.position.x, rigidbody.position.z), model.transform.position.z);

			// Reset the model's rotation.
			model.transform.rotation = Quaternion.identity;

			// Find the height at each corner of the model.
			Vector3 frontLeft = new Vector3(model.collider.bounds.min.x, 0, model.collider.bounds.max.z);
			frontLeft.y = scrLandscape.Instance.GetHeightFromNoise(frontLeft.x, frontLeft.z);
			
			Vector3 frontRight = new Vector3(model.collider.bounds.max.x, 0, model.collider.bounds.max.z);
			frontRight.y = scrLandscape.Instance.GetHeightFromNoise(frontRight.x, frontRight.z);
			
			Vector3 backLeft = new Vector3(model.collider.bounds.min.x, 0, model.collider.bounds.min.z);
			backLeft.y = scrLandscape.Instance.GetHeightFromNoise(backLeft.x, backLeft.z);
			
			Vector3 backRight = new Vector3(model.collider.bounds.max.x, 0, model.collider.bounds.min.z);
			backRight.y = scrLandscape.Instance.GetHeightFromNoise(backRight.x, backRight.z);
			
			// Get the average normal.
			float precision = scrLandscape.CELL_SCALE * 0.5f;
			model.transform.up = (scrLandscape.Instance.GetNormalFromNoise(frontLeft.x, frontLeft.z, precision) +
			                      scrLandscape.Instance.GetNormalFromNoise(frontRight.x, frontRight.z, precision) +
			                      scrLandscape.Instance.GetNormalFromNoise(backLeft.x, backLeft.z, precision) +
			                      scrLandscape.Instance.GetNormalFromNoise(backRight.x, backRight.z, precision)).normalized;


			// Make the model point in the same direction as the main object.
			model.transform.Rotate (0, transform.eulerAngles.y, 0, Space.Self);
			
			#region Debug Rays
			Debug.DrawRay(frontLeft, Vector3.up);
			Debug.DrawRay(frontLeft, scrLandscape.Instance.GetNormalFromNoise(frontLeft.x, frontLeft.z, precision));
			
			Debug.DrawRay(frontRight, Vector3.up);
			Debug.DrawRay(frontRight, scrLandscape.Instance.GetNormalFromNoise(frontRight.x, frontRight.z, precision));
			
			Debug.DrawRay(backLeft, Vector3.up);
			Debug.DrawRay(backLeft, scrLandscape.Instance.GetNormalFromNoise(backLeft.x, backLeft.z, precision));
			
			Debug.DrawRay(backRight, Vector3.up);
			Debug.DrawRay(backRight, scrLandscape.Instance.GetNormalFromNoise(backRight.x, backRight.z, precision));
			
			Debug.DrawLine(model.transform.position, model.transform.position + model.transform.up * 100);
			#endregion
		}
	}

	public void Ragdollize()
	{
		// Since the model isn't boned right now, simply enable the rigidbody component.
		RagdollActive = true;
		ragdollTimer = 0.0f;
		rigidbody.isKinematic = false;

		transform.position = model.transform.position;
		model.transform.localPosition = Vector3.zero;

		// Bounce back.
		rigidbody.velocity = (transform.up - transform.forward).normalized * rigidbody.mass * 10;
		rigidbody.AddTorque(Random.rotation.eulerAngles * 5, ForceMode.Impulse);
	}

    //to be overwritten by an inheriting class with the specific adjustments to direction and speed BEFORE adjustments
    protected abstract void SpecificMovement();
}
