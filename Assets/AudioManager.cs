using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	public static AudioManager main;

	public GameObject prefab;
	public AudioClip coin;
	public AudioClip goomba;
	public AudioClip breakBlock;
	public AudioClip pipe;
	
	public AudioClip jump;
	public AudioClip megaJump;
	
	public AudioClip death;
	public AudioClip flag;
	public AudioClip win;
	
	public AudioClip beep;
	public AudioClip bump;

	private void Awake() {
		main = this;
	}

	public static void PlaySound(AudioClip clip, float volume) {
		AudioSource source = Instantiate(main.prefab).GetComponent<AudioSource>();
		source.clip = clip;
		source.Play();
		source.volume = volume;
		Destroy(source.gameObject, source.clip.length);
	}
}