using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Mario : MonoBehaviour {
	
	private Rigidbody2D rb2d;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	private float xvel, yvel;
	private JumpState jump;

	private const float conversion = 65536; // == 0x10000
	private const float maxX = 10496 / conversion;
	private const float runAcc = 152 / conversion;
	private const float skidPower = 416 / conversion;
	private const float releaseDeAcc = 208 / conversion;

	private const float fastJumpPower = 20480 / conversion;
	private const float jumpPower = 16384 / conversion;

	private const float fastJumpReq = 9472 / conversion;
	private const float modJumpReq = 9472 / conversion;

	private const float fastJumpDecay = 1792 / conversion;
	private const float fastJumpDecayUp = 512 / conversion;
	private const float modJumpDecay = 1536 / conversion;
	private const float modJumpDecayUp = 480 / conversion;
	private const float slowJumpDecay = 2304 / conversion;
	private const float slowJumpDecayUp = 640 / conversion;

	// Use this for initialization
	void Start () {
		xvel = 0;
		yvel = 0;
		jump = JumpState.AtRest;
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private int invs = 0;
	
	// Update is called once per frame
	void FixedUpdate() {
		if (Input.GetKeyDown(KeyCode.Space) && jump == JumpState.AtRest) {
			invs = 3;
			if (Mathf.Abs(xvel) > fastJumpReq) {
				jump = JumpState.FastJump;
				yvel = fastJumpPower;
			} else if (Mathf.Abs(xvel) > modJumpReq) {
				jump = JumpState.ModerateJump;
				yvel = jumpPower;
			} else {
				jump = JumpState.SlowJump;
				yvel = jumpPower;
			}
		}

		bool moving = false;
		bool skidding = false;
		if (Input.GetKey(KeyCode.D)) {
			moving = true;
			if (xvel >= 0) {
				xvel += runAcc;
			} else if (xvel < 0) { // Skidding
				xvel += skidPower;
				skidding = true;
			}
		}
		if (Input.GetKey(KeyCode.A)) {
			moving = true;
			if (xvel <= 0) {
				xvel -= runAcc;
			} else if (xvel > 0) { // Skidding
				xvel -= skidPower;
				skidding = true;
			}
		}
		if (!moving) {
			if (xvel > 0) {
				xvel -= releaseDeAcc;
				if (xvel < 0) xvel = 0;
			} else {
				xvel += releaseDeAcc;
				if (xvel > 0) xvel = 0;
			}
		}
		
		// X decay
		if (xvel > maxX) {
			xvel = maxX;
		} else if (xvel < -maxX) {
			xvel = -maxX;
		}
		
		// Y decay
		if (jump != JumpState.AtRest) {
			if (Input.GetKey(KeyCode.Space)) {
				switch (jump) {
					case JumpState.FastJump:
						yvel -= fastJumpDecayUp;
						break;
					case JumpState.ModerateJump:
						yvel -= modJumpDecayUp;
						break;
					case JumpState.SlowJump:
						yvel -= slowJumpDecayUp;
						break;
				}
			} else {
				switch (jump) {
					case JumpState.FastJump:
						yvel -= fastJumpDecay;
						break;
					case JumpState.ModerateJump:
						yvel -= modJumpDecay;
						break;
					case JumpState.SlowJump:
						yvel -= slowJumpDecay;
						break;
				}
			}
		}
//		print(yvel);
		invs -= 1;
		// Sprite Flip
		if (xvel > 0) {
			spriteRenderer.flipX = false;
		} else if (xvel < 0) {
			spriteRenderer.flipX = true;
		}
		animator.SetFloat("xvel", Mathf.Abs(xvel));
		animator.SetBool("skidding", skidding);
		rb2d.MovePosition(new Vector2(transform.position.x, transform.position.y) + new Vector2(xvel, yvel));
	}

	void OnCollisionStay2D(Collision2D other) {
		Vector2 normal = other.contacts[0].normal;
//		print(normal);
		if (Mathf.Abs(normal.x) > 0.1) {
			xvel = 0;
		}
		if (normal.y > 0.01 && invs <= 0) {
			yvel = 0;
			jump = JumpState.AtRest;
		} else {
			print(normal);
			print("AAAA");
		}
	}
}

enum JumpState {
	AtRest,
	SlowJump,
	ModerateJump,
	FastJump
}