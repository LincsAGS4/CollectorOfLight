using UnityEngine;
using System.Collections;

public class LightOrbSpin : MonoBehaviour 
{
	public int spinRate;
    public GameObject lightOrb;
		
	// Update is called once per frame
	void Update () 
    {
        //rotate orb by that amount
        Vector3 orbRotation = lightOrb.transform.eulerAngles;

        orbRotation.y += spinRate;
        //If y has passed useful location, reset it
        if (orbRotation.y > 360)
        { orbRotation.y -= 360; }

        lightOrb.transform.eulerAngles = orbRotation;
	}
}
