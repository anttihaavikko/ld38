using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	private float airTime = 0f;

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
	public GameObject poopPrefab;
	public Transform poopPos;
	private bool pooping = false;

	// particles
	public GameObject jumpParticles, landParticles;

	// sound stuff
	private AudioSource audioSource;
	public AudioClip jumpClip, landClip, eatClip, keyClip, cageClip, hopClip, poopClip;

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

		if (!grounded) {
			airTime += Time.deltaTime;
		} else {
			airTime = 0f;
		}

		if (airTime > 5f) {
			HudManager.Instance.DisplayMessage ("PRESS  R  TO  RESTART", 3f);
			airTime = 0;
		}

		if (Input.GetKeyDown (KeyCode.R)) {
			Instantiate (jumpParticles, transform.position, Quaternion.identity);
			ResetPosition ();
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("Start");
		}

		if (Input.GetAxis ("Vertical") < -0.1f && Mathf.Abs (Input.GetAxis ("Horizontal")) < 0.1f) {
			anim.SetBool ("duck", true);
		} else {
			pooping = false;
			anim.SetBool ("duck", false);
		}

		if (Input.GetAxis ("Vertical") > 0.1f && Mathf.Abs (Input.GetAxis ("Horizontal")) < 0.1f) {
			anim.SetBool ("stretch", true);
		} else {
			anim.SetBool ("stretch", false);
		}

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
					PlayClip (jumpClip, 0.5f);
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
					PlayClip (hopClip, 0.1f);
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

				if (Random.value < 0.005f) {
					int move = Random.Range (0, 3);

					if (move == 0) {
						anim.SetTrigger ("brow");
					}

					if (move == 1) {
						anim.SetTrigger ("brow_left");
					}

					if (move == 2) {
						anim.SetTrigger ("brow_right");
					}
				}
			}
		}
	}

	public void DelayedPoop() {
		pooping = true;
		Invoke ("Poop", Random.Range (0.5f, 2.5f));
	}

	public void Poop() {

		if (pooping) {

			PlayClip (poopClip, 1f);

			GameObject poop = Instantiate (poopPrefab, poopPos.position, Quaternion.identity);
			poop.GetComponent<DirectionalGravity> ().levelSelector = dirGrav.levelSelector;

			Rigidbody2D poopBody = poop.GetComponent<Rigidbody2D> ();
			Vector2 dir = transform.rotation * Vector2.left;
			poopBody.AddForce (dir * 30f * transform.localScale.x, ForceMode2D.Impulse);

			body.AddForce (-dir * 2f * transform.localScale.x, ForceMode2D.Impulse);

			levelSelector.AddPoop (poop);

			DelayedPoop ();
		}
	}

	private void Land(float force) {

		doubleJumped = false;

		// landing sound
		if (audioSource && landClip) {
			PlayClip (landClip, force / 40f);
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
			Land (coll.relativeVelocity.magnitude);
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
			PlayClip (keyClip, 1f);
			EffectManager.Instance.AddEffectAt (0, other.transform.position);
			key = true;
			Destroy (other.gameObject);
		}

		if (other.gameObject.tag == "Goal") {

			Goal g = other.GetComponent<Goal> ();

			if (!g.cage || key) {

				if (g.cage) {
					PlayClip (cageClip, 1f);
				}

				PlayClip (eatClip, 1f);
				EffectManager.Instance.AddEffectAt (0, other.transform.position);
				Destroy (other.gameObject);
				mouth.SetActive (true);
				Invoke ("NextLevel", 2f);
			}
		}
	}

	void NextLevel() {
		ResetPosition ();
		levelSelector.NextLevel ();
	}

	void PlayClip(AudioClip clip, float volume) {
		audioSource.pitch = 1f + Random.Range (-0.1f, 0.1f);
		audioSource.PlayOneShot (clip, volume);
	}

	void ResetPosition() {
		mouth.SetActive (false);
		body.velocity = Vector2.zero;
		transform.position = spawn;
	}
}
