using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerController : MonoBehaviour {

	// configurables
	public LevelSelector levelSelector;
	public float speed = 7f;
	public float acceleration = 0.75f;
	public float jump = 10f;
	public float inputBuffer = 0.05f;
	public bool canDoubleJump = true;
	public bool mirrorWhenTurning = true;

	// physics
	private Rigidbody2D body;
	public Transform groundCheck;
	public LayerMask groundLayer;
	public float groundCheckRadius = 0.2f;
	public float wallCheckDistance = 1f;
	public bool checkForEdges = false;
	private float groundAngle = 0;

	// flags
	private bool canControl = true;
	private bool running = false;
	private bool grounded = false;
	private bool doubleJumped = false;

	// misc
	private float jumpBufferedFor = 0;
	private DirectionalGravity dirGrav;
	private Vector3 spawn;

	// particles
	public GameObject jumpParticles, landParticles;

	// sound stuff
	private AudioSource audioSource;
	public AudioClip jumpClip, landClip;

	// animations
	private Animator anim;

	// ###############################################################

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		audioSource = GetComponent<AudioSource> ();
		anim = GetComponentInChildren<Animator> ();
		dirGrav = GetComponent<DirectionalGravity> ();

		spawn = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		bool wasGrounded = grounded;

		// just landed
		if (!wasGrounded && grounded) {
			Land ();
		}

		// just left the ground
		if (wasGrounded && !grounded) {
			groundAngle = 0;
		}

		// jump buffer timing
		if (jumpBufferedFor > 0) {
			jumpBufferedFor -= Time.deltaTime;
		}

		// controls
		if (canControl) {

			float inputDirection = Input.GetAxis("Horizontal");

			bool jumpCheck = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, groundLayer);;


			// jump
			if ((jumpCheck || (canDoubleJump && !doubleJumped)) && (Input.GetButtonDown("Jump") || jumpBufferedFor > 0)) {

				if (!jumpCheck) {
					doubleJumped = true;
				}

				jumpBufferedFor = 0;

				// jump sounds
				if (audioSource && jumpClip) {
					audioSource.PlayOneShot (jumpClip);
				}

				// jump particles
				if (jumpParticles) {
					Instantiate (jumpParticles, groundCheck.position, Quaternion.identity);
				}

				// animation
				if (anim) {
					anim.speed = 1f;
					anim.SetTrigger ("jump");
				}

				body.AddForce (dirGrav.MoveVector (inputDirection) + dirGrav.JumpVector() * jump, ForceMode2D.Impulse);

			} else if (canControl && Input.GetButtonDown("Jump")) {
			
				// jump command buffering
				jumpBufferedFor = 0.2f;
			}

			// moving
			if (grounded && Mathf.Abs(inputDirection) > 0.1f) {
				Vector2 move = dirGrav.MoveVector (inputDirection) + dirGrav.JumpVector() * 2;

//				if (Input.GetButton ("Jump")) {
//					move *= jump * 0.1f;
//				}

//				float speedMod = (grounded) ? 1f : 0.5f;

//				if (Mathf.Abs (move.x) > Mathf.Abs (move.y)) {
//					body.velocity = new Vector2 (speedMod * speed * move.x, body.velocity.y);
//				} else {
//					body.velocity = new Vector2 (body.velocity.x, speedMod * speed * move.y);
//				}

				body.AddForce (move * speed, ForceMode2D.Impulse);

//				if (body.velocity.magnitude > 10) {
//					body.velocity = body.velocity.normalized * 10;
//				}

			}

			if (Mathf.Abs(inputDirection) < 0.1f) {
			}

			// direction
			if (mirrorWhenTurning && Mathf.Abs(inputDirection) > inputBuffer) {

				float dir = Mathf.Sign (inputDirection);
				transform.localScale = new Vector2 (dir, 1);

//				Transform sprite = transform.Find("Character");
//				Vector3 scl = sprite.localScale;
//				scl.x = dir;
//				sprite.localScale = scl;

//				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 90f - dir * 90f, transform.localEulerAngles.z);
			}

			Vector2 p = transform.position + Vector3.right * inputDirection * wallCheckDistance;
			bool wallHug = Physics2D.OverlapCircle (p, groundCheckRadius, groundLayer);
			Color hugLineColor = grounded ? Color.green : Color.red;
			Debug.DrawLine (transform.position, p, hugLineColor, 0.2f);

			if (wallHug && !checkForEdges) {
				body.velocity = new Vector2 (0, body.velocity.y);
			}

			running = inputDirection < -inputBuffer || inputDirection > inputBuffer;

			if (!grounded) {
				running = false; 
			}

			if (anim) {
				if (running) {
					anim.speed = Mathf.Abs (body.velocity.x * 0.2f);
					anim.SetFloat ("speed", Mathf.Abs(body.velocity.x));
				} else {
					anim.speed = 1f;
					anim.SetFloat ("speed", 0);
				}
			}
		}
	}

	private void Land() {

		doubleJumped = false;

		// landing sound
		if (audioSource && landClip) {
			audioSource.PlayOneShot (landClip);
		}

		// landing particles
		if (landParticles) {
			Instantiate (landParticles, groundCheck.position, Quaternion.identity);
		}

		// animation
		if (anim) {
			anim.speed = 1f;
			anim.SetTrigger ("land");
		}
	}

	public bool IsGrounded() {
		return grounded;
	}

	void OnCollisionStay2D(Collision2D coll) {
		groundAngle = Mathf.Atan2(coll.contacts [0].normal.y, coll.contacts [0].normal.x) * Mathf.Rad2Deg - 90;
	}

	void OnCollisionEnter2D(Collision2D coll) {
		grounded = true;
		groundAngle = Mathf.Atan2(coll.contacts [0].normal.y, coll.contacts [0].normal.x) * Mathf.Rad2Deg - 90;
	}

	void OnCollisionExit2D(Collision2D coll) {
		grounded = false;
	}

	public float GetGroundAngle() {
		if (Mathf.Abs (groundAngle) > 90) {
			groundAngle = 0;
		}
		return groundAngle;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Goal") {
			body.velocity = Vector2.zero;
			transform.position = spawn;
			levelSelector.NextLevel ();
		}
	}
}
