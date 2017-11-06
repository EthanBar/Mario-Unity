using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Mario : MonoBehaviour {
	
	private Rigidbody2D rb2d;
	private SpriteRenderer spriteRenderer;
	private Animator animator;

	private float xvel, yvel;

	private const float conversion = 65536;
	private const float maxX = 10496 / conversion;
	private const float runAcc = 152 / conversion;
	private const float skidPower = 416 / conversion;
	private const float releaseDeAcc = 208 / conversion;

	// Use this for initialization
	void Start () {
		xvel = 0;
		yvel = 0;
		rb2d = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		bool moving = false;
		bool skidding = false;
		if (Input.GetKey(KeyCode.D)) {
			moving = true;
			if (xvel >= 0) {
				xvel += runAcc;
			} else if (xvel < 0) { // Skidding
				xvel += skidPower;
				skidding = true;
			}
		}
		if (Input.GetKey(KeyCode.A)) {
			moving = true;
			if (xvel <= 0) {
				xvel -= runAcc;
			} else if (xvel > 0) { // Skidding
				xvel -= skidPower;
				skidding = true;
			}
		}
		if (!moving) {
			if (xvel > 0) {
				xvel -= releaseDeAcc;
				if (xvel < 0) xvel = 0;
			} else {
				xvel += releaseDeAcc;
				if (xvel > 0) xvel = 0;
			}
		}
		if (xvel > maxX) {
			xvel = maxX;
		} else if (xvel < -maxX) {
			xvel = -maxX;
		}
		if (xvel > 0) {
			spriteRenderer.flipX = false;
		} else if (xvel < 0) {
			spriteRenderer.flipX = true;
		}
		animator.SetFloat("xvel", Mathf.Abs(xvel));
		animator.SetBool("skidding", skidding);
		print(xvel);
		rb2d.MovePosition(new Vector2(transform.position.x, transform.position.y) + new Vector2(xvel, yvel));
	}

	void OnCollisionStay2D(Collision2D other) {
		xvel = 0;
	}
}
