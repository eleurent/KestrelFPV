using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonView))]
public class NetworkDrone : Photon.MonoBehaviour
{
	public float KpPosition;
	public float KdSpeed;
	public float KpAngle;
	public float KdRate;

	private Vector3 lastPosition;
	private Vector3 lastVelocity;
	private Vector3 lastAcceleration;
	private Quaternion lastRotation;
	private Vector3 lastAngularVelocity;
	private float lastPower;

	private Vector3 estPosition;
	private Vector3 estVelocity;
	
	private double lastUpdateTimestamp;

	private static float PROPAGATION_TIMEOUT = 2.0f;
	private static float RESET_DISTANCE = 1.0f;
	
	public void Awake()
	{
		if (photonView.isMine)
		{
			this.enabled = false; // due to this, Update() is not called on the owner client.
		}
		
		lastPosition = transform.position;
		lastVelocity = Vector3.zero;
		lastAcceleration = Vector3.zero;
		lastRotation = transform.rotation;
		lastAngularVelocity = Vector3.zero;
		estPosition = lastPosition;
		estVelocity = lastVelocity;
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 position = transform.localPosition;
			Vector3 velocity = GetComponent<Rigidbody>().velocity;
			Vector3 acceleration = GetComponent<ControllerManager>().getAcceleration();
			Quaternion rotation = transform.localRotation;
			Vector3 angularVelocity = GetComponent<Rigidbody>().angularVelocity;
			float power = GetComponent<PropellersController>().getPower();
			stream.Serialize(ref position);
			stream.Serialize(ref velocity);
			stream.Serialize(ref acceleration);
			stream.Serialize(ref rotation);
			stream.Serialize(ref angularVelocity);
			stream.Serialize(ref power);
		}
		else
		{
			// Receive latest state information
			Vector3 position = Vector3.zero;
			Vector3 velocity = Vector3.zero;
			Vector3 acceleration = Vector3.zero;
			Quaternion rotation = Quaternion.identity;
			Vector3 angularVelocity = Vector3.zero;
			float power = 0;
			
			stream.Serialize(ref position);
			stream.Serialize(ref velocity);
			stream.Serialize(ref acceleration);
			stream.Serialize(ref rotation);
			stream.Serialize(ref angularVelocity);
			stream.Serialize(ref power);

			lastPosition = position;
			lastVelocity = velocity;
			lastAcceleration = acceleration;
			lastRotation = rotation;
			lastAngularVelocity = angularVelocity;
			lastPower = power;

			estPosition = lastPosition;
			estVelocity = lastVelocity;

			lastUpdateTimestamp = info.timestamp;
		}
	}
	
	public void FixedUpdate()
	{
		if (!photonView.isMine) {
			// If drone is too far from estimate, reset its state
			bool isTooFar = Vector3.Distance(estPosition, transform.position) > RESET_DISTANCE + lastVelocity.magnitude*PhotonNetwork.GetPing()*0.001f;
			if  (isTooFar) {
				transform.position = estPosition;
				GetComponent<Rigidbody> ().velocity = estVelocity;
				transform.rotation = lastRotation;
				GetComponent<Rigidbody> ().angularVelocity = lastAngularVelocity;
			}
			// If last update is too old, reset estimate on last update
			bool isTooOld = PhotonNetwork.time - lastUpdateTimestamp > PROPAGATION_TIMEOUT;
			if (isTooOld) {
				estPosition = lastPosition;
				estVelocity = lastVelocity;
			}
			// Position loop
			estVelocity += lastAcceleration*Time.deltaTime;
			estPosition += estVelocity*Time.deltaTime;
			Vector3 accelerationCommand = lastAcceleration
										+ KpPosition*(estPosition - transform.position)
										+ KdSpeed*(estVelocity - GetComponent<Rigidbody> ().velocity);
			GetComponent<Rigidbody> ().AddForce(accelerationCommand, ForceMode.Acceleration);

			// Angle loop
			Quaternion quaternionError = lastRotation*Quaternion.Inverse (transform.rotation);
			Vector3 axis = Vector3.zero; float angleError = 0; quaternionError.ToAngleAxis (out angleError, out axis);
			if (double.IsInfinity(axis.magnitude)) axis = Vector3.zero;
			Vector3 torqueCommand = KpAngle*angleError*Mathf.Deg2Rad*axis
				                  + KdRate*(lastAngularVelocity - GetComponent<Rigidbody> ().angularVelocity);
			GetComponent<Rigidbody> ().AddTorque(torqueCommand, ForceMode.Acceleration);

			// Propeller audio
			// Set default motor commands for propeller animation
			GetComponent<PropellersController> ().MixRPM (Mathf.Sqrt(lastPower)/4, 0, 0, 0, 0);
		}
	}
}
