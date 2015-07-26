using UnityEngine;
using System.Collections;

public class DroneStyle : MonoBehaviour {

	public GameObject nose;
	public GameObject top;
	public GameObject propFL;
	public GameObject propFR;

	static public Color bebopBlue = new Color (5 / 255.0f, 116 / 255.0f, 186 / 255.0f);
	static public Color bebopRed = new Color (240 / 255.0f, 40 / 255.0f, 30 / 255.0f);
	static public Color bebopYellow = new Color (215 / 255.0f, 170 / 255.0f, 60 / 255.0f);

	private Color droneColor = bebopBlue;

	public void SetColor(Color color) {
		droneColor = color;
		nose.GetComponent<Renderer> ().material.SetColor ("_Color", droneColor);
		top.GetComponent<Renderer> ().material.SetColor ("_Color", droneColor);
		propFL.GetComponent<Renderer> ().material.SetColor ("_Color", droneColor);
		propFR.GetComponent<Renderer> ().material.SetColor ("_Color", droneColor);
	}

	public Color GetColor() {
		return droneColor;
	}

	public static Color PickRandomColor () {
		Color[] colorsList = {bebopBlue, bebopRed, bebopYellow};
		return colorsList[Random.Range(0, colorsList.Length)];
	}

	[PunRPC]
	public void ReceiveColorRPC(Vector3 color)
	{
		SetColor (new Color(color.x, color.y, color.z));
	}
}
