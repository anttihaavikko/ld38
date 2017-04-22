using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour {

	public ParticleSystem[] effects;

	// ==================

	private static EffectManager instance = null;

	public static EffectManager Instance {
		get { return instance; }
	}

	// ==================

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}
	}

	public ParticleSystem AddEffect(int effect, Transform t) {
		ParticleSystem e = Instantiate (effects[effect], t.position, Quaternion.identity);
		e.transform.parent = t;
		return e;
	}

	public ParticleSystem AddEffectAt(int effect, Vector3 position) {
		ParticleSystem e = Instantiate (effects[effect], position, Quaternion.identity);
		e.transform.position = position;
		return e;
	}
}
