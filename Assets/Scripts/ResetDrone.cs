using UnityEngine;
using System.Collections;

public class ResetDrone : MonoBehaviour {
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private PhotonView m_PhotonView;
	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		m_PhotonView = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!m_PhotonView.isMine) {
			return;
		}
		if (Input.GetButtonDown ("Restart")) {
			Restart();
		}
		if (!PhotonNetwork.connected) {
			if (Input.GetButtonDown ("NextScene")) {
				Application.LoadLevel ((Application.loadedLevel + 1) % Application.levelCount);
			}
		}
	}

	public void Restart() {
		transform.position = initialPosition;
		transform.rotation = initialRotation;
		GetComponent<Rigidbody> ().velocity = Vector3.zero;
		GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		GetComponent<ControllerManager> ().resetYawRef();
	}
}
