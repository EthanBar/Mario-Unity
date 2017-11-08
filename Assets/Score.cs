using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {
	private Text text;
	private int score;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}

	public void AddScore(int score) {
		this.score += score;
		text.text = "Mario\n" + this.score.ToString().PadLeft(6, '0');
	}
}
