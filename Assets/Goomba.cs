using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba : MonoBehaviour {
	private float right;
	public Vector2 dimensions;
	private bool active;
	private bool dead;
	private float downAcc;
	
	void Start () {
	    right = -1;
		dead = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (dead) {
			Fall();
			return;
		}
		active = Mathf.Abs(transform.position.x - Mario.mario.transform.position.x) <= 14;
		if (!active) return;
		if (transform.position.x - Mario.mario.transform.position.x >= 15) Destroy(gameObject);
		float rightmove = 0.035f * right;
		Vector2 move = new Vector2(rightmove, downAcc);
		downAcc -= 0.0184f;
		CollisionInfo[] collisions = Actor.Collide(transform.position,
			new Vector2(transform.position.x, transform.position.y) + move, dimensions, 0.01f);
		foreach (CollisionInfo collision in collisions) {
			if (collision.hitTop) {
				transform.position = new Vector2(transform.position.x, collision.obj.GetPosition().y + dimensions.y / 2 + collision.obj.height / 2);
				move.y = 0;
				downAcc = 0;
			} else if (collision.hitBottom) {
				transform.position = new Vector2(transform.position.x, collision.obj.GetPosition().y - dimensions.y / 2 - collision.obj.height / 2);
				move.y = 0;
				downAcc = 0;
			}
			if (collision.hitRight) {
				print(collision.obj.name);
				transform.position = new Vector2(collision.obj.GetPosition().x + dimensions.x / 2 + collision.obj.width / 2, transform.position.y);
				move.x *= -1;
				right = 1;
			} else if (collision.hitLeft) {
				transform.position = new Vector2(collision.obj.GetPosition().x - dimensions.x / 2 - collision.obj.width / 2, transform.position.y);
				move.x *= -1;
				right = -1;
			}
		}
		transform.position = new Vector2(transform.position.x, transform.position.y) + move;
	}

	void Fall() {
		Vector2 position = transform.position;
		position.x += 0.035f;
		position.y += downAcc;
		transform.position = position;
		downAcc -= 0.01f;
		if (position.y < -8) {
			Destroy(gameObject);
		}
	}

	public void Kill() {
		dead = true;
		Destroy(GetComponent<RectCollider>());
		GetComponent<SpriteRenderer>().flipY = true;
		downAcc = 0.1f;
	}
}
