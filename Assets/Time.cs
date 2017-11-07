using UnityEngine;
using UnityEngine.UI;

public class Time : MonoBehaviour {
	private Text text;
	private float clock;

	// Use this for initialization
	void Start () {
		text = GetComponent<Text>();
		clock = 400;
	}
	
	// Update is called once per frame
	void Update () {
		clock -= (1 / 60f) * 2.408f;
		text.text = "Time\n" + Mathf.Ceil(clock);
	}
}
