using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour {
	private Text text;
	private int coins;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
	}

	public void AddCoins(int coins) {
		this.coins += coins;
		text.text = "*" + this.coins.ToString().PadLeft(2, '0');
	}
}
