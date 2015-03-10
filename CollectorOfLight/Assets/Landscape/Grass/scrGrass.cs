using UnityEngine;
using System.Collections;

public class scrGrass : MonoBehaviour
{
	public float Height;
	public int LayerCount;
	public Material LayerMaterial;

	private MeshFilter filter;
	private MeshFilter[] layers;

	void Awake ()
	{
		filter = GetComponent<MeshFilter>();

		GameObject layer = new GameObject("Layer", typeof(MeshFilter), typeof(MeshRenderer));
		layer.renderer.material = LayerMaterial;

		layers = new MeshFilter[LayerCount];
		for (int i = 0; i < layers.Length; ++i)
		{
			layers[i] = ((GameObject)Instantiate(layer)).GetComponent<MeshFilter>();
			layers[i].transform.parent = transform;
			layers[i].transform.localScale = Vector3.one;
			layers[i].transform.localPosition = Vector3.zero;// new Vector3(0.0f, Height * 0.1f, 0.0f); (Use this if using the vertex/fragment version of the grass shader to avoid ztest problems).
			layers[i].renderer.material.SetFloat("_Height", Height);
			layers[i].renderer.material.SetFloat("_Part", (float)i / layers.Length);
		}

		Destroy (layer);
	}

	void Update ()
	{
		for (int i = 0; i < layers.Length; ++i)
		{
			layers[i].sharedMesh = filter.sharedMesh;
		}
	}
}
