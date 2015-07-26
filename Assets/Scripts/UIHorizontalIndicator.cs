using UnityEngine;
using System.Collections;

public class UIHorizontalIndicator : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject drone = GameObject.FindGameObjectWithTag ("Player");
		if (drone != null)
			transform.localEulerAngles = new Vector3 (0, 0, -drone.transform.localEulerAngles [2]);
	}
}
