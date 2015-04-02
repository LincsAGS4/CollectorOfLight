using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class scrPool : MonoBehaviour
{
	public string Identifier;
	public int Capacity;
	public int Remaining { get; private set; }
	public GameObject[] Prefabs;

	private scrPoolable[] pool;
	private int[] links;
	private int index;

	// Use this for initialization
	void Start ()
	{
		pool = new scrPoolable[Capacity];
		links = new int[Capacity];
		index = 0;

		for (int i = 0; i < Capacity; ++i)
		{
			// Instantiate each prefab, hopefully an equal amount of each.
			pool[i] = ((GameObject)Instantiate(Prefabs[i % Prefabs.Length])).GetComponent<scrPoolable>();
			links[i] = i + 1;
		
			pool[i].transform.SetParent(transform);
			pool[i].gameObject.SetActive(false);
		}

		Remaining = Capacity;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Loop through the pool and, if any of the objects have expired, make them available by pointing their link towards the available index then making the index point to them.
		for (int i = 0; i < Capacity; ++i)
		{
			if (pool[i].gameObject.activeSelf && pool[i].Expired)
			{
				// Deactivate the pool item.
				pool[i].gameObject.SetActive(false);

				links[i] = index;
				index = i;

				++Remaining;
			}
		}
	}

	public List<scrPoolable> GetAllActive()
	{
		List<scrPoolable> active = new List<scrPoolable>();
		for (int i = 0; i < Capacity; ++i)
		{
			if (pool[i].gameObject.activeSelf)
				active.Add(pool[i]);
		}
		return active;
	}

	public scrPoolable Create(params object[] initParams)
	{
		try
		{
			if (index != Capacity)
			{
				// Activate and initialise the item.
				scrPoolable item = pool[index];
				item.gameObject.SetActive(true);
				item.Init(initParams);
				Renderer[] allComponents = item.gameObject.GetComponentsInChildren<Renderer>();

				foreach(Renderer r in allComponents)
				{
					r.material.SetColor("_Color", new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
				}

				// Shift the index to the next available item.
				index = links[index];

				--Remaining;

				return item;
			}


		}
		catch
		{
			Debug.Log (Identifier);
		}
		return null;
	}
}
