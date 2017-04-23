using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour {

	private int level;

	public int Level {
		get { return level; }
		set { level = value; }
	}

	// singleton stuff

	private static SaveManager instance = null;

	// Static singleton property
	public static SaveManager Instance {
		get {
			if (instance) {
				return instance;
			} else {
				instance = new GameObject("SaveManager").AddComponent<SaveManager>();
				instance.Init ();
				return instance;
			}
		}
	}

	void Init() {
		DontDestroyOnLoad(instance.gameObject);
		level = 0;
		Load ();
	}

	public void UnlockLevel(int l) {
		level = l;
		Save ();
	}

	public void Save() {
		PlayerPrefs.SetInt ("CurrentLevel", level);
	}

	public void Load() {
		if (PlayerPrefs.HasKey ("CurrentLevel")) {
			level = PlayerPrefs.GetInt ("CurrentLevel");
		}
	}
}
