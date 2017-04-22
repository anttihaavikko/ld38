using UnityEngine;
using System.Collections;

public class Blink : MonoBehaviour {

	private float timer;
	private float dir;
	private float size;

	public float speed = 1f;
	private Vector3 originalPos;



	// Use this for initialization
	void Start () {
		originalPos = transform.localPosition;

		ResetTimer ();
		size = transform.localScale.y;
	}
	
	// Update is called once per frame
	void Update () {

		timer += dir * speed;

		if (timer >= 0) {
			transform.localScale = new Vector2 (transform.localScale.x, size - size * timer / 10f);
		}

		if (timer <= 0 && dir < 0) {
			ResetTimer ();
		}

		if (timer >= 10) {
			dir = -1f;
		}

		float posX = Mathf.PerlinNoise(Time.time * 0.5f, 0f);
		float posY = Mathf.PerlinNoise(Time.time * 0.5f, 500f);

		transform.localPosition = new Vector3 (originalPos.x - 0.1f + posX * 0.2f, originalPos.y - 0.1f + posY * 0.2f, originalPos.z);
	}

	void ResetTimer() {
		timer = Random.Range (-50, -200);
		dir = 1f;
	}
}
