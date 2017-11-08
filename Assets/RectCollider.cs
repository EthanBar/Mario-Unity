using UnityEngine;

public class RectCollider : MonoBehaviour {
	public float width = 1;
	public float height = 1;
	public BlockType blockType = BlockType.solid;
	public Pickup pickup = Pickup.coin;
	
	private CollisionInfo noCollision; 
	
	private void Awake() {
		Actor.RegisterCollider(this);
		noCollision = new CollisionInfo(false, false, false, false, this);
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
		return new CollisionInfo(collisions[3], collisions[1], collisions[0], collisions[2], this);
	}

	public GameObject GetGameObject() {
		return gameObject;
	}

	public float GetWidth() {
		return width;
	}
	
	public float GetHeight() {
		return height;
	}

	public Vector2 GetPosition() {
		return transform.position;
	}

	public BlockType GetBlockType() {
		return blockType;
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
	goomba
}

[System.Serializable]
public enum Pickup {
	coin
}