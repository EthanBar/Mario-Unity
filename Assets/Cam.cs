﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cam : MonoBehaviour {
	private Transform mario;

	// Use this for initialization
	void Start () {
		Screen.SetResolution(256, 240, true, 60);
		mario = GameObject.Find("Mario").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (mario.position.x > transform.position.x) {
			transform.position =  new Vector3(mario.position.x, transform.position.y, -10);
		}
	}
}
