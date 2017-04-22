using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinPitch : MonoBehaviour {

	public float seed = 0f;
	public float speed = 1f;

	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		audioSource.pitch = 0.25f + Mathf.PerlinNoise (Time.time * speed, seed) * 0.25f;
	}
}
