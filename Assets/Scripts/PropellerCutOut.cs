using UnityEngine;
using System.Collections;

public class PropellerCutOut : MonoBehaviour {

	public int motorNumber;

	void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag ("Player")) {
//			GameObject drone = GameObject.Find ("Drone");
//			drone.GetComponent<PropellersController> ().stopMotor(motorNumber);
		}
	}

	void OnTriggerExit(Collider other) {
		if (!other.CompareTag ("Player")) {
//			GameObject drone = GameObject.Find ("Drone");
//			drone.GetComponent<PropellersController> ().startMotor(motorNumber);
		}
	}

}
