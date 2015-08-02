using ExitGames.Client.Photon;
using UnityEngine;


/// <summary>
/// Simple script that uses a property to sync a start time for a multiplayer game.
/// </summary>
/// <remarks>
/// When entering a room, the first player will store the synchronized timestamp. 
/// You can't set the room's synchronized time in CreateRoom, because the clock on the Master Server
/// and those on the Game Servers are not in sync. We use many servers and each has it's own timer.
/// 
/// Everyone else will join the room and check the property to calculate how much time passed since start.
/// You can start a new round whenever you like.
/// 
/// Based on this, you should be able to implement a synchronized timer for turns between players.
/// </remarks>
public class InRoomRoundTimer : MonoBehaviour
{
    public int TurnDuration = 1;                  // time per round/turn
	public int BreakDuration = 1;                  // time per round/turn

	private int gameMode;
	private double StartTime;                        // this should could also be a private. i just like to see this in inspector
    private bool startRoundWhenTimeIsSynced;        // used in an edge-case when we wanted to set a start time but don't know it yet.
    private const string StartTimeKey = "st";       // the name of our "start time" custom property.
	private bool timeHasBeenSynced;
	private int phase = 0;

    private void StartRoundNow()
    {
        // in some cases, when you enter a room, the server time is not available immediately.
        // time should be 0.0f but to make sure we detect it correctly, check for a very low value.
        if (PhotonNetwork.time < 0.0001f)
        {
            // we can only start the round when the time is available. let's check that in Update()
            startRoundWhenTimeIsSynced = true;
            return;
        }
        startRoundWhenTimeIsSynced = false;

        

        ExitGames.Client.Photon.Hashtable startTimeProp = new Hashtable();  // only use ExitGames.Client.Photon.Hashtable for Photon
        startTimeProp[StartTimeKey] = PhotonNetwork.time;
        PhotonNetwork.room.SetCustomProperties(startTimeProp);              // implement OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged) to get this change everywhere
    }

	public void Start()
	{
		if (PhotonNetwork.offlineMode) {
			if (PlayerPrefs.HasKey(LobbyMenu.MODE_KEY)) {
				gameMode = PlayerPrefs.GetInt(LobbyMenu.MODE_KEY);
			}
			this.StartRoundNow();
		}
	}
    
    /// <summary>Called by PUN when this client entered a room (no matter if joined or created).</summary>
    public void OnJoinedRoom()
    {
		// Game mode
		if (PhotonNetwork.room.customProperties.ContainsKey (LobbyMenu.MODE_KEY)) {
			gameMode = (int)  PhotonNetwork.room.customProperties[LobbyMenu.MODE_KEY];
		}

		// Start time
        if (PhotonNetwork.isMasterClient)
        {
            this.StartRoundNow();
        }
        else
        {
            // as the creator of the room sets the start time after entering the room, we may enter a room that has no timer started yet
            Debug.Log("StartTime already set: " + PhotonNetwork.room.customProperties.ContainsKey(StartTimeKey));
			if (PhotonNetwork.room.customProperties.ContainsKey(StartTimeKey)) {
				StartTime = (double) PhotonNetwork.room.customProperties[StartTimeKey];
				timeHasBeenSynced = true;
			}
        }
    }

    /// <summary>Called by PUN when new properties for the room were set (by any client in the room).</summary>
    public void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(StartTimeKey))
        {
            StartTime = (double)propertiesThatChanged[StartTimeKey];
			timeHasBeenSynced = true;
        }
    }

    /// <remarks>
    /// In theory, the client which created the room might crash/close before it sets the start time.
    /// Just to make extremely sure this never happens, a new masterClient will check if it has to
    /// start a new round.
    /// </remarks>
    public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (!PhotonNetwork.room.customProperties.ContainsKey(StartTimeKey))
        {
            Debug.Log("The new master starts a new round, cause we didn't start yet.");
            this.StartRoundNow();
        }
    }


    void Update()
    {
        if (startRoundWhenTimeIsSynced) {
			this.StartRoundNow ();   // the "time is known" check is done inside the method.
		}

		if (LobbyMenu.MODES [gameMode] == "Race") {
			if (timeHasBeenSynced) {
				double elapsedTime = (PhotonNetwork.time - StartTime);
				double wrappedElapsedTime = elapsedTime % (TurnDuration + BreakDuration);
				switch (phase) {
				case 0:
				// Break phase
					GetComponent<DroneNetworkManager> ().RaceManager.GetComponent<RaceManager> ().SetTimeText (string.Format ("Start in: {0:0}", 0.5 + BreakDuration - wrappedElapsedTime));
					if (wrappedElapsedTime > BreakDuration) {
						GetComponent<DroneNetworkManager> ().GetMyDrone ().GetComponent<ResetDrone> ().Restart ();
						GetComponent<DroneNetworkManager> ().RaceManager.GetComponent<RaceManager> ().SetTextColor (Color.white);
						GetComponent<DroneNetworkManager> ().RaceManager.GetComponent<RaceManager> ().StartRace ();
						phase = 1;
					}
					break;
				case 1:
				// Round phase
					if (wrappedElapsedTime <= BreakDuration) {
						GetComponent<DroneNetworkManager> ().RaceManager.GetComponent<RaceManager> ().SetLapText ("Waiting...");
						GetComponent<DroneNetworkManager> ().RaceManager.GetComponent<RaceManager> ().SetTextColor (Color.red);
						GetComponent<DroneNetworkManager> ().RaceManager.GetComponent<RaceManager> ().StopRace ();
						phase = 0;
					}
					break;
				}
			}
		} else {
			GetComponent<DroneNetworkManager> ().RaceManager.SetActive(false);
		}
    }
}
