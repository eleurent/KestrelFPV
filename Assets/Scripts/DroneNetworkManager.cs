using UnityEngine;
using System.Collections;

public class DroneNetworkManager : MonoBehaviour 
{
	public GameObject Cameras;
	public GameObject OptionsCanvas;
	public GameObject RaceManager;
	private GameObject myDrone;
	public GUISkin Skin;
	public Vector3 spawnZoneCenter;
	public Vector3 spawnZoneDimension;
	public Vector3 spawnZoneRotation;


	public void Awake()
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			Application.LoadLevel(LobbyMenu.SceneNameMenu);
			return;
		}
		
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		CreatePlayerObject ();
	}
	
	void OnJoinedRoom()
	{
//		CreatePlayerObject();
	}
	
	void CreatePlayerObject()
	{
		// Choose spawn position
		int positionIndex = PhotonNetwork.player.ID % 8;
		Vector3 normalizedPosition = 2*((positionIndex % 2) * Vector3.forward + ((positionIndex / 2) % 2) * Vector3.up + ((positionIndex / 4) % 2) * Vector3.right)-Vector3.one;
		Vector3 position = transform.position + spawnZoneCenter + Vector3.Scale(normalizedPosition, spawnZoneDimension);

		// Spawn a drone
		myDrone = PhotonNetwork.Instantiate("Drone", position, Quaternion.Euler(spawnZoneRotation), 0);

		// Attach local cameras and option panel
		Cameras.GetComponent<CameraSwitch>().AttachToTarget (myDrone);
		OptionsCanvas.GetComponent<OptionsMenuController> ().SetDrone (myDrone);
		myDrone.GetComponent<ResetDrone> ().setRaceManager(RaceManager.GetComponent<RaceManager>());

		// Use physics
		myDrone.GetComponent<Rigidbody> ().useGravity = true;

		// Send color
		Color myColor = DroneStyle.PickRandomColor();
		myDrone.GetComponent<PhotonView> ().RPC("ReceiveColorRPC", PhotonTargets.AllBuffered, new Vector3(myColor.r, myColor.g, myColor.b));
	}

	public GameObject GetMyDrone() {
		return myDrone;
	}

	public void OnGUI()
	{
		if (this.Skin != null)
		{
			GUI.skin = this.Skin;
		}
		if (GUILayout.Button("Return to Lobby"))
		{
			PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
		}
	}
	
	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched: " + player);
		
		string message;
		SceneChat chatComponent = GetComponent<SceneChat>();  // if we find a InRoomChat component, we print out a short message
		
		if (chatComponent != null)
		{
			// to check if this client is the new master...
			if (player.isLocal)
			{
				message = "You are Master Client now.";
			}
			else
			{
				message = player.name + " is Master Client now.";
			}
			
			
			chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
		}
	}
	
	public void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom (local)");
		
		// back to main menu        
		Application.LoadLevel(LobbyMenu.SceneNameMenu);
	}
	
	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton");
		
		// back to main menu        
		Application.LoadLevel(LobbyMenu.SceneNameMenu);
	}
	
	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
	}
	
	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
	}
	
	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPlayerDisconneced: " + player);
	}
	
	public void OnFailedToConnectToPhoton()
	{
		Debug.Log("OnFailedToConnectToPhoton");
		
		// back to main menu        
		Application.LoadLevel(LobbyMenu.SceneNameMenu);
	}
}
