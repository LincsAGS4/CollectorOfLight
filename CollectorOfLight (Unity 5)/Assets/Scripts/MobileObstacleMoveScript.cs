using UnityEngine;
using System.Collections;

public abstract class MobileObstacleMoveScript : MonoBehaviour 
{
    public GameObject container;	//The empty gameobject determining position & rotation of the base of the object
	public GameObject model;		//The actual model

    public InputController inputController;

    public float currentSpeed;   //The current speed the creature is moving at
    public float adjustedSpeed;     //The speed of the creature adjusted to account for slopes
    public float standardSpeed;     /*The default speed of the creature, that it will return to after its speed is
                                     *altered (This is determined by the upgrade system)*/
    
    public float verticalAngle;

    protected Time time;              //Used to track time for exponential decays

	public bool RagdollActive = false;
	protected float ragdollDuration = 3.0f;
	protected float ragdollTimer = 0.0f;

	// Use this for initialization
	void Awake () 
    {
		standardSpeed = currentSpeed;
        
	}
	
	// Update is called once per frame
	void Update () 
    {
		if (RagdollActive)
		{
			ragdollTimer += Time.deltaTime;
			if (ragdollTimer >= ragdollDuration)
			{
				RagdollActive = false;
				GetComponent<Rigidbody>().isKinematic = true;

				transform.position = model.transform.position;
				Vector3 modelPos = transform.position;
				transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
				model.transform.position = modelPos;

			}

			return;
		}

        SpecificMovement();

        Vector3 speedAdjustNormal = scrLandscape.Instance.GetNormal(container.transform.position.x, container.transform.position.z);

        //vertical angle is 90 degrees minus the vertical component of the normals' angle
		/*float*/ verticalAngle = (Mathf.PI/2) - (Mathf.Asin(speedAdjustNormal.y / speedAdjustNormal.magnitude));
               
        //create an adjustedSpeed based on the angle of the normal
		if (verticalAngle >= 0) 
		{ adjustedSpeed = currentSpeed * (1 + (verticalAngle/60)); }
		else if (verticalAngle < 0) 
		{ adjustedSpeed = currentSpeed / (1 + (verticalAngle/60)); }

		//Move object along its current facing
        //Convert move speed to translation, translate Ellek along xz directions by its speed
		Vector3 newContainerPos = container.transform.position;

		newContainerPos += transform.forward * adjustedSpeed * Time.deltaTime;

		Vector3 newModelPos = container.transform.position;
        //update the new position with the y position from terrain
		newModelPos.y = scrLandscape.Instance.GetHeightFromNoise (newContainerPos.x, newContainerPos.z);// + model.transform.localScale.y * 0.5f;

        //The position won't be changed from here on out, so it can now be pushed to the container
		GetComponent<Rigidbody>().MovePosition(newContainerPos);
		model.transform.position = newModelPos;

        #region RotatingToNormal
        //raycast at the new position
		Vector3 up = scrLandscape.Instance.GetNormalFromNoise(container.transform.position.x, container.transform.position.z, 1.0f);
		//model.transform.up = Vector3.Lerp (model.transform.up, up, 0.5f);
		//model.transform.forward = transform.forward;

		Quaternion prevRot = model.transform.rotation;
		model.transform.LookAt(model.transform.position + transform.forward, up);
		model.transform.rotation = Quaternion.Lerp (prevRot, model.transform.rotation, 0.8f);
		model.transform.position += model.transform.up * model.transform.localScale.y;
        #endregion
    }

	public void Ragdollize()
	{
		// Since the model isn't boned right now, simply enable the rigidbody component.
		RagdollActive = true;
		ragdollTimer = 0.0f;
		GetComponent<Rigidbody>().isKinematic = false;

		transform.position = model.transform.position;
		model.transform.localPosition = Vector3.zero;

		// Bounce back.
		GetComponent<Rigidbody>().velocity = (transform.up - transform.forward).normalized * GetComponent<Rigidbody>().mass * 10;
		GetComponent<Rigidbody>().AddTorque(Random.rotation.eulerAngles * 5, ForceMode.Impulse);
	}

    //to be overwritten by an inheriting class with the specific adjustments to direction and speed BEFORE adjustments
    protected abstract void SpecificMovement();
}
