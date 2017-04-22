using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinRotation : MonoBehaviour {

	float seed;
	float amount = 10f;

	// Use this for initialization
	void Start () {
		seed = Random.value * 1000f;
	}
	
	// Update is called once per frame
	void Update () {
		float a = Mathf.PerlinNoise(Time.time * 0.5f, seed);
		transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, -amount * 0.5f + a * amount));
	}
}
