using UnityEngine;
using System.Collections;

public class scrPlayer : MonoBehaviour 
{
	public static scrPlayer Instance { get; private set; }

	public Vector2 Velocity2D { get; private set; }
	public float Speed2D { get; private set; }
	public int LightScore { get; private set; }

	// Use this for initialization
	void Awake () {
		Instance = this;
		LightScore = 1;
	}
	
	// Update is called once per frame
	void Update () {
		Velocity2D = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
		Speed2D = Velocity2D.magnitude;
	}
}
