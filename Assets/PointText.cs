using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointText : MonoBehaviour {
	private RectTransform rect;
	private int frame;

	// Use this for initialization
	void Start () {
		rect = GetComponent<RectTransform>();
		transform.SetParent(GameObject.Find("Canvas").transform, false);
	}

	public void SetPoints(int points) {
		GetComponent<Text>().text = points.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if (frame > 60) Destroy(gameObject);
		rect.position = new Vector2(rect.position.x, rect.position.y + 1);
		frame++;
	}
}
