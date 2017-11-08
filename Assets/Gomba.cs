using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gomba : MonoBehaviour {
	private bool right;
	public Vector2 dimensions;

	private float downAcc;
	
	// Update is called once per frame
	void Update () {
		float rightmove = right ? 1 : -1;
		Vector2 move = new Vector2(rightmove * 0.045f, downAcc);
		downAcc -= 0.0234f;
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
				move.x = 0;
				right = false;
			} else if (collision.hitLeft) {
				transform.position = new Vector2(collision.obj.GetPosition().x - dimensions.x / 2 - collision.obj.GetWidth() / 2, transform.position.y);
				move.x = 0;
				right = true;
			}
		}
		transform.position = new Vector2(transform.position.x, transform.position.y) + move;
	}
}
