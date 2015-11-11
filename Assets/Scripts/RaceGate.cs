using UnityEngine;
using System.Collections;

public class RaceGate : MonoBehaviour {

	public AudioClip gateSound;
	public Color enabledColor = new Color(0.83f, 0.69f, 0.21f, 1.0f);
	public Color nextColor = new Color(0.9f, 0.75f, 0.28f, 0.6f);
	public Color emissionColor = new Color(0.5f, 0.5f, 0.0f, 0.5f);
	public Color nextEmissionColor = new Color(0.1f, 0.1f, 0.1f, 0.0f);

	private RaceManager raceManager;
	private bool enabledGate = false;

	private AudioSource gateSoundSource;

	void Start() {
		gateSoundSource = gameObject.AddComponent<AudioSource>();
		gateSoundSource.playOnAwake = false;
		gateSoundSource.clip = gateSound;
		DisableGate ();
	}

	public void SetRaceManager(RaceManager rm) {
		raceManager = rm;
	}

	public void EnableGate() {
		enabledGate = true;
		GetComponent<Renderer> ().material.color = enabledColor;
		GetComponent<Renderer> ().material.SetColor ("_EmissionColor", emissionColor);
		Show ();
	}

	public void setNextGate() {
		enabledGate = false;
		GetComponent<Renderer> ().material.color = nextColor;
		GetComponent<Renderer> ().material.SetColor ("_EmissionColor", nextEmissionColor);
		Show ();
	}

	public void DisableGate() {
		enabledGate = false;
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
					gateSoundSource.Play();
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
