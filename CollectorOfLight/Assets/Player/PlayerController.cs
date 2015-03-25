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
	public int GemScore { get; set; }

    public InputController inputController;
    public EllekMoveScript ellekSystem;

	public Slider KinectSlider;
	private Canvas overlayCanvas;
    private Canvas worldCanvas;
	private Image vignette;


	void Awake()
	{
		Instance = this;
		currentFacing = 0;
		turnRate = 0.5f;
		LightScore = 10;

		overlayCanvas = GameObject.Find ("Overlay Canvas").GetComponent<Canvas>();
        worldCanvas = GameObject.Find("World Canvas").GetComponent<Canvas>();
		worldCanvas.transform.Find ("Light").GetComponent<Text>().text = LightScore.ToString();
		vignette = overlayCanvas.transform.Find("Vignette").GetComponent<Image>();
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

		
		// Set the vignette colour to exponentially get whiter with the light score, with full white occurring at 50 orbs.
		vignette.color = Color.Lerp (Color.clear, Color.white, (LightScore * LightScore) / 2500.0f);

		Speed2D = ellekSystem.adjustedSpeed;
		Velocity2D = new Vector2(transform.forward.x, transform.forward.z) * Speed2D;

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

        if (Input.GetKey(KeyCode.L))
        {
            LightScore++;
        }

		// Set the vignette colour to exponentially get whiter with the light score, with full white occurring at 100 orbs.
		vignette.color = Color.Lerp (Color.clear, Color.white, (LightScore * LightScore) / 10000.0f);
	}

	void FixedUpdate()
	{
		if (!ellekSystem.RagdollActive)
		{
			Vector3 p = transform.position - transform.forward * 3.0f;
			Camera.main.transform.position = Vector3.Lerp (Camera.main.transform.position, new Vector3(p.x, scrLandscape.Instance.GetHeightFromNoise(p.x, p.z) + 3.0f, p.z), 0.8f);
			Camera.main.transform.LookAt(transform.position + transform.forward * 30.0f);
		}
		else
		{
			Camera.main.transform.LookAt(transform.position);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<scrLightOrb>())
		{
			++LightScore;
			worldCanvas.transform.Find ("Light").GetComponent<Text>().text = LightScore.ToString();
		}
		else if (other.GetComponent<scrGem>())
		{
			++GemScore;
			worldCanvas.transform.Find ("Gems").GetComponent<Text>().text = GemScore.ToString();
		}
	}
}
