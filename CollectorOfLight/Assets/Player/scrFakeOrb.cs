using UnityEngine;
using System.Collections;

public class scrFakeOrb : MonoBehaviour
{
	public GameObject LightOrbPrefab;
	int bounces = 3;

	void Update()
	{
		if (GetComponent<scrPredictPhysics>().OnGround)
		{
			if (bounces == 0)
			{
				scrLightOrb orb = ((GameObject)Instantiate (LightOrbPrefab, transform.position, Quaternion.identity)).GetComponent<scrLightOrb>();
				orb.Init(transform.position.x, transform.position.z, 0, 1);
				orb.DestroyOnExpire = true;
				Destroy(gameObject);
			}
			--bounces;
		}
	}
}
