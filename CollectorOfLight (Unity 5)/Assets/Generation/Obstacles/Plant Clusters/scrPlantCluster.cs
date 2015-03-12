using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class scrPlantCluster : scrPoolable
{
	#region Static
	private static int[] plantIndexes = new int[TOTAL_PLANTS];

	public static void InitStatic()
	{
		for (int i = 0; i < TOTAL_PLANTS; ++i)
		{
			plantIndexes[i] = i; 
		}
	}
	#endregion

	const int TOTAL_PLANTS = 16;	// Must be wholly divisible by the number of plant prefabs.
	const int VISIBLE_PLANTS_MIN = 5;
	const int VISIBLE_PLANTS_MAX = 16;
	const float SCATTER_RADIUS_MIN = 10;
	const float SCATTER_RADIUS_MAX = 40;
	const float PLANT_SCALE_MIN = 0.8f;
	const float PLANT_SCALE_MAX = 3.0f;

	private Transform[] allChildren = new Transform[TOTAL_PLANTS];
	private Transform[] activeChildren;	// The children of this object that have been activated.

	public GameObject PlantAPrefab;
	public GameObject PlantBPrefab;
	public GameObject PlantCPrefab;
	public GameObject PlantDPrefab;

	void Awake ()
	{
		GameObject[] plantPrefabs = new GameObject[] { PlantAPrefab, PlantBPrefab, PlantCPrefab, PlantDPrefab };
		int numOfEachPlant = TOTAL_PLANTS / plantPrefabs.Length;

		// Spawn all plants, but make them inactive.
		int plantIndex = 0;
		for (int i = 0; i < plantPrefabs.Length; ++i)
		{
			for (int j = 0; j < numOfEachPlant; ++j)
			{
				GameObject plant = (GameObject)Instantiate(plantPrefabs[i], transform.position, Quaternion.identity);
				plant.transform.SetParent(transform);
				plant.SetActive(false);

				allChildren[plantIndex] = plant.transform;
				++plantIndex;
			}
		}
	}

	// ---- scrPoolable ----

	/// <param name="initParams">float x, float z, int numPlants</param>
	public override void Init(params object[] initParams)
	{
		Expired = false;
		
		float x = (float)initParams[0];
		float z = (float)initParams[1];
		transform.position = new Vector3(x, scrLandscape.Instance.GetHeight(x, z), z);

		int numPlants = (int)initParams[2];
		activeChildren = new Transform[numPlants];
		for (int i = 0; i < numPlants; ++i)
		{
			// Get a random child.
			int index = Random.Range(i, TOTAL_PLANTS);
			activeChildren[i] = allChildren[plantIndexes[index]];
			activeChildren[i].gameObject.SetActive(true);

			// Swap the indexes so the same child won't be randomly chosen again.
			int temp = plantIndexes[index];
			plantIndexes[index] = plantIndexes[i];
			plantIndexes[i] = temp;

			// Scale the plant randomly (do this before positioning so that large plants do not overlap near the centre.
			float scale = Random.Range (PLANT_SCALE_MIN, PLANT_SCALE_MAX);
			activeChildren[i].localScale = new Vector3(scale, scale, scale);

			// Position the plant such that all plants are scattered in a circle.
			float angle = (float)i / numPlants * Mathf.PI * 2;
			float radius = Random.Range(SCATTER_RADIUS_MIN + scale * 0.5f, SCATTER_RADIUS_MAX);
			float px = transform.position.x + Mathf.Sin(angle) * radius;
			float pz = transform.position.z + Mathf.Cos(angle) * radius;
			activeChildren[i].position = new Vector3(px, scrLandscape.Instance.GetHeight(px, pz), pz);

			// Rotate the plant to the normal of the landscape.
			activeChildren[i].up = scrLandscape.Instance.GetNormal(px, pz);

			// Rotate the plant randomly.
			activeChildren[i].Rotate(Vector3.up, Random.Range (0, 360), Space.Self);
		}
	}
}
