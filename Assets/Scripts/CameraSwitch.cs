using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraSwitch : MonoBehaviour
{
    public GameObject[] objects;
	public Camera FPVCamera;
	public GameObject target;

    private int m_CurrentActiveObject;

    public void NextCamera()
    {
        int nextactiveobject = m_CurrentActiveObject + 1 >= objects.Length ? 0 : m_CurrentActiveObject + 1;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i == nextactiveobject);
        }

        m_CurrentActiveObject = nextactiveobject;
    }

	
	public void Start() {
		LoadPlayerPrefs ();
	}

	public void AttachToTarget(GameObject drone) {
		target = drone;
		FPVCamera.transform.parent = drone.transform;
		FPVCamera.transform.localPosition = new Vector3 (0, 0.07f, 0.32f);

	}

	public void Update()
	{
		bool next = Input.GetButtonDown ("CameraSwitch");
		if (next) {
			NextCamera ();
		}
	}

	public void LoadPlayerPrefs() {
		if (PlayerPrefs.HasKey("cameraTilt")) {
			FPVCamera.transform.localEulerAngles = new Vector3(-PlayerPrefs.GetFloat("cameraTilt"),0,0);
		}
		if (PlayerPrefs.HasKey("cameraFOV")) {
			FPVCamera.fieldOfView = PlayerPrefs.GetFloat("cameraFOV");
		}
	}

}
