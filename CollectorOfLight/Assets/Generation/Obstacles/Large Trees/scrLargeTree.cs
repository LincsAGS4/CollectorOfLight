using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class scrLargeTree : scrPoolable
{
	public float scaleMin = 1.0f;
	public float scaleMax = 2.0f;
    public float fruitMin = 2.0f;
    public float fruitMax = 5.0f;

    public List<GameObject> fruitPrefabs;
    List<GameObject> attachedFruit = new List<GameObject>();
	// ---- scrPoolable ----
	
	/// <param name="initParams">float x, float z</param>
	public override void Init(params object[] initParams)
	{
		Expired = false;
		
		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

        // Orient to the ground.
        transform.up = scrLandscape.Instance.GetNormal(x, z);

        // Randomise scale.
        float scale = Random.Range(scaleMin, scaleMax);
        transform.localScale = new Vector3(scale, scale, scale);

        // Spawn Fruits
        if (attachedFruit.Count == 0)
        {
            //Spawn in new fruit on the object
            float fruitCount = Random.Range(fruitMin, fruitMax);
            for (int i = 0; i < fruitCount; i++)
            {
                GameObject fruit = Instantiate(fruitPrefabs[0]) as GameObject;
                fruit.transform.parent = gameObject.transform;
                fruit.transform.position = gameObject.transform.position;
                fruit.transform.up = gameObject.transform.up;
                fruit.transform.position += new Vector3(Random.Range(-2f, 2f), 5, Random.Range(-2f, 2f));
                attachedFruit.Add(fruit);

            }
        }
        else
        {
            //Reset the fruit to start positions
            foreach (GameObject g in attachedFruit)
            {
                g.transform.position = gameObject.transform.position;
                g.transform.up = gameObject.transform.up;
                g.transform.position += new Vector3(Random.Range(-2f, 2f), 5, Random.Range(-2f, 2f));
            }
        }

		// Randomise yaw.
		transform.Rotate(Vector3.up, Random.Range (0, 360), Space.Self);
	}
}
