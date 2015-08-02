// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerMenu.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using ExitGames.Client.Photon;

public class LobbyMenu : MonoBehaviour
{
    public GUISkin Skin;
    public Vector2 WidthAndHeight = new Vector2(600,400);
    
	private string roomName = "myRoom";
	public static string MAP_KEY = "map";
	public static string MODE_KEY = "m";
	public static string[] MAPS = new string[] {"Viking", "Forest", "Stadium"};
	public static string[] MODES = new string[] {"Freeflight", "Race"};
	private int selectedMap = 0;
	private int selectedMode = 0;

    private Vector2 scrollPos = Vector2.zero;
    private bool connectFailed = false;

	public static readonly string SceneNameMenu = "Lobby";
    public static readonly string SceneNameGame = "MultiScene";

    private string errorDialog;
    private double timeToClearDialog;
    public string ErrorDialog
    {
        get 
        { 
            return errorDialog; 
        }
        private set
        {
            errorDialog = value;
            if (!string.IsNullOrEmpty(value))
            {
                timeToClearDialog = Time.time + 4.0f;
            }
        }
    }

    public void Awake()
    {
		PhotonNetwork.autoJoinLobby = true;

        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("2.0");
        }

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
			if (PlayerPrefs.HasKey("playerName")) {
				PhotonNetwork.playerName = PlayerPrefs.GetString("playerName");
			}
			else {
            	PhotonNetwork.playerName = "Pilot" + Random.Range(1, 9999);
			}
        }

        // if you wanted more debug out, turn this on:
        // PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }

	public void Start() {
		if (PlayerPrefs.HasKey(LobbyMenu.MODE_KEY)) {
			selectedMode = PlayerPrefs.GetInt(LobbyMenu.MODE_KEY);
		}
		if (PlayerPrefs.HasKey(LobbyMenu.MAP_KEY)) {
			selectedMap = PlayerPrefs.GetInt(LobbyMenu.MAP_KEY);
		}
	}

    public void OnGUI()
    {
        if (this.Skin != null)
        {
            GUI.skin = this.Skin;
        }

        if (!PhotonNetwork.connected)
        {
            if (PhotonNetwork.connecting)
            {
                GUILayout.Label("Connecting to: " + PhotonNetwork.ServerAddress);
            }
            else
            {
                GUILayout.Label("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
            }
            
            if (this.connectFailed)
            {
                GUILayout.Label("Connection failed. Check setup and use Setup Wizard to fix configuration.");
//                GUILayout.Label(String.Format("Server: {0}", new object[] {PhotonNetwork.ServerAddress}));
//                GUILayout.Label("AppId: " + PhotonNetwork.PhotonServerSettings.AppID);
				GUILayout.BeginHorizontal();
                if (GUILayout.Button("Try Again", GUILayout.Width(160)))
                {
                    this.connectFailed = false;
                    PhotonNetwork.ConnectUsingSettings("2.0");
                }
				if (GUILayout.Button("Offline mode", GUILayout.Width(160)))
				{
					PhotonNetwork.offlineMode = true;
					this.connectFailed = false;
				}
				GUILayout.EndHorizontal();
            }

            return;
        }

        Rect content = new Rect((Screen.width - WidthAndHeight.x)/2, (Screen.height - WidthAndHeight.y)/2, WidthAndHeight.x, WidthAndHeight.y);
        GUI.Box(content,"Join or Create Room");
        GUILayout.BeginArea(content);

        GUILayout.Space(40);
        
        // Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player name:", GUILayout.Width(150));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
        GUILayout.Space(158);
        if (GUI.changed)
        {
            // Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        }
        GUILayout.EndHorizontal();

		// Mode
		GUILayout.BeginHorizontal ();
		GUILayout.Label("Game mode:", GUILayout.Width(150));
		selectedMode = GUILayout.Toolbar (selectedMode, MODES);
		if (GUI.changed)
		{
			// Save mode
			PlayerPrefs.SetInt(MODE_KEY, selectedMode);
		}
		GUILayout.EndHorizontal ();

		// Map
		GUILayout.BeginHorizontal ();
		GUILayout.Label("Map:", GUILayout.Width(150));
		selectedMap = GUILayout.Toolbar (selectedMap, MAPS);
		if (GUI.changed)
		{
			// Save map
			PlayerPrefs.SetInt(MAP_KEY, selectedMap);
		}
		GUILayout.EndHorizontal ();

        
		// Create a room (fails if exist!)
        GUILayout.BeginHorizontal();
        GUILayout.Label("Roomname:", GUILayout.Width(150));
        this.roomName = GUILayout.TextField(this.roomName);
        
        if (GUILayout.Button("Create Room", GUILayout.Width(160)))
        {
			RoomOptions roomOptions = new RoomOptions(){maxPlayers = 8};
			string[] customRoomPropertiesForLobby = { MODE_KEY, MAP_KEY };
			Hashtable customRoomProperties = new Hashtable() {{ MAP_KEY, selectedMap }, {MODE_KEY,  selectedMode}};
			roomOptions.customRoomPropertiesForLobby = customRoomPropertiesForLobby;
			roomOptions.customRoomProperties = customRoomProperties;
            PhotonNetwork.CreateRoom(this.roomName, roomOptions, null);
        }
        GUILayout.EndHorizontal();

		// Join room by title
		if (!PhotonNetwork.offlineMode) {

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Join Room", GUILayout.Width (160))) {
				PhotonNetwork.JoinRoom (this.roomName);
			}

			GUILayout.EndHorizontal ();


			if (!string.IsNullOrEmpty (this.ErrorDialog)) {
				GUILayout.Label (this.ErrorDialog);

				if (timeToClearDialog < Time.time) {
					timeToClearDialog = 0;
					this.ErrorDialog = "";
				}
			}

			GUILayout.Space (15);

			// Join random room
			GUILayout.BeginHorizontal ();

			GUILayout.Label (PhotonNetwork.countOfPlayers + " users are online in " + PhotonNetwork.countOfRooms + " rooms.");
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Join Random", GUILayout.Width (160))) {
				PhotonNetwork.JoinRandomRoom ();
			}
	        

			GUILayout.EndHorizontal ();

			GUILayout.Space (15);

			if (PhotonNetwork.connectionStateDetailed == PeerState.JoinedLobby) {
			
				if (PhotonNetwork.GetRoomList ().Length == 0) {
					GUILayout.Label ("Currently no games are available.");
					GUILayout.Label ("Rooms will be listed here, when they become available.");
				} else {
					GUILayout.Label (PhotonNetwork.GetRoomList ().Length + " rooms available:");

					// Room listing: simply call GetRoomList: no need to fetch/poll whatever!
					this.scrollPos = GUILayout.BeginScrollView (this.scrollPos);
					foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList()) {
						int map = 0, mode = 0;
						if (roomInfo.customProperties.ContainsKey (MAP_KEY)) {
							map = (int)roomInfo.customProperties [MAP_KEY];
						}
						if (roomInfo.customProperties.ContainsKey (MODE_KEY)) {
							mode = (int)roomInfo.customProperties [MODE_KEY];
						}
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("[" + MODES [mode] + "] " + roomInfo.name + " - " + MAPS [map] + " (" + roomInfo.playerCount + "/" + roomInfo.maxPlayers + ")");
						if (GUILayout.Button ("Join", GUILayout.Width (150))) {
							PhotonNetwork.JoinRoom (roomInfo.name);
						}

						GUILayout.EndHorizontal ();
					}

					GUILayout.EndScrollView ();
				}
			} else {
				GUILayout.Label ("Loading...");
			}
		}
        GUILayout.EndArea();
    }

    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }


    public void OnPhotonCreateRoomFailed()
    {
        this.ErrorDialog = "Error: Can't create room (room name maybe already used).";
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
    }

    public void OnPhotonJoinRoomFailed(object[] cause)
    {
        this.ErrorDialog = "Error: Can't join room: " + cause[1];
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
    }
    public void OnPhotonRandomJoinFailed()
    {
        this.ErrorDialog = "Error: Can't join random room (none found).";
        Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        PhotonNetwork.LoadLevel(SceneNameGame);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.networkingPeer.ServerAddress);
    }
}
