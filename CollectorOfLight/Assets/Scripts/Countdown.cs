using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {

	public Text gameStart;
	float secsTilStart;
	float secsTilStartDisp;
	public string sceneToLoad, message;

	// Use this for initialization
	void Start () {
		secsTilStart = 15;
	
	}
	
	// Update is called once per frame
	void Update () {

		secsTilStart -= Time.deltaTime;
		secsTilStartDisp = Mathf.RoundToInt (secsTilStart);

		gameStart.text = message + secsTilStartDisp.ToString();
		if (secsTilStart <= 0) {
			Application.LoadLevel(sceneToLoad);
		}
	
	}


}
