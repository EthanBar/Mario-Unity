using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBreak : MonoBehaviour {

	private Vector2 acceleration;
	private Vector2 velocity;
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector2 position = transform.position;
		position += velocity;
		velocity += acceleration;
		transform.position = position;
	}
	
	public void SetData(Vector2 acceleration, Vector2 velocity) {
		this.velocity = velocity;
		this.acceleration = acceleration;
		Destroy(gameObject, 2.0f);
	}
	
}
