using UnityEngine;
using System.Collections;

public class scrPool : MonoBehaviour
{
	public string Name;
	public int Capacity;
	public int Available { get; private set; }
	public GameObject Prefab;

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
			pool[i] = ((GameObject)Instantiate(Prefab)).GetComponent<scrPoolable>();
			links[i] = i + 1;
		
			pool[i].gameObject.SetActive(false);
		}

		Available = Capacity;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Loop through the pool and, if any of the objects have become inactive, make them available by pointing their link towards the available index then making the index point to them.
		for (int i = 0; i < Capacity; ++i)
		{
			if (!pool[i].gameObject.activeSelf)
			{
				links[i] = index;
				index = i;

				++Available;
			}
		}
	}

	public scrPoolable Create(params object[] initParams)
	{
		if (index != Capacity)
		{
			// Activate and initialise the item.
			scrPoolable item = pool[index];
			item.gameObject.SetActive(true);
			item.Init(initParams);

			// Shift the index to the next available item.
			index = links[index];

			--Available;

			return item;
		}

		return null;
	}
}
