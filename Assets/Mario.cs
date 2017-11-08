using UnityEngine;

public class Mario : MonoBehaviour {
	
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private AudioSource audioSource;
	private Score score;

	public Vector2 dimensions;
	private float xvel, yvel;
	private JumpState jump;
	private bool fastAirStraff;
	private bool grounded;

	public GameObject breakTile;

	private const float conversion = 65536; // == 0x10000
	private const float maxRunX = 10496 / conversion;
	private const float maxWalkX = 6400 / conversion;
	private const float walkAcc = 152 / conversion;
	private const float runAcc = 228 / conversion;
	
	private const float skidPower = 416 / conversion;
	private const float releaseDeAcc = 208 / conversion;

	private const float fastJumpPower = 20480 / conversion;
	private const float jumpPower = 16384 / conversion;

	private const float fastJumpReq = 9472 / conversion;
	private const float modJumpReq = 4096 / conversion;

	private const float fastJumpDecay = 2304 / conversion;
	private const float fastJumpDecayUp = 640 / conversion;
	private const float modJumpDecay = 1536 / conversion;
	private const float modJumpDecayUp = 460 / conversion; // 480
	private const float slowJumpDecay = 1792 / conversion;
	private const float slowJumpDecayUp = 490 / conversion; //512

	private const float airStrafeBorder = 6400 / conversion;
	private const float airStrafeFast = 7424 / conversion; 
	
	private const float goombaJump = 18432 / conversion;
	// Use this for initialization
	void Start () {
		xvel = 0;
		yvel = 0;
		jump = JumpState.SlowJump;
		grounded = true;
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();
		score = GameObject.Find("Score").GetComponent<Score>();
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
			audioSource.Play();
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
					if (Input.GetKey(KeyCode.LeftShift)) {
						xvel += runAcc;
					} else xvel += walkAcc;
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
					if (Input.GetKey(KeyCode.LeftShift)) {
						xvel -= runAcc;
					} else xvel -= walkAcc;
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
		float maxSpeed = Input.GetKey(KeyCode.LeftShift) ? maxRunX : maxWalkX;
		if (xvel > maxSpeed) {
			xvel = maxSpeed;
		} else if (xvel < -maxSpeed) {
			xvel = -maxSpeed;
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

		CollisionInfo[] collisions = Actor.Collide(curPos, attemptPos, dimensions);
		
		foreach (CollisionInfo collision in collisions) {
			if (collision.hitTop) {
				transform.position = new Vector2(transform.position.x, collision.obj.GetPosition().y + dimensions.y / 2 + collision.obj.GetHeight() / 2);
				move.y = 0;
				yvel = 0;
				grounded = true;
				if (collision.obj.blockType == BlockType.goomba) {
					AudioManager.PlaySound(AudioManager.main.goomba);
					Destroy(collision.obj.gameObject);
					yvel = goombaJump;
					move.y = goombaJump;
					grounded = false;
					score.AddScore(100);
				}
			} else if (collision.hitBottom) {
				transform.position = new Vector2(transform.position.x, collision.obj.GetPosition().y - dimensions.y / 2 - collision.obj.GetHeight() / 2);
				move.y = 0;
				yvel = 0;
				if (collision.obj.blockType == BlockType.breakable) {
					AudioManager.PlaySound(AudioManager.main.breakBlock);
					BreakTile(collision.obj.transform.position);
					Destroy(collision.obj.gameObject);
				} else if (collision.obj.blockType == BlockType.coinblock) {
					Animator animator = collision.obj.gameObject.GetComponent<Animator>();
					if (!animator.GetBool("used")) {
						AudioManager.PlaySound(AudioManager.main.coin);
						score.AddScore(200);
					}
					animator.SetBool("used", true);
				}
			}
			if (collision.hitRight) {
				transform.position = new Vector2(collision.obj.GetPosition().x + dimensions.x / 2 + collision.obj.GetWidth() / 2, transform.position.y);
				move.x = 0;
				xvel = 0;
			} else if (collision.hitLeft) {
				transform.position = new Vector2(collision.obj.GetPosition().x - dimensions.x / 2 - collision.obj.GetWidth() / 2, transform.position.y);
				move.x = 0;
				xvel = 0;
			}
		}
		transform.position += new Vector3(move.x, move.y, 0);
	}

	void BreakTile(Vector2 position) {
		const float gravity = -0.01f;
		const float hor = 0.01f;
		Instantiate(breakTile, position, Quaternion.identity).GetComponent<TileBreak>().SetData(new Vector2(0, gravity),
			new Vector2(-hor, 0.3f));
		Instantiate(breakTile, position, Quaternion.identity).GetComponent<TileBreak>().SetData(new Vector2(0, gravity),
			new Vector2(-hor, 0.2f));
		Instantiate(breakTile, position, Quaternion.identity).GetComponent<TileBreak>().SetData(new Vector2(0, gravity),
			new Vector2(hor, 0.3f));
		Instantiate(breakTile, position, Quaternion.identity).GetComponent<TileBreak>().SetData(new Vector2(0, gravity),
			new Vector2(hor, 0.2f));
	}
}

internal enum JumpState {
	SlowJump,
	ModerateJump,
	FastJump
}