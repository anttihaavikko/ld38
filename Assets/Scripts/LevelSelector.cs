using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour {

	private int current;

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

		// log completion
		Application.ExternalCall("DoStats", "Completed " + transform.GetChild (current).gameObject.name + " (" + current + ")");

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

		return looped;
	}
}
