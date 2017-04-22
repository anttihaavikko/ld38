using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinPitch : MonoBehaviour {

	public float seed = 0f;
	public float speed = 1f;
	public float start = 0.25f;
	public float amount = 0.25f;

	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		audioSource.pitch = start + Mathf.PerlinNoise (Time.time * speed, seed) * amount;
	}
}
