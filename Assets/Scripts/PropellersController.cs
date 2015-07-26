using UnityEngine;
using System.Collections;

public class PropellersController : MonoBehaviour {

	public float maxRPM = 12000;
	
	private Vector4 RPM;
	private Vector3[] propellersPositions;
	private Vector4 propellersCW;

	private Vector3 thrustModel;
	private Vector3 torqueModel;

	private Vector4 motorStarted;
	
	private Vector3 totalForce;
	private Vector3 totalTorque;

	private float b0_roll;
	private float b0_pitch;
	private float b0_yaw;
	private float b0_z;

	// Use this for initialization
	void Start () {
		RPM = Vector4.zero;

		thrustModel = new Vector3 (1.597752e-006f, -9.509696e-004f, 0*0.799216e+000f)/1000.0f*9.81f;
		torqueModel = new Vector3 (2.230120e-007f, -9.213740e-005f, -0*0.034894e+000f)/1000.0f;

		float Lx = 0.10f;
		float Ly = 0.10f;
		propellersPositions = new Vector3[4];
		propellersPositions [0] = new Vector3 (-Lx, 0, Ly);
		propellersPositions [1] = new Vector3 (Lx, 0, Ly);
		propellersPositions [2] = new Vector3 (Lx, 0, -Ly);
		propellersPositions [3] = new Vector3 (-Lx, 0, -Ly);
		propellersCW = new Vector4 (-1, 1, -1, 1);
		motorStarted = Vector4.one;

		float Ix = 10e-4f;
		float Iy = 12e-4f;
		float Iz = 15e-4f;
		float m = 0.4f;
		b0_roll = 4 * Lx * (2 * thrustModel.x * 8200 + thrustModel.y)/Ix;
		b0_pitch = 4 * Ly * (2 * thrustModel.x * 8200 + thrustModel.y) / Iy;
		b0_yaw = 4 * (2 * torqueModel.x * 8200 + torqueModel.y) / Iz;
		b0_z = 4 * (2 * thrustModel.x * 8200 + thrustModel.y) / m;
	}

	public void MixRPM(float heightFeedforward, float heightCommand, float rollCommand, float pitchCommand, float yawCommand) {
		rollCommand =  rollCommand / b0_roll;
		pitchCommand = pitchCommand / b0_pitch;
		yawCommand = yawCommand / b0_yaw;
		heightCommand = heightCommand / b0_z;

		// Apply angle command
		RPM.Set (pitchCommand + rollCommand - yawCommand,
	             pitchCommand - rollCommand + yawCommand,
	             - pitchCommand - rollCommand - yawCommand,
	             - pitchCommand + rollCommand + yawCommand);

		// Clamping of height command
		if (heightFeedforward < 5000)
			heightFeedforward = 5000;
		float heightTotal = heightFeedforward + heightCommand;
		for (int i=0; i<4; i++) {
			if (RPM[i] + heightTotal > maxRPM) {
				heightTotal = maxRPM - RPM[i];
			}
		}

		// Total command
		RPM = RPM + (new Vector4 (heightTotal, heightTotal, heightTotal, heightTotal));

		// Clamp to max
		for (int i=0; i<4; i++) {
			if (RPM[i] < 0) {
				RPM[i] = 0;
			} else if (RPM[i] > maxRPM) {
				RPM[i] = maxRPM;
			}
		}

		RPM.Set (RPM[0]*motorStarted[0], RPM[1]*motorStarted[1], RPM[2]*motorStarted[2], RPM[3]*motorStarted[3]);

		ApplyForces ();
	}

	// Update is called once per frame
	void ApplyForces () {
		totalTorque = Vector3.zero;
		totalForce = Vector3.zero;
		for (int i=0;i<4;i++)
		{
			float propThrust = thrustModel.x*RPM[i]*RPM[i] + thrustModel.y*RPM[i] +  thrustModel.z;
			float propDrag = torqueModel.x*RPM[i]*RPM[i] + torqueModel.y*RPM[i] +  torqueModel.z;
			Vector3 propForce = propThrust*Vector3.up;
			Vector3 propTorque = propDrag*propellersCW[i]*Vector3.up + Vector3.Cross(propellersPositions[i], propForce);
			totalForce+=propForce;
			totalTorque+=propTorque;
		}
	}

	public float getPower() {
		return RPM.sqrMagnitude;
	}

	public float getMaxPower() {
		return 4*maxRPM*maxRPM;
	}

	public float getRPM(int i) {
		return RPM[i]*propellersCW[i];
	}

	public void stopMotor(int motor) {
//		motorStarted [motor] = 0;
		motorStarted = Vector4.zero;
	}

	public void startMotor(int motor) {
//		motorStarted [motor] = 1;
		motorStarted = Vector4.one;

	}

	public Vector3 getTotalForce() {
		return totalForce;
	}

	public Vector3 getTotalTorque() {
		return totalTorque;
	}
}
