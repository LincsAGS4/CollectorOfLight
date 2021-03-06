﻿using UnityEngine;
using System.Collections;

public class scrEllekStatus : MonoBehaviour
{
	#region Powerups
	public static float PowerupDuration = 20.0f;	// Starts at 20 seconds, can be upgraded in shop.
	
	// Speed powerup.
	public static float SpeedPowerupBoost = 2.0f;
	bool speedPowerupActive = false;
	float speedPowerupTimer = 0.0f;
	
	// Ghost powerup.
	float ghostPowerupRespawnDuration = 3.0f;
	bool ghostPowerupActive = false;
	float ghostPowerupTimer = 0.0f;
	
	// Magnet powerup.
	public static float MagnetPowerupRadius = 100.0f;
	public static float MagnetAttractSpeed = 1.0f;
	bool magnetPowerupActive = false;
	float magnetPowerupTimer = 0.0f;
	
	// Light powerup.
	public static int LightPowerupReward = 20;
	#endregion

	public const int HEARTS_MAX = 8;
	public static int HeartCapacity = 3;
	public int Hearts = 3;
	
	float originalFOV;
	public EllekMoveScript ellekSystem;

	// Use this for initialization
	void Start ()
	{
		originalFOV = Camera.main.fieldOfView;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (speedPowerupActive)
		{
			speedPowerupTimer += Time.deltaTime;
			if (speedPowerupTimer >= PowerupDuration)
			{
				speedPowerupActive = false;
				speedPowerupTimer = 0.0f;
			}
			ellekSystem.currentSpeed = ellekSystem.standardSpeed * Mathf.Lerp (SpeedPowerupBoost, 1.0f, speedPowerupTimer / PowerupDuration);
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, Mathf.Lerp (originalFOV + 20, originalFOV, speedPowerupTimer / PowerupDuration), 0.2f);
		}
		
		if (ghostPowerupActive)
		{
			ghostPowerupTimer += Time.deltaTime;
			if (ghostPowerupTimer >= PowerupDuration)
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
			
			magnetPowerupTimer += Time.deltaTime;
			if (magnetPowerupTimer >= PowerupDuration)
			{
				magnetPowerupActive = false;
				magnetPowerupTimer = 0.0f;
			}
		}
	}

	
	void OnTriggerEnter(Collider other)
	{
		if (!ellekSystem.RagdollActive && other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
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
					break;
				}
			}
			else if (!ghostPowerupActive && !other.GetComponent<scrLightOrb>())
			{
				--Hearts;
				Debug.Log (Hearts);
				ellekSystem.Ragdollize();
				ghostPowerupActive = true;
				ghostPowerupTimer = ghostPowerupRespawnDuration;

				if (Hearts == 0)
				{
					ellekSystem.enabled = false;
					Invoke ("EndTheGame", 5.0f);
				}
			}
		}
	}

	void EndTheGame()
	{
		Application.LoadLevel("MainMenu");
	}
}
