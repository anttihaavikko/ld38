using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalGravity : MonoBehaviour {

	Vector2 gravity = Vector2.down;
	Vector2 gravityRounded = Vector2.down;
	Rigidbody2D body;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		SolveGravity ();
		float angle = Mathf.Atan2(gravityRounded.y, gravityRounded.x) * Mathf.Rad2Deg + 90;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler (new Vector3 (0, 0, angle)), 20f);

		Vector2 usedGravity = (transform.position.magnitude > 3f) ? gravity : gravityRounded;

		body.AddForce (gravityRounded * 10f * transform.position.magnitude, ForceMode2D.Force);
	}

	void SolveGravity() {

		gravity = -transform.position.normalized;
		
		if (transform.position.x < 0 && Mathf.Abs(transform.position.x) > Mathf.Abs(transform.position.y)) {
			gravityRounded = Vector2.right;
			return;
		}

		if (transform.position.x > 0 && Mathf.Abs(transform.position.x) > Mathf.Abs(transform.position.y)) {
			gravityRounded = Vector2.left;
			return;
		}

		if (transform.position.y < 0) {
			gravityRounded = Vector2.up;
			return;
		}

		gravityRounded = Vector2.down;
	}

	public Vector2 MoveVector(float direction) {

		Vector2 vec = Vector2.zero;

		if (gravityRounded == Vector2.down) {
			vec = new Vector2 (direction, 0);
		}

		if (gravityRounded == Vector2.up) {
			vec = new Vector2 (-direction, 0);
		}

		if (gravityRounded == Vector2.left) {
			vec = new Vector2 (0, -direction);
		}

		if (gravityRounded == Vector2.right) {
			vec = new Vector2 (0, direction);
		}

		return vec.normalized;
	}

	public Vector2 JumpVector() {
		return -gravityRounded;
	}
}
