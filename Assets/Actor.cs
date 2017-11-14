using System.Collections.Generic;
using UnityEngine;

public static class Actor {
	
	private static List<RectCollider> colliders;

	public static CollisionInfo[] Collide(Vector2 curPos, Vector2 attemptPos, Vector2 dimensions, float shorten) {
		List<CollisionInfo> collisions = new List<CollisionInfo>();
		foreach (RectCollider collider in colliders) {
			CollisionInfo collision = collider.Collide(dimensions, curPos, attemptPos, shorten);
			if (collision.hitBottom || collision.hitTop || collision.hitRight || collision.hitLeft) {
				collisions.Add(collision);
			}
		}
		return collisions.ToArray();
	}

	public static void RegisterCollider(RectCollider collider) {
		if (colliders == null) colliders = new List<RectCollider>();
		colliders.Add(collider);
	}
	
	public static void DeleteCollider(RectCollider collider) {
		colliders.Remove(collider);
	}
}
