using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class scrEllekStatus : MonoBehaviour
{
	#region Powerups
	public static float PowerupDuration = 20.0f;	// Starts at 20 seconds, can be upgraded in shop.
	
	// Speed powerup.
	public static float SpeedPowerupBoost = 2.0f;
	public bool speedPowerupActive = false;
	float speedPowerupTimer = 0.0f;
	
	// Ghost powerup.
	float ghostPowerupRespawnDuration = 3.0f;
	public bool ghostPowerupActive = false;
	float ghostPowerupTimer = 0.0f;
	
	// Magnet powerup.
	public static float MagnetPowerupRadius = 100.0f;
	public static float MagnetAttractSpeed = 1.0f;
	public bool magnetPowerupActive = false;
	float magnetPowerupTimer = 0.0f;
	
	// Light powerup.
	public static int LightPowerupReward = 20;
	#endregion

	float originalFOV;
	public EllekMoveScript ellekSystem;

	public GameObject FakeOrbPrefab;
	private Canvas canvas;


	// Use this for initialization
	void Start ()
	{
		originalFOV = Camera.main.fieldOfView;
		canvas = GameObject.Find ("World Canvas").GetComponent<Canvas>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (speedPowerupActive)
		{
			ellekSystem.currentSpeed = ellekSystem.standardSpeed * Mathf.Lerp (SpeedPowerupBoost, 1.0f, speedPowerupTimer / PowerupDuration);
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, Mathf.Lerp (originalFOV + 20, originalFOV, speedPowerupTimer / PowerupDuration), 0.2f);
			speedPowerupTimer -= Time.deltaTime;
			if (speedPowerupTimer <= 0)
			{
				speedPowerupActive = false;
				speedPowerupTimer = 0.0f;
			}

		}
		
		if (ghostPowerupActive)
		{
			ghostPowerupTimer -= Time.deltaTime;
			if (ghostPowerupTimer <= 0)
			{
				ghostPowerupActive = false;
				ghostPowerupTimer = 0.0f;
			}
		}
		
		if (magnetPowerupActive)
		{
			Collider[] orbs = Physics.OverlapSphere(ellekSystem.model.transform.position, MagnetPowerupRadius, 1 << LayerMask.NameToLayer("Light"));
			if (orbs.Length != 0)
			{
				foreach (Collider c in orbs)
				{
					scrLightOrb orb = c.GetComponent<scrLightOrb>();
					orb.AnchorPosition = Vector3.Lerp (orb.SpawnPosition, ellekSystem.model.transform.position, MagnetAttractSpeed / Vector3.Distance(orb.transform.position, ellekSystem.model.transform.position));
					orb.transform.Find ("Orb").transform.localScale = Vector3.one * Mathf.Lerp (1.0f, 0.2f, MagnetAttractSpeed / Vector3.Distance(orb.transform.position, ellekSystem.model.transform.position));
				}
			}
			
			magnetPowerupTimer -= Time.deltaTime;
			if (magnetPowerupTimer <= 0)
			{
				magnetPowerupActive = false;
				magnetPowerupTimer = 0.0f;
			}
		}
	}

	
	void OnTriggerEnter(Collider other)
	{
		if (!ellekSystem.RagdollActive && other.gameObject.layer == LayerMask.NameToLayer("Powerup"))
		{
			if (other.GetComponent<scrPowerup>())
			{
				switch(other.GetComponent<scrPowerup>().Effect)
				{
				case scrPowerup.Powerup.Speed:
					speedPowerupActive = true;
					speedPowerupTimer = PowerupDuration;
					break;
				case scrPowerup.Powerup.Ghost:
					ghostPowerupActive = true;
					ghostPowerupTimer = PowerupDuration;
					break;
				case scrPowerup.Powerup.Magnet:
					magnetPowerupActive = true;
					magnetPowerupTimer = PowerupDuration;
					break;
				case scrPowerup.Powerup.MoreLight:
					GetComponent<PlayerController>().LightScore += LightPowerupReward;
					canvas.transform.Find ("Light").GetComponent<Text>().text = GetComponent<PlayerController>().LightScore.ToString();
					break;
				}
			}
		}

		if (!ellekSystem.RagdollActive && other.gameObject.layer == LayerMask.NameToLayer ("Obstacle")) 
		{
			if (!ghostPowerupActive && !other.GetComponent<scrLightOrb>())
			{
				
				ellekSystem.Ragdollize();
				ghostPowerupActive = true;
				ghostPowerupTimer = ghostPowerupRespawnDuration;
				
				StartCoroutine(ReleaseOrbs());
			}
		}
	}

	IEnumerator ReleaseOrbs()
	{
		if (PlayerController.Instance.LightScore != 0)
		{
			// The light score should be halved.
			int nextLightScore = PlayerController.Instance.LightScore / 2;
			int lightToRelease = PlayerController.Instance.LightScore - nextLightScore;

			// Fling out light orbs in a circle.
			for (int i = 0; i < lightToRelease; ++i)
			{
				float angle = (float)i / (lightToRelease / 4) * Mathf.PI * 2;
				GameObject orb = ((GameObject)Instantiate(FakeOrbPrefab, ellekSystem.model.transform.position, Quaternion.identity));
				orb.rigidbody.velocity = new Vector3(Mathf.Sin(angle), 0.5f, Mathf.Cos (angle)) * Random.Range (10.0f, 20.0f);
				orb.rigidbody.velocity += rigidbody.velocity;
				--PlayerController.Instance.LightScore;
				canvas.transform.Find ("Light").GetComponent<Text>().text = PlayerController.Instance.LightScore.ToString();
				yield return new WaitForSeconds(1.0f / lightToRelease);
			}
		}
	}

	void EndTheGame()
	{
		Application.LoadLevel("MainMenu");
	}
}
