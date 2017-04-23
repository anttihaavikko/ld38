using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBunny : MonoBehaviour {

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		Invoke ("DoGesture", Random.Range(2f, 6f));
	}

	void DoGesture() {

		int gesture = Random.Range (0, 3);

		if (gesture == 0) {
			anim.SetBool ("duck", true);
			Invoke ("UnDuck", 3f);
		}

		if (gesture == 1) {
			anim.SetBool ("stretch", true);
			Invoke ("UnStretch", 1f);
		}

		if (gesture == 2) {
			anim.SetTrigger ("wiggle");
		}

		Invoke ("DoGesture", Random.Range(4f, 10f));
	}

	void UnDuck() {
		anim.SetBool ("duck", false);
	}

	void UnStretch() {
		anim.SetBool ("stretch", false);
	}

	public void DelayedPoop() {
		// not here
	}
}
