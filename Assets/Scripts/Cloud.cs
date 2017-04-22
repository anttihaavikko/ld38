using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {

	private float speed;

	private float min = -40f;
	private float max = 40f;

	// Use this for initialization
	void Awake () {

		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();

		float r = Random.value * 4f;

		float depth = r * 50f + 5;

		float xdir = (Random.value < 0.5f) ? 1f : -1f;
		float ydir = (Random.value < 0.5f) ? 1f : -1f;

		transform.localPosition = new Vector3 (transform.localPosition.x + Random.Range(-20, 20), transform.localPosition.y + Random.Range(-5, 5), depth);
		transform.localScale = new Vector3 (xdir * (1f + r), ydir * (1f + r), 1f);

		sprite.color = new Color (1, 1, 1, Random.value * 0.05f);

		speed = (0.1f + Random.value * 2f) * 0.3f;
	}

	void Update() {
		transform.Translate(Vector3.right * Time.deltaTime * speed);

		if (transform.localPosition.x > max) {
			transform.localPosition = new Vector3 (min, transform.localPosition.y, transform.localPosition.z);
		}
	}
}
