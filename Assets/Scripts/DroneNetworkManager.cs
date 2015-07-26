using UnityEngine;
using System.Collections;

public class DroneNetworkManager : MonoBehaviour 
{
	public GameObject Cameras;
	public GameObject OptionsPanel;
	public GameObject RaceManager;
	private GameObject myDrone;

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
		Vector3 position = new Vector3(26.0f, 1.0f, -40.0f) + (Random.Range(0,6)-3)*Vector3.left + (-Random.Range(0,6))*Vector3.forward + (Random.Range(0,3))*Vector3.up;

		// Spawn a drone
		myDrone = PhotonNetwork.Instantiate("Drone", position, Quaternion.identity, 0);

		// Attach local cameras and option panel
		Cameras.GetComponent<CameraSwitch>().AttachToTarget (myDrone);
		OptionsPanel.GetComponent<OptionsMenuController> ().SetDrone (myDrone);
		RaceManager.GetComponent<RaceManager> ().drone = myDrone;

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
		if (GUILayout.Button("Return to Lobby"))
		{
			PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
		}
	}
	
	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched: " + player);
		
		string message;
		InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message
		
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
