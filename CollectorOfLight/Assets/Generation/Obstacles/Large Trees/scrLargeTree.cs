using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scrLargeTree : scrPoolable
{
	public float scaleMin = 1.0f;
	public float scaleMax = 2.0f;
    public float fruitMin = 2.0f;
    public float fruitMax = 5.0f;
	bool fruitDrop = false;

    public List<GameObject> fruitPrefabs;
    public List<GameObject> fruitSpawnPoints = new List<GameObject>();
    List<GameObject> attachedFruit = new List<GameObject>();
    
	// ---- scrPoolable ----
	
	/// <param name="initParams">float x, float z</param>
	public override void Init(params object[] initParams)
	{
		Expired = false;

		fruitDrop = false;
		
		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

        // Orient to the ground.
        transform.up = scrLandscape.Instance.GetNormal(x, z);

        // Randomise scale.
        float scale = Random.Range(scaleMin, scaleMax);
        transform.localScale = new Vector3(scale, scale, scale);


		float capsuleRadius = GetComponentInChildren<CapsuleCollider>().radius * transform.localScale.y * scale*0.5f;
		float capsuleHeight = GetComponentInChildren<CapsuleCollider> ().height * transform.localScale.y * scale*0.45f;

		float spawnBounds;


        // Spawn Fruits
        if (attachedFruit.Count == 0)
        {
            //Spawn in new fruit on the object
            float fruitCount = Random.Range(fruitMin, fruitMax);
            for (int i = 0; i < fruitCount; i++)
            {
                GameObject fruit = Instantiate(fruitPrefabs[Random.Range(0,fruitPrefabs.Count)]) as GameObject;
                fruit.transform.parent = gameObject.transform;
                fruit.transform.position = gameObject.transform.position;
                fruit.transform.up = gameObject.transform.up;
                fruit.transform.localPosition = fruitSpawnPoints[i].transform.localPosition;
                attachedFruit.Add(fruit);
            }
        }
        else
		{
            //Reset the fruit to start positions
            for (int i = 0; i < attachedFruit.Count; i++)
            {
                attachedFruit[i].transform.localPosition = fruitSpawnPoints[i].transform.localPosition;
            }
        }

		// Randomise yaw.
		transform.Rotate(Vector3.up, Random.Range (0, 360), Space.Self);
	}

	protected override void Update ()
	{
		base.Update ();

		if(scrLandscape.Instance.PhysContains(transform.position) && !fruitDrop)
		{
			foreach(GameObject fruit in attachedFruit)
			{
				fruit.rigidbody.isKinematic = false;
				fruit.rigidbody.AddForce(fruit.transform.up);
			}

			fruitDrop = true;
		}
	}
}
