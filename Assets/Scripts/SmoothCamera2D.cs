using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class SmoothCamera2D : MonoBehaviour {

	public float dampTime = 0.15f;
	public Transform target;

	private VignetteAndChromaticAberration chroma;

	private float shakeAmount = 0, shakeTime = 0;

	void Start() {
		chroma = GetComponent<VignetteAndChromaticAberration> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (target) {

			if (shakeTime > 0) {
				shakeTime -= Time.deltaTime;
				transform.position += new Vector3 (Random.Range (-shakeAmount, shakeAmount), Random.Range (-shakeAmount, shakeAmount), 0);
			}

			chroma.chromaticAberration = Mathf.MoveTowards (chroma.chromaticAberration, 0, 1f);

			float dist = Mathf.Clamp (target.transform.position.magnitude * 1.5f, 5f, 10f);

			Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, dist, 0.025f);
		}

	}

	public void Shake(float amount, float time) {
		shakeAmount = amount;
		shakeTime = time;

		chroma.chromaticAberration = 100 * amount;
	}
}