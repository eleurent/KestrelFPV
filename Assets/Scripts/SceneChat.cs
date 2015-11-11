using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class SceneChat : Photon.MonoBehaviour 
{
    public Rect GuiRect = new Rect(0,0, 250,300);
    public bool IsVisible = true;
    public bool AlignBottom = false;
    public List<string> messages = new List<string>();
    private string inputLine = "";
    private Vector2 scrollPos = Vector2.zero;
	public GUISkin Skin;

    public static readonly string ChatRPC = "Chat";
	private static string placeholder = "Press Enter to chat";

    public void Start()
    {
        if (this.AlignBottom)
        {
            this.GuiRect.y = Screen.height - this.GuiRect.height;
        }
    }

    public void OnGUI()
    {
        if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
        {
            return;
        }

		if (this.Skin != null)
		{
			GUI.skin = this.Skin;
		}
        
        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
			if (GUI.GetNameOfFocusedControl () == "ChatInput")
			{
				if (!string.IsNullOrEmpty(this.inputLine)) {
					this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
				}
                this.inputLine = "";
                GUI.FocusControl("");
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else
            {
                GUI.FocusControl("ChatInput");
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(this.GuiRect);

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.FlexibleSpace();
		for (int i = 0; i <messages.Count; i++)
        {
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
        inputLine = GUILayout.TextField(inputLine);


		if (UnityEngine.Event.current.type == EventType.Repaint)
		{
			if (GUI.GetNameOfFocusedControl () == "ChatInput")
			{
				if (inputLine == placeholder) inputLine = "";
			}
			else
			{
				if (inputLine == "") inputLine = placeholder;
			}
		}
		
		// Send button
		//        if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)))
//        {
//            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
//            this.inputLine = "";
//            GUI.FocusControl("");
//        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        string senderName = "anonymous";

        if (mi != null && mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.name))
            {
                senderName = mi.sender.name;
            }
            else
            {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Add(senderName +": " + newLine);
		scrollPos.y = Mathf.Infinity;
    }

    public void AddLine(string newLine)
    {
        this.messages.Add(newLine);
    }
}
