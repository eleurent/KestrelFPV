using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerScoreList : MonoBehaviour {

	public GameObject playerScoreEntryPrefab;
	public bool shouldUpdate = true;

	void Update () {
		if(!shouldUpdate) {
			return;
		}

		// Clear
		while(this.transform.childCount > 0) {
			Transform c = this.transform.GetChild(0);
			c.SetParent(null);  // Become Batman
			Destroy (c.gameObject);
		}

		// Populate
		foreach(PhotonPlayer player in PhotonNetwork.playerList) {
			GameObject go = (GameObject)Instantiate(playerScoreEntryPrefab);
			go.transform.SetParent(this.transform);
			go.transform.Find ("Username").GetComponent<Text>().text = player.name;
			go.transform.Find ("Lap").GetComponent<Text>().text = player.GetLap().ToString();
			TimeSpan timeSpan = TimeSpan.FromSeconds(player.GetTime());
			go.transform.Find ("Time").GetComponent<Text>().text = String.Format("{0:D2}:{1:D2}:{2:D3}",timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		}
		shouldUpdate = false;
	}

	void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps) {
		shouldUpdate = true;
	}
}
