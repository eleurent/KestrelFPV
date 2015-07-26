using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsMenuController : MonoBehaviour {

	private bool initialized = false;
	private ControllerManager controllerManager;
	private CameraSwitch cameraSwitch;

	public GameObject drone;
	public GameObject optionsPanel;

	public Slider tiltSensitivitySlider;
	public Slider yawSensitivitySlider;
	public Slider heightSensitivitySlider;
	public Toggle squareMappingToggle;
	public Toggle axisToggleT;
	public Toggle axisToggleR;
	public Toggle axisToggleE;
	public Toggle axisToggleA;
	public Slider tiltRateKpSlider;
	public Slider yawRateKpSlider;
	public Toggle enableStabilizationToggle;
	public Slider tiltAngleSlider;
	public Slider verticalSpeedSlider;
	public Slider tiltAngleKpSlider;
	public Slider yawAngleKpSlider;
	public Slider verticalSpeedKpSlider;
	public Slider cameraTiltSlider;
	public Slider cameraFOVSlider;

	private float defaultTiltSensibility;
	private float defaultYawSensibility;
	private float defaultHeightSensibility;
	private bool defaultSquareMapping;
	private bool defaultInvertAxisT;
	private bool defaultInvertAxisR;
	private bool defaultInvertAxisE;
	private bool defaultInvertAxisA;
	private float defaultTiltRateKp;
	private float defaultYawRateKp;
	private bool defaultEnableStabilization;
	private float defaultTiltAngle;
	private float defaultVerticalSpeed;
	private float defaultTiltAngleKp;
	private float defaultYawAngleKp;
	private float defaultVerticalSpeedKp;
	private float defaultCameraTilt;
	private float defaultCameraFOV;
	
	void Start () {
		GameObject cameras = GameObject.Find ("Cameras");
		cameraSwitch = cameras.GetComponent<CameraSwitch> ();
	}

	public void SetDrone(GameObject drone) {
		controllerManager = drone.GetComponent<ControllerManager> ();
		Initialize ();
	}

	void Initialize() {
		if (!initialized && optionsPanel.activeSelf) {
			// Set default
			defaultTiltSensibility = tiltSensitivitySlider.value;
			defaultYawSensibility = yawSensitivitySlider.value;
			defaultHeightSensibility = heightSensitivitySlider.value;
			defaultSquareMapping = squareMappingToggle.isOn;
			defaultInvertAxisT = axisToggleT.isOn;
			defaultInvertAxisR = axisToggleR.isOn;
			defaultInvertAxisE = axisToggleE.isOn;
			defaultInvertAxisA = axisToggleA.isOn;
			defaultTiltRateKp = tiltRateKpSlider.value;
			defaultYawRateKp = yawRateKpSlider.value;
			defaultEnableStabilization = enableStabilizationToggle.isOn;
			defaultTiltAngle = tiltAngleSlider.value;
			defaultVerticalSpeed = verticalSpeedSlider.value;
			defaultTiltAngleKp = tiltAngleKpSlider.value;
			defaultYawAngleKp = yawAngleKpSlider.value;
			defaultVerticalSpeedKp = verticalSpeedKpSlider.value;
			defaultCameraTilt = cameraTiltSlider.value;
			defaultCameraFOV = cameraFOVSlider.value;

			// Load PlayerPrefs
			if (PlayerPrefs.HasKey ("tiltSensitivity")) {
				tiltSensitivitySlider.value = PlayerPrefs.GetFloat ("tiltSensitivity");
			}
			if (PlayerPrefs.HasKey ("yawSensitivity")) {
				yawSensitivitySlider.value = PlayerPrefs.GetFloat ("yawSensitivity");
			}
			if (PlayerPrefs.HasKey ("heightSensitivity")) {
				heightSensitivitySlider.value = PlayerPrefs.GetFloat ("heightSensitivity");
			}
			if (PlayerPrefs.HasKey("squareMapping")) {
				squareMappingToggle.isOn = (PlayerPrefs.GetInt("squareMapping") > 0);
			}
			if (PlayerPrefs.HasKey("invertAxisT")) {
				axisToggleT.isOn = (PlayerPrefs.GetInt("invertAxisT") > 0);
			}
			if (PlayerPrefs.HasKey("invertAxisR")) {
				axisToggleR.isOn = (PlayerPrefs.GetInt("invertAxisR") > 0);
			}
			if (PlayerPrefs.HasKey("invertAxisE")) {
				axisToggleE.isOn = (PlayerPrefs.GetInt("invertAxisE") > 0);
			}
			if (PlayerPrefs.HasKey("invertAxisA")) {
				axisToggleA.isOn = (PlayerPrefs.GetInt("invertAxisA") > 0);
			}
			if (PlayerPrefs.HasKey ("tiltRateKp")) {
				tiltRateKpSlider.value = PlayerPrefs.GetFloat ("tiltRateKp");
			}
			if (PlayerPrefs.HasKey ("yawRateKp")) {
				yawRateKpSlider.value = PlayerPrefs.GetFloat ("yawRateKp");
			}
			if (PlayerPrefs.HasKey ("enableStabilization")) {
				enableStabilizationToggle.isOn = (PlayerPrefs.GetInt("enableStabilization") > 0);
			}
			if (PlayerPrefs.HasKey ("tiltAngle")) {
				tiltAngleSlider.value = PlayerPrefs.GetFloat ("tiltAngle");
			}
			if (PlayerPrefs.HasKey ("verticalSpeed")) {
				verticalSpeedSlider.value = PlayerPrefs.GetFloat ("verticalSpeed");
			}
			if (PlayerPrefs.HasKey ("tiltAngleKp")) {
				tiltAngleKpSlider.value = PlayerPrefs.GetFloat ("tiltAngleKp");
			}
			if (PlayerPrefs.HasKey ("yawAngleKp")) {
				yawAngleKpSlider.value = PlayerPrefs.GetFloat ("yawAngleKp");
			}
			if (PlayerPrefs.HasKey ("verticalSpeedKp")) {
				verticalSpeedKpSlider.value = PlayerPrefs.GetFloat ("verticalSpeedKp");
			}
			if (PlayerPrefs.HasKey ("cameraTilt")) {
				cameraTiltSlider.value = PlayerPrefs.GetFloat ("cameraTilt");
			}
			if (PlayerPrefs.HasKey ("cameraFOV")) {
				cameraFOVSlider.value = PlayerPrefs.GetFloat ("cameraFOV");
			}

			// Update texts
			tiltSensitivitySlider.GetComponentInChildren<Text> ().text = "Tilt sensitivity: " + tiltSensitivitySlider.value.ToString ("F0") + "°/s";
			yawSensitivitySlider.GetComponentInChildren<Text> ().text = "Yaw sensitivity: " + yawSensitivitySlider.value.ToString ("F0") + "°/s";
			heightSensitivitySlider.GetComponentInChildren<Text> ().text = "Height sensitivity: " + heightSensitivitySlider.value.ToString ("F0") + " RPM";
			tiltRateKpSlider.GetComponentInChildren<Text> ().text = "Tilt rate Kp: " + tiltRateKpSlider.value.ToString ("F0") + " Hz";
			yawRateKpSlider.GetComponentInChildren<Text> ().text = "Yaw rate Kp: " + yawRateKpSlider.value.ToString ("F0") + " Hz";
			tiltAngleSlider.GetComponentInChildren<Text> ().text = "Tilt angle: " + tiltAngleSlider.value.ToString ("F0") + "°";
			verticalSpeedSlider.GetComponentInChildren<Text> ().text = "Vertical speed: " + verticalSpeedSlider.value.ToString ("F0") + "m/s";
			tiltAngleKpSlider.GetComponentInChildren<Text> ().text = "Tilt angle Kp: " + tiltAngleKpSlider.value.ToString ("F0") + " Hz";
			yawAngleKpSlider.GetComponentInChildren<Text> ().text = "Yaw angle Kp: " + yawAngleKpSlider.value.ToString ("F0") + " Hz";
			verticalSpeedKpSlider.GetComponentInChildren<Text> ().text = "Vertical speed Kp: " + verticalSpeedKpSlider.value.ToString ("F0") + " Hz";
			cameraTiltSlider.GetComponentInChildren<Text> ().text = "FPV camera tilt: " + cameraTiltSlider.value.ToString ("F0") + "°";
			cameraFOVSlider.GetComponentInChildren<Text> ().text = "FPV camera FOV: " + cameraFOVSlider.value.ToString ("F0") + "°";

			initialized = true;
		}
	}

	void Update() {
		if (Input.GetButtonDown ("Cancel")) {
			optionsPanel.SetActive(!optionsPanel.activeSelf);
		    Initialize();
		}
	}

	public void setTiltSensitivity(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("tiltSensitivity", value);
			tiltSensitivitySlider.GetComponentInChildren<Text>().text = "Tilt sensitivity: " + value.ToString("F0") + "°/s";
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setYawSensitivity(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("yawSensitivity", value);
			yawSensitivitySlider.GetComponentInChildren<Text>().text = "Yaw sensitivity: " + value.ToString("F0") + "°/s";
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setHeightSensitivity(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("heightSensitivity", value);
			heightSensitivitySlider.GetComponentInChildren<Text>().text = "Height sensitivity: " + value.ToString("F0") + " RPM";
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setSquareMapping(bool value) {
		if (initialized) {
			PlayerPrefs.SetInt("squareMapping", value ? 1 : 0);
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setInvertAxisT(bool value) {
		if (initialized) {
			PlayerPrefs.SetInt("invertAxisT", value ? 1 : 0);
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setInvertAxisR(bool value) {
		if (initialized) {
			PlayerPrefs.SetInt("invertAxisR", value ? 1 : 0);
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setInvertAxisE(bool value) {
		if (initialized) {
			PlayerPrefs.SetInt("invertAxisE", value ? 1 : 0);
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setInvertAxisA(bool value) {
		if (initialized) {
			PlayerPrefs.SetInt("invertAxisA", value ? 1 : 0);
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setTiltRateKp(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("tiltRateKp", value);
			tiltRateKpSlider.GetComponentInChildren<Text>().text = "Tilt rate Kp: " + value.ToString("F0") + " Hz";;
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setYawRateKp(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("yawRateKp", value);
			yawRateKpSlider.GetComponentInChildren<Text>().text = "Yaw rate Kp: " + value.ToString("F0") + " Hz";;
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setEnableStabilization(bool value) {
		if (initialized) {
			PlayerPrefs.SetInt("enableStabilization", value ? 1 : 0);
			if (value)
				controllerManager.resetYawRef();
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setTiltAngle(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("tiltAngle", value);
			tiltAngleSlider.GetComponentInChildren<Text>().text = "Tilt angle: " + value.ToString("F0") + "°";
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setVerticalSpeed(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("verticalSpeed", value);
			verticalSpeedSlider.GetComponentInChildren<Text>().text = "Vertical speed: " + value.ToString("F0") + "m/s";
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setTiltAngleKp(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("tiltAngleKp", value);
			tiltAngleKpSlider.GetComponentInChildren<Text>().text = "Tilt angle Kp: " + value.ToString("F0") + " Hz";;
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setYawAngleKp(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("yawAngleKp", value);
			yawAngleKpSlider.GetComponentInChildren<Text>().text = "Yaw angle Kp: " + value.ToString("F0") + " Hz";;
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setVerticalSpeedKp(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("verticalSpeedKp", value);
			verticalSpeedKpSlider.GetComponentInChildren<Text>().text = "Vertical speed Kp: " + value.ToString("F0") + " Hz";;
			controllerManager.LoadPlayerPrefs();
		}
	}

	public void setCameraTilt(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("cameraTilt", value);
			cameraTiltSlider.GetComponentInChildren<Text>().text = "FPV Camera tilt: " + value.ToString("F0") + "°";
			cameraSwitch.LoadPlayerPrefs();
		}
	}

	public void setCameraFOV(float value) {
		if (initialized) {
			PlayerPrefs.SetFloat ("cameraFOV", value);
			cameraFOVSlider.GetComponentInChildren<Text>().text = "FPV Camera FOV: " + value.ToString("F0") + "°";
			cameraSwitch.LoadPlayerPrefs();
		}
	}

	public void resetDefaults() {
		setTiltSensitivity (defaultTiltSensibility);
		setYawSensitivity (defaultYawSensibility);
		setHeightSensitivity (defaultHeightSensibility);
		setSquareMapping (defaultSquareMapping);
		setInvertAxisT (defaultInvertAxisT);
		setInvertAxisR (defaultInvertAxisR);
		setInvertAxisE (defaultInvertAxisE);
		setInvertAxisA (defaultInvertAxisA);
		setTiltRateKp (defaultTiltRateKp);
		setYawRateKp (defaultYawRateKp);
		setEnableStabilization (defaultEnableStabilization);
		setTiltAngle (defaultTiltAngle);
		setVerticalSpeed (defaultVerticalSpeed);
		setTiltAngleKp (defaultTiltAngleKp);
		setYawAngleKp (defaultYawAngleKp);
		setVerticalSpeedKp (defaultVerticalSpeedKp);
		setCameraTilt (defaultCameraTilt);
		setCameraFOV (defaultCameraFOV);

		tiltSensitivitySlider.value = PlayerPrefs.GetFloat ("tiltSensitivity");
		yawSensitivitySlider.value = PlayerPrefs.GetFloat ("yawSensitivity");
		heightSensitivitySlider.value = PlayerPrefs.GetFloat ("heightSensitivity");
		squareMappingToggle.isOn = (PlayerPrefs.GetInt ("squareMapping") > 0);
		axisToggleT.isOn = (PlayerPrefs.GetInt ("invertAxisT") > 0);
		axisToggleR.isOn = (PlayerPrefs.GetInt ("invertAxisR") > 0);
		axisToggleE.isOn = (PlayerPrefs.GetInt ("invertAxisE") > 0);
		axisToggleA.isOn = (PlayerPrefs.GetInt ("invertAxisA") > 0);
		tiltRateKpSlider.value = PlayerPrefs.GetFloat ("tiltRateKp");
		yawRateKpSlider.value = PlayerPrefs.GetFloat ("yawRateKp");
		enableStabilizationToggle.isOn = (PlayerPrefs.GetInt ("enableStabilization") > 0);
		tiltAngleSlider.value = PlayerPrefs.GetFloat ("tiltAngle");
		verticalSpeedSlider.value = PlayerPrefs.GetFloat ("verticalSpeed");
		tiltAngleKpSlider.value = PlayerPrefs.GetFloat ("tiltAngleKp");
		yawAngleKpSlider.value = PlayerPrefs.GetFloat ("yawAngleKp");
		verticalSpeedKpSlider.value = PlayerPrefs.GetFloat ("verticalSpeedKp");
		cameraTiltSlider.value = PlayerPrefs.GetFloat ("cameraTilt");
		cameraFOVSlider.value = PlayerPrefs.GetFloat ("cameraFOV");
	}
}
