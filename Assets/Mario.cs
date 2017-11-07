using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Timeline;

public class Mario : MonoBehaviour {
	
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	private float xvel, yvel;
	private JumpState jump;
	private bool fastAirStraff;
	private bool grounded;

	private static List<RectCollider> colliders;

	private const float conversion = 65536; // == 0x10000
	private const float maxX = 10496 / conversion;
	private const float walkAcc = 152 / conversion;
	private const float runAcc = 228 / conversion;
	
	private const float skidPower = 416 / conversion;
	private const float releaseDeAcc = 208 / conversion;

	private const float fastJumpPower = 20480 / conversion;
	private const float jumpPower = 16384 / conversion;

	private const float fastJumpReq = 9472 / conversion;
	private const float modJumpReq = 4096 / conversion;

	private const float fastJumpDecay = 1792 / conversion;
	private const float fastJumpDecayUp = 512 / conversion;
	private const float modJumpDecay = 1536 / conversion;
	private const float modJumpDecayUp = 480 / conversion;
	private const float slowJumpDecay = 2304 / conversion;
	private const float slowJumpDecayUp = 640 / conversion;

	private const float airStrafeBorder = 6400 / conversion;
	private const float airStrafeFast = 7424 / conversion; 
	
	// Use this for initialization
	void Start () {
		xvel = 0;
		yvel = 0;
		jump = JumpState.SlowJump;
		grounded = true;
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void FixedUpdate() {
		// Vertical input
		if (Input.GetKeyDown(KeyCode.Space) && grounded) {
			grounded = false;
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
			fastAirStraff = Mathf.Abs(xvel) > airStrafeFast;
		}
		
		// Horiztonal input
		bool moving = false;
		bool skidding = false;
		if (Input.GetKey(KeyCode.D)) {
			if (!grounded) {
				if (xvel >= 0) {
					if (xvel >= airStrafeBorder) {
						xvel += runAcc;
					} else {
						xvel += walkAcc;
					}
				} else if (xvel < 0) {
					// Slowing mid air
					if (-xvel >= airStrafeBorder) {
						xvel += runAcc;
					} else {
						if (fastAirStraff) {
							xvel += releaseDeAcc;
						}
						xvel += walkAcc;
					}
				}
			} else {
				moving = true;
				if (xvel >= 0) {
					xvel += walkAcc;
				} else if (xvel < 0) {
					// Skidding
					xvel += skidPower;
					skidding = true;
				}
			}
		}
		if (Input.GetKey(KeyCode.A)) {
			if (!grounded) {
				if (xvel <= 0) {
					if (-xvel >= airStrafeBorder) {
						xvel -= runAcc;
					} else {
						xvel -= walkAcc;
					}
				} else if (xvel > 0) {
					// Slowing mid air
					if (xvel >= airStrafeBorder) {
						xvel -= runAcc;
					} else {
						if (fastAirStraff) {
							xvel -= releaseDeAcc;
						}
						xvel -= walkAcc;
					}
				}
			} else {
				moving = true;
				if (xvel <= 0) {
					xvel -= walkAcc;
				} else if (xvel > 0) {
					// Skidding
					xvel -= skidPower;
					skidding = true;
				}
			}
		}
		
		// X velocity decay
		if (!moving && grounded) {
			if (xvel > 0) {
				xvel -= releaseDeAcc;
				if (xvel < 0) xvel = 0;
			} else {
				xvel += releaseDeAcc;
				if (xvel > 0) xvel = 0;
			}
		}
		
		// X velocity cap
		if (xvel > maxX) {
			xvel = maxX;
		} else if (xvel < -maxX) {
			xvel = -maxX;
		}
		
		// Y velocity decay
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
		
		
		// Sprite Flip
		if (xvel > 0) {
			spriteRenderer.flipX = skidding;
		} else if (xvel < 0) {
			spriteRenderer.flipX = !skidding;
		}
		
		// Move according to velocities
		Move(new Vector2(xvel, yvel));
		
		// Set animator variables
		animator.SetFloat("xvel", Mathf.Abs(xvel));
		animator.SetBool("skidding", skidding);
		animator.SetBool("jumping", !grounded);
	}

	// Collision detection
	private void Move(Vector2 move) {
		Vector2 curPos = transform.position;
		Vector2 attemptPos = curPos + move;
		
		
		foreach (RectCollider collider in colliders) {
			CollisionInfo collision = collider.Collide(new Vector2(1, 2), curPos, attemptPos);
//			print(collisions[1]);
			if (collision.hitTop) {
				transform.position = new Vector2(transform.position.x, collision.position.y + 1 + collision.height / 2);
				move.y = 0;
				yvel = 0;
				grounded = true;
			} else if (collision.hitBottom) {
				transform.position = new Vector2(transform.position.x, collision.position.y - 1 - collision.height / 2);
				move.y = 0;
				yvel = 0;
			}
			if (collision.hitRight) {
				transform.position = new Vector2(collision.position.x + 0.5f + collision.width / 2, transform.position.y);
				move.x = 0;
				xvel = 0;
			} else if (collision.hitLeft) {
				transform.position = new Vector2(collision.position.x - 0.5f - collision.width / 2, transform.position.y);
				move.x = 0;
				xvel = 0;
			}
		}

		transform.position += new Vector3(move.x, move.y, 0);
	}

	public static void AddCollider(RectCollider collider) {
		if (colliders == null) colliders = new List<RectCollider>();
		colliders.Add(collider);
	}
}

enum JumpState {
	SlowJump,
	ModerateJump,
	FastJump
}