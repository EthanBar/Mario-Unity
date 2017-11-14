using UnityEditorInternal;
using UnityEngine;
using Time = UnityEngine.Time;

public class RectCollider : MonoBehaviour {
	public float width = 1;
	public float height = 1;
	public BlockType blockType = BlockType.solid;
	public Pickup pickup = Pickup.coin;
	
	private static CollisionInfo noCollision;

	private bool registered;
	private int delay;
	
	private void Awake() {
		delay = Random.Range(0, 30);
		Actor.RegisterCollider(this);
		registered = true;
		noCollision = new CollisionInfo(false, false, false, false, this);
	}

	public void FixedUpdate() {
		if (bouncing) Bounce();
		if (UnityEngine.Time.frameCount + delay % 30 != 0) return;
		if (Mathf.Abs(Mario.mario.transform.position.x - transform.position.x) > 15) {
			if (registered) {
				Actor.DeleteCollider(this);
				registered = false;
			}
		} else {
			if (!registered) {
				Actor.RegisterCollider(this);
				registered = true;
			}
		}
	}

	private bool bouncing;
	private int bounceFrame;

	public CollisionInfo Collide(Vector2 dimensions, Vector2 currentPosition, Vector2 newPosition, float shorten) {
		Vector2 myPosition = transform.position;

		float widthCol = width / 2 + dimensions.x / 2 - shorten;
		float heightCol = height / 2 + dimensions.y / 2 - shorten;
		
		bool withinX = myPosition.x <= currentPosition.x + widthCol && myPosition.x >= currentPosition.x - widthCol;
		bool withinY = myPosition.y <= currentPosition.y + heightCol && myPosition.y >= currentPosition.y - heightCol;
		if (!withinX && !withinY) return noCollision;

		bool[] collisions = new bool[4];
//		if (Mathf.Abs(dx) <= widthCol && Mathf.Abs(dy) <= heightCol)
//		{
//			/* collision! */
//			float wy = widthCol * dy;
//			float hx = heightCol * dx;
//
//			if (wy > hx) {
//				if (wy > -hx) {
//					collisions[3] = true;
//				} else {
//					collisions[1] = true;
//				}
//			} else {
//				if (wy > -hx) {
//					collisions[0] = true;
//				} else {
//					collisions[2] = true;
//				}
//			}
//		}
		if (withinX && currentPosition.y >= myPosition.y + heightCol) {
			if (newPosition.y <= myPosition.y + heightCol) {
				collisions[1] = true;
			}
		} else if (withinX && currentPosition.y <= myPosition.y - heightCol) {
			if (newPosition.y >= myPosition.y - heightCol) {
				collisions[3] = true;
			}
		}
		if (withinY && currentPosition.x >= myPosition.x + widthCol) {
			if (newPosition.x <= myPosition.x + widthCol) {
				collisions[0] = true;
			}
		} else if (withinY && currentPosition.x <= myPosition.x - widthCol) {
			if (newPosition.x >= myPosition.x - widthCol) {
				collisions[2] = true;
			}
		}
		return new CollisionInfo(collisions[3], collisions[1], collisions[0], collisions[2], this);
	}

	private Vector2 originalposition;

	private static readonly float[] yoffsets = {
		0.04f, 0.08f, 0.15f, 0.23f, 0.3f, 0.4f, 0.45f, 0.48f, 0.5f
	};

	private void Bounce() {
		if (bounceFrame < yoffsets.Length) {
			transform.position = new Vector2(originalposition.x, originalposition.y + yoffsets[bounceFrame]);
		} else if (bounceFrame == yoffsets.Length * 2 - 2) {
			transform.position = originalposition;
			bouncing = false;
		} else {
			transform.position = new Vector2(originalposition.x, originalposition.y + yoffsets[
				                                                     yoffsets.Length - (bounceFrame - yoffsets.Length) - 1]);
		}
		bounceFrame++;
	}
	
	public void StartBounce() {
		AudioManager.PlaySound(AudioManager.main.bump, 1);
		bouncing = true;
		bounceFrame = 0;
		originalposition = transform.position;
		Transform enemies = GameObject.Find("Enemies").transform;
		for (int i = 0; i < enemies.childCount; i++) {
			Transform enemie = enemies.GetChild(i);
			if (enemie.position.y > transform.position.y && enemie.position.y < transform.position.y + 1.5f &&
			    enemie.position.x > transform.position.x - 1 && enemie.position.x < transform.position.x + 1) {
				Goomba goomba = enemie.GetComponent<Goomba>();
				if (goomba != null) {
					goomba.Kill();
				} else Destroy(enemie.gameObject);
			}
		}
	}
	
	public Vector2 GetPosition() {
		return transform.position;
	}

	private void OnDestroy() {
		Actor.DeleteCollider(this);
	}
}

public struct CollisionInfo {
	public CollisionInfo(bool hitBottom, bool hitTop, bool hitRight, bool hitLeft, RectCollider obj) {
		this.hitBottom = hitBottom;
		this.hitTop = hitTop;
		this.hitRight = hitRight;
		this.hitLeft = hitLeft;
		this.obj = obj;
	}
	public readonly bool hitBottom, hitTop, hitRight, hitLeft;
	public readonly RectCollider obj;
}

[System.Serializable]
public enum BlockType {
	solid,
	breakable,
	coinblock,
	goomba,
	flag
}

[System.Serializable]
public enum Pickup {
	coin
}