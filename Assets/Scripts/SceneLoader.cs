using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

	public int scene;
	public bool autoStart = false;
	public float timeLimit = 0f;

	private float angle = 0f;
	private bool loadingScene = false;
	private AsyncOperation async;
	private Image image;

	private float timeGone = 0f;

	public GameObject displayText;

	void Start() {
		image = GetComponent<Image> ();
		image.color = new Color (1, 1, 1, 0);

		Cursor.visible = false;

		if (autoStart) {
			Invoke ("StartLoading", 5f);
		}
	}

	// Updates once per frame
	void Update() {

		timeGone += Time.deltaTime;

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log ("Quit...");
			Application.Quit ();
			return;
		}

		if (!loadingScene && Input.anyKeyDown && timeGone >= timeLimit) {
			StartLoading ();
		}
		
		// If the new scene has started loading...
		if (loadingScene == true) {
			angle -= 7;
			transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, angle));
		}

	}

	IEnumerator LoadNewScene() {
		async = SceneManager.LoadSceneAsync (scene);
		async.allowSceneActivation = true;
		yield return async;
	}

	public void StartLoading() {

		if (loadingScene) {
			return;
		}

		image.color = new Color (1, 1, 1, 0.75f);
		loadingScene = true;

		if (displayText) {
			displayText.SetActive (false);
		}

		StartCoroutine (LoadNewScene ());
	}

}
