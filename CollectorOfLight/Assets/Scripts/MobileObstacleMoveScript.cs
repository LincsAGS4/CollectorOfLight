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

				transform.position = model.transform.position;
				Vector3 modelPos = transform.position;
				transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
				model.transform.position = modelPos;

			}

			return;
		}

        SpecificMovement();

        Vector3 speedAdjustNormal = scrLandscape.Instance.GetNormal(this.transform.position.x, this.transform.position.z);

        //vertical angle is 90 degrees minus the vertical component of the normals' angle
		/*float*/ verticalAngle = (Mathf.PI/2) - (Mathf.Asin(speedAdjustNormal.y / speedAdjustNormal.magnitude));
               
        //create an adjustedSpeed based on the angle of the normal
		if (verticalAngle >= 0) 
		{ adjustedSpeed = currentSpeed * (1 + (verticalAngle/60)); }
		else if (verticalAngle < 0) 
		{ adjustedSpeed = currentSpeed / (1 + (verticalAngle/60)); }

		if (MoveModelToLandscape)
		{

			//Move object along its current facing
	        //Convert move speed to translation, translate Ellek along xz directions by its speed
			Vector3 newthisPos = this.transform.position;

			newthisPos += transform.forward * adjustedSpeed * Time.deltaTime;

			Vector3 newModelPos = this.transform.position;
	        //update the new position with the y position from terrain
			newModelPos.y = scrLandscape.Instance.GetHeightFromNoise (newthisPos.x, newthisPos.z);// + model.transform.localScale.y * 0.5f;

	        //The position won't be changed from here on out, so it can now be pushed to the this
			rigidbody.MovePosition(newthisPos);
			model.transform.position = newModelPos;

	        #region RotatingToNormal
	        //raycast at the new position
			Vector3 up = scrLandscape.Instance.GetNormalFromNoise(this.transform.position.x, this.transform.position.z, 1.0f);
			//model.transform.up = Vector3.Lerp (model.transform.up, up, 0.5f);
			//model.transform.forward = transform.forward;

			Quaternion prevRot = model.transform.rotation;
			model.transform.LookAt(model.transform.position + transform.forward, up);
			model.transform.rotation = Quaternion.Lerp (prevRot, model.transform.rotation, 0.8f);
			model.transform.position += model.transform.up * model.transform.localScale.y * 0.5f;
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
