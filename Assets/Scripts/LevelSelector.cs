using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour {

	private int current;
	private Planet[] planets;

	void Awake() {
		Application.targetFrameRate = 60;
	}

	// Use this for initialization
	void Start () {

		int idx = 0;

		// find currently active level
		foreach(Transform child in transform) {
			if (child.gameObject.activeSelf) {
				current = idx;
			}

			idx++;
		}

		ActivatePlanets ();
	}

	void Update() {

		if (Input.GetKeyDown (KeyCode.KeypadPlus)) {
			NextLevel ();
		}

		if (Input.GetKeyDown (KeyCode.KeypadMinus)) {
			NextLevel (-1);
		}
	}

	public bool NextLevel(int dir = 1) {

		bool looped = false;

		// deactivate current level
		transform.GetChild (current).gameObject.SetActive (false);

		current += dir;

		if (current >= transform.childCount) {
			current = 0;
			looped = true;
		}

		if (current < 0) {
			current = transform.childCount - 1;
		}

		// activate next level
		transform.GetChild (current).gameObject.SetActive (true);

		ActivatePlanets ();

		return looped;
	}

	void ActivatePlanets() {
		planets = transform.GetChild (current).gameObject.GetComponentsInChildren<Planet> ();
		Debug.Log (planets.Length + " planets found");
	}

	public Vector2 ClosestPlanet(Vector2 pos) {

		float distance = 9999f;
		Planet p = null;

		for (int i = 0; i < planets.Length; i++) {
			float d = (pos - (Vector2)planets [i].transform.position).magnitude / planets[i].Mass();

			if (d < distance) {
				p = planets [i];
				distance = d;
			}
		}

//		Debug.DrawLine (pos, p.transform.position, Color.red, 0.5f);

		return p.transform.position;
	}
}
