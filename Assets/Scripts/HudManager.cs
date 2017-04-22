using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour {

	public Text infoDisplay;

	private float messageAlpha = 0;

	private static HudManager instance = null;
	public static HudManager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}
	}

	void Update() {
		messageAlpha = Mathf.MoveTowards (messageAlpha, 0, Time.deltaTime / Time.timeScale);

		if (messageAlpha <= 1f) {
			infoDisplay.color = new Color (1, 1, 1, messageAlpha);
		}
	}

	public void DisplayMessage(string msg, float delay) {
		infoDisplay.text = msg;
		infoDisplay.color = Color.white;
		messageAlpha = 1f + delay;
	}
}
