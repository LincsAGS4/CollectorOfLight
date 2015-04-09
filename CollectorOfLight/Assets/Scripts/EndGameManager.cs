using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour {

	public Text gems, light;
	// Use this for initialization
	void Start () {
		gems.text = "You collected " + PlayerPrefs.GetInt ("LastGems").ToString () + " Gems, Your high score is " + PlayerPrefs.GetInt ("Gem High Score").ToString ();
		light.text = "You collected " + PlayerPrefs.GetInt ("LastLight").ToString () + " Light, Your high score is " + PlayerPrefs.GetInt ("Light High Score").ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
