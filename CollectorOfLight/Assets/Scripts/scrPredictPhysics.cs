using UnityEngine;
using System.Collections;

/// <summary>
/// Predicts physics instead of using built-in physics. Use for objects outside of the terrain collider, for example.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class scrPredictPhysics : MonoBehaviour
{
	public bool ForcePrediction = false;
	public bool UsingPrediction { get; private set; }
	public bool OnGround { get; private set; }

	private delegate void Prediction();
	private Prediction prediction;

	void Awake ()
	{
		bool IHaventProgrammedThisYet = true;
		if (IHaventProgrammedThisYet)
		{
			prediction = CrapPrediction;
			return;
		}

		// Determine the prediction function to use.
		if (GetComponent<SphereCollider>())
			prediction = SpherePrediction;
		else if (GetComponent<BoxCollider>())
			prediction = BoxPrediction;
		else if (GetComponent<CapsuleCollider>())
			prediction = CapsulePrediction;
	}

	void Update ()
	{
		UsingPrediction = ForcePrediction | !scrLandscape.Instance.PhysContains(transform.position) | transform.position.y < scrLandscape.Instance.LowestPoint.y;	 // Set whether prediction is necessary.
	}

	void FixedUpdate()
	{
		if (UsingPrediction)
			prediction();
	}

	void CrapPrediction()
	{
		// Perform crappy almost-sphere prediction.
		float groundHeight = scrLandscape.Instance.GetHeightFromNoise(transform.position.x, transform.position.z);
		if (transform.position.y - transform.localScale.y * 0.5f < groundHeight)
		{
			transform.position = new Vector3(transform.position.x, groundHeight + transform.localScale.y * 0.5f, transform.position.z);

			// Bounce a bit away from the landscape.
			float speed = rigidbody.velocity.magnitude;
			rigidbody.velocity = scrLandscape.Instance.GetNormalFromNoise(transform.position.x, transform.position.z, 0.5f) * speed * 0.6f;

			OnGround = true;
		}
		else
		{
			OnGround = false;
		}
	}

	void SpherePrediction()
	{
		SphereCollider sphere = GetComponent<SphereCollider>();

		// Not implemented.
	}

	void BoxPrediction()
	{
		BoxCollider box = GetComponent<BoxCollider>();

		// Not implemented.
	}

	void CapsulePrediction()
	{
		CapsuleCollider box = GetComponent<CapsuleCollider>();
		
		// Not implemented.
	}
}
