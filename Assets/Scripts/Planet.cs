using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

	public float Mass() {
		return Mathf.Sqrt(transform.localScale.x * transform.localScale.y);
	}
}
