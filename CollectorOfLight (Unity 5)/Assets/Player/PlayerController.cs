using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using KinectForWheelchair;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance { get; private set; }
	public float currentFacing;
	private float turnRate;

	public Vector2 Velocity2D { get; private set; }
	public float Speed2D { get; private set; }
	public int LightScore { get; set; }

    public InputController inputController;
    public EllekMoveScript ellekSystem;

	public Slider KinectSlider;



	void Awake()
	{
		Instance = this;
		currentFacing = 0;
		turnRate = 0.5f;
		LightScore = 0;
	}

	// Update is called once per frame
    void Update()
	{    				
		if (Input.GetKey(KeyCode.A)) 
		{
			currentFacing -= turnRate;
		}
		if (Input.GetKey(KeyCode.D)) 
		{
			currentFacing += turnRate;
		}
		
		if (ellekSystem.ChangeAngle((currentFacing*(Mathf.PI/180))) == false)
		{
			Debug.Log("Turning did not register; player rotated too far.");
		}

		Speed2D = ellekSystem.adjustedSpeed;
		Velocity2D = new Vector2(transform.forward.x, transform.forward.z) * Speed2D;

		if (!ellekSystem.RagdollActive)
		{
			Vector3 p = transform.position - transform.forward * 2.5f;
			Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, new Vector3(p.x, scrLandscape.Instance.GetHeightFromNoise(p.x, p.z) + 2.0f, p.z), 0.8f);
			Camera.main.transform.LookAt(transform.position + transform.forward * 30.0f);
		}
		else
		{
			Camera.main.transform.LookAt(transform.position);
		}

		// Get the input info
		SeatedInfo inputInfo = this.inputController.InputInfo;
		if (inputInfo == null)
			return;

		// Set the player position and direction
		if (inputInfo.Features == null)
			return;

		//Debug.Log (inputInfo.Features.Angle);
		KinectSlider.value = inputInfo.Features.Angle;

		//Debug.Log (inputInfo.Features.Position);
		if (ellekSystem.ChangeAngle(inputInfo.Features.Angle * Mathf.Deg2Rad))
		{
			Debug.Log("Turning did not register; player rotated too far.");
		}
		
		return;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<scrLightOrb>())
		{
			++LightScore;
		}
	}
}
