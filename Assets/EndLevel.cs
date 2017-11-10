using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EndLevel : MonoBehaviour {

	private int flagFrame;
	private bool finished;

	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private bool poweredUp;
	private float topPole;
	private Mario mario;

	public void HitPole(Animator animator, Mario mario, SpriteRenderer spriteRenderer, bool poweredUp) {
		this.animator = animator;
		this.mario = mario;
		this.spriteRenderer = spriteRenderer;
		this.poweredUp = poweredUp;
		finished = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!finished) return;
		flagFrame++;
		if (flagFrame == 1) {
			GameObject.Find("Time").GetComponent<Time>().stopClock = true;
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Pole"), 1);
			animator.SetBool("isMega", poweredUp);
			topPole = mario.transform.position.y;
			AudioManager.PlaySound(AudioManager.main.flag, 1);
			Camera.main.GetComponent<AudioSource>().Stop();
		} else if (flagFrame < 70) {
			mario.transform.position = new Vector2(195, Mathf.Lerp(topPole, -3f, flagFrame / 70f));
			transform.position = new Vector2(195, Mathf.Lerp(4.5f, -3.5f, flagFrame / 70f));
		} else if (flagFrame < 100) {
			spriteRenderer.flipX = true;
			mario.transform.position = new Vector2(196, -3f);
		} else if (flagFrame == 100) {
			spriteRenderer.flipX = false;
			animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 0);
			animator.SetLayerWeight(animator.GetLayerIndex("Pole"), 0);
			animator.SetFloat("xvel", 1);
			animator.SetBool("skidding", false);
			animator.SetBool("jumping", false);
			if (poweredUp) {
				animator.SetLayerWeight(animator.GetLayerIndex("Mega"), 1);
			} else {
				animator.SetLayerWeight(animator.GetLayerIndex("Mini"), 1);
			}
			animator.SetBool("isMega", poweredUp);
			AudioManager.PlaySound(AudioManager.main.win, 1);
		} else if (flagFrame < 200) {
			mario.transform.position = new Vector2(Mathf.Lerp(196, 200, (flagFrame - 100) / 100f), -5 + mario.dimensions.y / 2);
		} else if (flagFrame == 200) {
			spriteRenderer.enabled = false;
			GameObject.Find("Time").GetComponent<Time>().finishlevel = true;
		}
	}
}
