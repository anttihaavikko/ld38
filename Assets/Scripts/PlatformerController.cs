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
	public GameObject mouth;
	public Transform shine;
	private bool key = false;

	// particles
	public GameObject jumpParticles, landParticles;

	// sound stuff
	private AudioSource audioSource;
	public AudioClip jumpClip, landClip;

	// animations
	private Animator anim;

	private SmoothCamera2D cam;

	// ###############################################################

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		audioSource = GetComponent<AudioSource> ();
		anim = GetComponentInChildren<Animator> ();
		dirGrav = GetComponent<DirectionalGravity> ();
		cam = Camera.main.GetComponent<SmoothCamera2D> ();

		spawn = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 shinePos = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.z) * (Vector3.up * 5f - transform.position).normalized * 0.05f;
		shinePos = new Vector2 (shinePos.x * transform.localScale.x, shinePos.x * transform.localScale.y);
		shine.localPosition = Vector2.MoveTowards(shine.localPosition, shinePos, 0.1f);

		bool wasGrounded = grounded;

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

			bool jumpCheck = Physics2D.OverlapCircle (groundCheck.position, groundCheckRadius, groundLayer);


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


				//bool wall = Physics2D.OverlapCircle (move, groundCheckRadius, groundLayer);

				// draw debug lines
				//Color debugLineColor = wall ? Color.red : Color.green;
				//Debug.DrawLine (transform.position, (Vector2)transform.position + move, debugLineColor, 0.2f);

				Vector2 p = transform.position + (Vector3)dirGrav.MoveVector (inputDirection) * 0.5f;
				bool wallHug = Physics2D.OverlapCircle (p, groundCheckRadius, groundLayer);
				Color hugLineColor = grounded ? Color.green : Color.red;
				Debug.DrawLine (transform.position, p, hugLineColor, 0.2f);

				if (wallHug && !checkForEdges) {
					body.velocity = new Vector2 (0, body.velocity.y);
				}

				if (!wallHug) {
					body.AddForce (move * speed, ForceMode2D.Impulse);
				}

			}

			if (Mathf.Abs(inputDirection) < 0.1f) {
			}

			// direction
			if (mirrorWhenTurning && Mathf.Abs(inputDirection) > inputBuffer) {

				float dir = Mathf.Sign (inputDirection);
				transform.localScale = new Vector2 (dir, 1);
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


				if (Random.value < 0.01f) {
					anim.SetTrigger ("wiggle");
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

		if (!grounded) {
			Land ();
		}
			
		if (coll.relativeVelocity.magnitude > 7f) {
			cam.Shake (coll.relativeVelocity.magnitude / 150f, coll.relativeVelocity.magnitude / 150f);
		}

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

		if (other.gameObject.tag == "Key") {
			EffectManager.Instance.AddEffectAt (0, other.transform.position);
			key = true;
			Destroy (other.gameObject);
		}

		if (other.gameObject.tag == "Goal") {

			Goal g = other.GetComponent<Goal> ();

			if (!g.cage || key) {
				EffectManager.Instance.AddEffectAt (0, other.transform.position);
				Destroy (other.gameObject);
				mouth.SetActive (true);
				Invoke ("NextLevel", 2f);
			}
		}
	}

	void NextLevel() {
		mouth.SetActive (false);
		body.velocity = Vector2.zero;
		transform.position = spawn;
		levelSelector.NextLevel ();
	}
}
