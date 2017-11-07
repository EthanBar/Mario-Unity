using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectCollider : MonoBehaviour {
	public float width = 1;
	public float height = 1;
	
	private CollisionInfo noCollision;
	
	private void Awake() {
		Mario.AddCollider(this);
		noCollision = new CollisionInfo(false, false, false, false, transform.position, width, height);
	}

	public CollisionInfo Collide(Vector2 dimensions, Vector2 currentPosition, Vector2 newPosition) {
		Vector2 myPosition = transform.position;

		float widthCol = width / 2 + dimensions.x / 2 - 0.01f;
		float heightCol = height / 2 + dimensions.y / 2 - 0.01f;
		
		bool withinX = myPosition.x < currentPosition.x + widthCol && myPosition.x > currentPosition.x - widthCol;
		bool withinY = myPosition.y < currentPosition.y + heightCol && myPosition.y > currentPosition.y - heightCol;
		if (!withinX && !withinY) return noCollision;

		bool[] collisions = new bool[4];
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
		return new CollisionInfo(collisions[3], collisions[1], collisions[0], collisions[2],
			transform.position, width, height);
	}
}

public struct CollisionInfo {
	public CollisionInfo(bool hitBottom, bool hitTop, bool hitRight, bool hitLeft,
						 Vector2 position, float width, float height) {
		this.hitBottom = hitBottom;
		this.hitTop = hitTop;
		this.hitRight = hitRight;
		this.hitLeft = hitLeft;
		this.position = position;
		this.width = width;
		this.height = height;
	}
	public bool hitBottom, hitTop, hitRight, hitLeft;
	public Vector2 position;
	public float width, height;
}