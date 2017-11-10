using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gomba : MonoBehaviour {
	private float right;
	public Vector2 dimensions;
	private bool active;
	
	private float downAcc;
	
	void Start () {
	    right = -1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		active = Mathf.Abs(transform.position.x - Mario.mario.transform.position.x) <= 9;
		if (!active) return;
		if (transform.position.x - Mario.mario.transform.position.x >= 9) Destroy(gameObject);
		float rightmove = 0.035f * right;
		Vector2 move = new Vector2(rightmove, downAcc);
		downAcc -= 0.0184f;
		CollisionInfo[] collisions = Actor.Collide(transform.position,
			new Vector2(transform.position.x, transform.position.y) + move, dimensions);
		foreach (CollisionInfo collision in collisions) {
			if (collision.hitTop) {
				transform.position = new Vector2(transform.position.x, collision.obj.GetPosition().y + dimensions.y / 2 + collision.obj.GetHeight() / 2);
				move.y = 0;
				downAcc = 0;
			} else if (collision.hitBottom) {
				transform.position = new Vector2(transform.position.x, collision.obj.GetPosition().y - dimensions.y / 2 - collision.obj.GetHeight() / 2);
				move.y = 0;
				downAcc = 0;
			}
			if (collision.hitRight) {
				transform.position = new Vector2(collision.obj.GetPosition().x + dimensions.x / 2 + collision.obj.GetWidth() / 2, transform.position.y);
				move.x *= -1;
				right = 1;
			} else if (collision.hitLeft) {
				transform.position = new Vector2(collision.obj.GetPosition().x - dimensions.x / 2 - collision.obj.GetWidth() / 2, transform.position.y);
				move.x *= -1;
				right = -1;
			}
		}
		transform.position = new Vector2(transform.position.x, transform.position.y) + move;
	}
}
