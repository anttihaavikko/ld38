using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalGravity : MonoBehaviour {

	public LevelSelector levelSelector;

	Vector2 gravity = Vector2.down;
	Vector2 gravityRounded = Vector2.down;
	Rigidbody2D body;

	Vector3 origin = Vector3.zero;

	float changeDelay = 0f;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		SolveGravity ();
		float angle = Mathf.Atan2(gravityRounded.y, gravityRounded.x) * Mathf.Rad2Deg + 90;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler (new Vector3 (0, 0, angle)), 20f);

		body.AddForce (gravityRounded * 30f, ForceMode2D.Force);
	}

	void SolveGravity() {

		if (changeDelay > 0) {
			changeDelay -= Time.deltaTime;
			return;
		}

		origin = levelSelector.ClosestPlanet (transform.position);

		gravity = -transform.position.normalized;

		Vector3 pos = origin - transform.position;
		
		if (pos.x < 0 && Mathf.Abs(pos.x) > Mathf.Abs(pos.y)) {
			ChangeRoundedGravity (Vector2.left);
			return;
		}

		if (pos.x > 0 && Mathf.Abs(pos.x) > Mathf.Abs(pos.y)) {
			ChangeRoundedGravity (Vector2.right);
			return;
		}

		if (pos.y < 0) {
			ChangeRoundedGravity (Vector2.down);
			return;
		}

		ChangeRoundedGravity (Vector2.up);
	}

	void ChangeRoundedGravity(Vector2 dir) {
		
		if (gravityRounded != dir) {
			changeDelay = 0.2f;
		}

		gravityRounded = dir;
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
