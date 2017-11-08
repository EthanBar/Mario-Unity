using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager main;

	public GameObject prefab;
	public AudioClip coin;
	public AudioClip goomba;
	public AudioClip breakBlock;

	private void Awake() {
		main = this;
	}

	public static void PlaySound(AudioClip clip) {
		AudioSource source = Instantiate(main.prefab).GetComponent<AudioSource>();
		source.clip = clip;
		source.Play();
		Destroy(source.gameObject, source.clip.length);
	}
}