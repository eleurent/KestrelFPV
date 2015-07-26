using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public class RaceManager : MonoBehaviour {

	public GameObject[] gates;
	public int numberOfLaps = 3;
	public GameObject raceCanvas;
	public Text lapText;
	public Text timeText;
	public GameObject drone;

	private bool raceStarted = false;
	private int currentGate = 0;
	private int currentLap = 0;
	private double startTime;
	private double endTime;

	void Start () {
		// Register gates;
		for (int i=0; i<gates.Length; i++) {
			gates[i].GetComponent<RaceGate>().SetRaceManager(this);
		}
		// Init race
		StopRace();
		lapText.text = "Waiting...";
		SetTextColor (Color.red);
	}

	public void StopRace() {
		Debug.Log ("Stop Race");
		for (int i=0; i<gates.Length; i++) {
			gates [i].GetComponent<RaceGate>().DisableGate ();
		}
		currentGate = 0;
		currentLap = 0;
	}

	public void StartRace() {
		Debug.Log ("Start Race");
		if (gates.Length > 0) {
			gates [(currentGate+1)%gates.Length].GetComponent<RaceGate> ().setNextGate ();
			gates [currentGate].GetComponent<RaceGate> ().EnableGate ();
			raceStarted = true;
			startTime = PhotonNetwork.time;
		}
		lapText.text = "Lap: 1/" + numberOfLaps;
	}
	
	public void NextGate () {
		Debug.Log ("Next Gate");
		currentGate = (currentGate + 1) % gates.Length;
		gates [(currentGate+1)%gates.Length].GetComponent<RaceGate> ().setNextGate ();
		gates[currentGate].GetComponent<RaceGate>().EnableGate();
		if (currentGate == 1) {
			NextLap ();
		}
	}

	void NextLap() {
		// End of race
		if (currentLap == numberOfLaps) {
			SetTextColor(Color.green);
			lapText.text = "Finished!";
			raceStarted = false;
			gates [currentGate].GetComponent<RaceGate>().DisableGate ();
		} else {
			// New lap
			currentLap++;
			lapText.text = "Lap: " + currentLap + "/" + numberOfLaps;
		}
	}

	public void SetLapText(string s) {
		lapText.text = s;
	}

	public void SetTimeText(string s) {
		timeText.text = s;
	}

	public void SetTextColor(Color c) {
		lapText.color = c;
		timeText.color = c;
	}

	void Update() {
		if (raceStarted) {
			double duration = PhotonNetwork.time - startTime;
			TimeSpan timeSpan = TimeSpan.FromSeconds(duration);
			SetTimeText(String.Format("{0:D2}:{1:D2}:{2:D3}",timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds));
		}
	}
}
