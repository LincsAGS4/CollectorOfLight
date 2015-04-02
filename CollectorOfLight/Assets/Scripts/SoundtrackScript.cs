using UnityEngine;
using System.Collections;

public class SoundtrackScript : MonoBehaviour
{
	public AudioClip[] soundtrack;

	// Use this for initialization
	void Start()
	{
		if (!audio.playOnAwake)
		{
			audio.clip = soundtrack[Random.Range(0, soundtrack.Length)];
			audio.Play();
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!audio.isPlaying)
		{
			audio.clip = soundtrack[Random.Range(0, soundtrack.Length)];
			audio.Play();
		}
	}
}