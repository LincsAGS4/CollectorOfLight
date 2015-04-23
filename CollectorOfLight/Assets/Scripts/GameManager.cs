using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	float duration = 60;
	Text timeDisplay, timeLbl;
	TimeSpan t;
	PlayerController pc;
	public bool timed;
	// Use this for initialization
	void Start () {
		pc = GameObject.Find ("Player").GetComponent<PlayerController> ();
		timeDisplay = GameObject.Find ("Time").GetComponent<Text> ();
		timeLbl = GameObject.Find ("TimeLbl").GetComponent<Text> ();
	if (timed) {
			timeDisplay.gameObject.SetActive(true);
			timeLbl.gameObject.SetActive(true);
			t = TimeSpan.FromSeconds (duration);
			timeDisplay.text = string.Format ("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

			if (!PlayerPrefs.HasKey ("Gem High Score")) {
				PlayerPrefs.SetInt ("Gem High Score", 0);
			}
			if (!PlayerPrefs.HasKey ("Light High Score")) {
				PlayerPrefs.SetInt ("Light High Score", 0);
			}
		}
		else
		{
			timeDisplay.gameObject.SetActive(false);
			timeLbl.gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){Application.Quit();}
		if (timed) {
			if (duration > 0) {
				duration -= Time.deltaTime;
			} else {
				PlayerPrefs.SetInt("LastGem", pc.GemScore);
				PlayerPrefs.SetInt("LastLight", pc.LightScore);
				if (pc.LightScore > PlayerPrefs.GetInt ("Light High Score")) {
					PlayerPrefs.SetInt ("Light High Score", pc.LightScore);
				}
				if (pc.GemScore > PlayerPrefs.GetInt ("Gem High Score")) {
					PlayerPrefs.SetInt ("Gem High Score", pc.GemScore);
				}
				Application.LoadLevel("EndGame");
				//PlayerPrefs.GetInt("Gem High Score");
			}
			t = TimeSpan.FromSeconds (duration);
			timeDisplay.text = string.Format ("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
		}
	
	}

	public void AddSeconds(float secondsToAdd)
	{
		if (timed) {
			duration += secondsToAdd;
		}
	}
}
