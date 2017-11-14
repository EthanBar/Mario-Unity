using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mario : MonoBehaviour {
	
	private SpriteRenderer spriteRenderer;
	private Animator animator;
	private Score score;
	private Coins coins;
	public static Mario mario;

	public Vector2 dimensions;
	private float xvel, yvel;
	private JumpState jump;
	private bool fastAirStraff;
	private bool grounded, jumping;

	public GameObject flag;
	public GameObject breakTile;
	public GameObject gameover;
	public GameObject floatText;

	
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

	private bool poweredUp;
	// Use this for initialization
	void Start () {
		poweredUp = true;
		animator = GetComponent<Animator>();
		mario = this;
		xvel = 0;
		yvel = 0;
		jump = JumpState.SlowJump;
		jumping = false;
		grounded = true;
		spriteRenderer = GetComponent<SpriteRenderer>();
		score = GameObject.Find("Score").GetComponent<Score>();
		coins = GameObject.Find("Coins").GetComponent<Coins>();
	}

	private bool keySpace, keyD, keyA, keyShift, keySpaceDown; 

	void Update() {
		keySpaceDown = Input.GetKeyDown(KeyCode.Space);
		keySpace = Input.GetKey(KeyCode.Space);
		keyD = Input.GetKey(KeyCode.D);
		keyA = Input.GetKey(KeyCode.A);
		keyShift = Input.GetKey(KeyCode.LeftShift);
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		if (onFlag) {
			FlagPole();
			return;
		}
//		if (Input.GetKeyDown(KeyCode.Space)) print(grounded);
		// Vertical input
		if (keySpaceDown	 && grounded) {
			jumping = true;
			if (Mathf.Abs(xvel) > fastJumpReq) {
				AudioManager.PlaySound(AudioManager.main.megaJump, 1);
				jump = JumpState.FastJump;
				yvel = fastJumpPower;
			} else if (Mathf.Abs(xvel) > modJumpReq) {
				AudioManager.PlaySound(AudioManager.main.jump, 1);
				jump = JumpState.ModerateJump;
				yvel = jumpPower;
			} else {
				AudioManager.PlaySound(AudioManager.main.jump, 1);
				jump = JumpState.SlowJump;
				yvel = jumpPower;
			}
			fastAirStraff = Mathf.Abs(xvel) > airStrafeFast;
		}
		
		// Horiztonal input
		bool moving = false;
		bool skidding = false;
		if (keyD) {
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
					if (keyShift) {
						xvel += runAcc;
					} else xvel += walkAcc;
				} else if (xvel < 0) {
					// Skidding
					xvel += skidPower;
					skidding = true;
				}
			}
		}
		if (keyA) {
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
					if (keyShift) {
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
		float maxSpeed = keyShift ? maxRunX : maxWalkX;
		if (xvel > maxSpeed) {
			xvel = maxSpeed;
		} else if (xvel < -maxSpeed) {
			xvel = -maxSpeed;
		}
		
		// Y velocity decay
		if (keySpace) {
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
		grounded = false;
		// Move according to velocities
		Move(new Vector2(xvel, yvel));
		if (jumping) jumping = !grounded;

		if (transform.position.y < -8) {
			StartCoroutine(GetHurt(true));
		}
		
		// Set animator variables
		animator.SetFloat("xvel", Mathf.Abs(xvel));
		animator.SetBool("skidding", skidding);
		animator.SetBool("jumping", jumping);
	}

	// Collision detection
	private void Move(Vector2 move) {
		Vector2 curPos = transform.position;
		Vector2 attemptPos = curPos + move;
		CollisionInfo[] collisions = Actor.Collide(curPos, attemptPos, dimensions, 0);
		if (collisions.Length > 0) move = HandleCollisions(move, collisions);

		transform.position += new Vector3(move.x, move.y, 0);
	}

	Vector2 HandleCollisions(Vector2 move, CollisionInfo[] collisions) {
		bool hitTop = false;
		CollisionInfo closestTopBlock = collisions[0];
		grounded = false;

		foreach (CollisionInfo collision in collisions) {
			if (collision.obj.blockType == BlockType.flag) {
				FlagPole();
				return move;
			}
			if (collision.hitTop) {
				move.y = 0;
				yvel = 0;
				grounded = true;
				if (collision.obj.blockType == BlockType.goomba) {
					AudioManager.PlaySound(AudioManager.main.goomba, 1);
					collision.obj.GetComponent<Goomba>().Kill();
					yvel = goombaJump;
					move.y = goombaJump;
					grounded = false;
					AddPoints(100, true, collision.obj.transform.position);
				}
			}
			if (collision.hitBottom) {
				if (collision.obj.blockType == BlockType.goomba) StartCoroutine(GetHurt(false));
				if (!hitTop) {
					closestTopBlock = collision;
					hitTop = true;
				} else {
					if (Mathf.Abs(transform.position.x - collision.obj.GetPosition().x) <
					    Mathf.Abs(transform.position.x - closestTopBlock.obj.GetPosition().x)) {
						closestTopBlock = collision;
					}
				}
				move.y = 0;
				yvel = 0;
			}
			if (collision.hitRight) {
				if (collision.obj.blockType == BlockType.goomba) StartCoroutine(GetHurt(false));
				move.x = 0;
				xvel = 0;
			} 
			if (collision.hitLeft) {
				if (collision.obj.blockType == BlockType.goomba) StartCoroutine(GetHurt(false));
				move.x = 0;
				xvel = 0;
			}
		}
		if (hitTop) HitTopBlock(closestTopBlock);
		return move;
	}
	
	private bool onFlag;
	private int flagFrame;
	private float topPole;

	void FlagPole() {
		onFlag = true;
		flag.GetComponent<EndLevel>().HitPole(animator, this, spriteRenderer, poweredUp);
	}

	
	private IEnumerator GetHurt(bool kill) {
		float waitTime = 0.2f;
		if (!poweredUp || kill) {
			UnityEngine.Time.timeScale = 0;
			Camera.main.GetComponent<AudioSource>().Stop();
			AudioManager.PlaySound(AudioManager.main.death, 1);
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Death"), 1);
			yield return new WaitForSecondsRealtime(1f);
			gameover.SetActive(true);
			yield return new WaitForSecondsRealtime(4f);
			UnityEngine.Time.timeScale = 1;
			SceneManager.LoadScene("1-1");
		} else {
			dimensions = new Vector2(1, 1);
			UnityEngine.Time.timeScale = 0;
			poweredUp = false;
			AudioManager.PlaySound(AudioManager.main.pipe, 1);
			Vector2 oldPosition = transform.position;
			Vector2 newPosition = transform.position;
			newPosition.y -= 0.4f;
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 1);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 0);
			transform.position = newPosition;
			yield return new WaitForSecondsRealtime(waitTime);
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 1);
			transform.position = oldPosition;
			yield return new WaitForSecondsRealtime(waitTime);
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 1);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 0);
			transform.position = newPosition;
			yield return new WaitForSecondsRealtime(waitTime);
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 1);
			transform.position = oldPosition;
			yield return new WaitForSecondsRealtime(waitTime);
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 1);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 0);
			transform.position = newPosition;
			UnityEngine.Time.timeScale = 1;
		}
	}

	void HitTopBlock(CollisionInfo collision) {
		if (collision.obj.blockType == BlockType.breakable) {
			if (poweredUp) {
				AudioManager.PlaySound(AudioManager.main.breakBlock, 1);
				BreakTile(collision.obj.transform.position);
				Destroy(collision.obj.gameObject);
			} else {
				collision.obj.StartBounce();
			}
		} else if (collision.obj.blockType == BlockType.coinblock) {
			Animator animator = collision.obj.gameObject.GetComponent<Animator>();
			if (!animator.GetBool("used")) {
				AudioManager.PlaySound(AudioManager.main.coin, 1);
				coins.AddCoins(1);
				AddPoints(200, true, collision.obj.transform.position);
				animator.SetBool("used", true);
				collision.obj.StartBounce();
			}
		} else {
			collision.obj.StartBounce();
		}
	}

	public static void AddPoints(int points, bool drawText, Vector2 position) {
		mario.score.AddScore(points);
		if (!drawText) return;
		Vector2 screenCoords = Camera.main.WorldToScreenPoint(new Vector2(position.x, position.y + 0.5f));
		screenCoords.x = (int) screenCoords.x;
		screenCoords.y = (int) screenCoords.y;
		Instantiate(mario.floatText, screenCoords, Quaternion.identity)
			.GetComponent<PointText>().SetPoints(points);
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