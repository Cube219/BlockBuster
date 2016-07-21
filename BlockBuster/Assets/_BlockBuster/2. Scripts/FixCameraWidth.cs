using UnityEngine;
using System.Collections;

public class FixCameraWidth : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		float screenRatio = 1440f / 2560f;
		Camera c = GetComponent<Camera>();
		c.orthographicSize = screenRatio * (Screen.height / Screen.width);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
