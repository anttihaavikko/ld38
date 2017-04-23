using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class SmoothCamera2D : MonoBehaviour {

	public float dampTime = 0.15f;
	public Transform target;

	private VignetteAndChromaticAberration chroma;
	private Bloom bloom;

	private float shakeAmount = 0, shakeTime = 0;

	private Rigidbody2D body;

	void Start() {
		chroma = GetComponent<VignetteAndChromaticAberration> ();
		bloom = GetComponent<Bloom> ();
		body = target.GetComponent<Rigidbody2D> ();
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

			float dist = Mathf.Clamp (target.transform.position.magnitude * 1.5f, 5f, 12f);

			Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, dist, 0.025f);

			float b = (body.velocity.magnitude > 5f) ? body.velocity.magnitude - 5f : 0f;
			b = Mathf.Clamp(0.4f + b * 0.1f, 0.4f, 2f);
			bloom.bloomIntensity = Mathf.Clamp(0.4f + b * 0.1f, 0.4f, 2f);
			bloom.bloomThreshold = Mathf.Clamp(0.4f - b * 0.1f, 0.2f, 0.4f);
		}

	}

	public void Shake(float amount, float time) {
		shakeAmount = amount;
		shakeTime = time;

		chroma.chromaticAberration = Screen.width / 7 * amount;
	}
}