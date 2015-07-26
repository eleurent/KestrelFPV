using UnityEngine;
using System.Collections;

public class RaceGate : MonoBehaviour {

	private RaceManager raceManager;
	private bool enabledGate = false;
	private Color enabledColor = new Color(0.83f, 0.69f, 0.21f, 1.0f);
	private Color nextColor = new Color(0.9f, 0.75f, 0.28f, 0.4f);
	private Color disabledColor = new Color(0.9f, 0.75f, 0.28f, 0.0f);

	void Start() {
		DisableGate ();
	}

	public void SetRaceManager(RaceManager rm) {
		raceManager = rm;
	}

	public void EnableGate() {
		enabledGate = true;
		GetComponent<Renderer> ().material.color = enabledColor;
		Show ();
	}

	public void setNextGate() {
		enabledGate = false;
		GetComponent<Renderer> ().material.color = nextColor;
		Show ();
	}

	public void DisableGate() {
		enabledGate = false;
		GetComponent<Renderer> ().material.color = disabledColor;
		Hide ();
	}

	public bool isEnabled() {
		return enabledGate;
	}

	void OnTriggerEnter(Collider other) {
		if (enabledGate) {
			GameObject root = other.transform.root.gameObject;
			if (root.name.Contains ("Drone")) {
				if (root.GetComponent<PhotonView> ().isMine) {
					DisableGate ();
					raceManager.NextGate ();
				}
			}
		}
	}

	public void Hide() {
		GetComponent<Renderer> ().enabled = false;
	}

	public void Show() {
		GetComponent<Renderer> ().enabled = true;
	}
}
