using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TelloLib;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

public class TelloController : SingletonMonoBehaviour<TelloController> {

	private static bool isLoaded = false;

	public GameObject drone;
	public float objectPosX;
	public float objectPosY;
	public float objectPosZ;

	private TelloVideoTexture telloVideoTexture;

	// FlipType is used for the various flips supported by the Tello.
	public enum FlipType
	{

		// FlipFront flips forward.
		FlipFront = 0,

		// FlipLeft flips left.
		FlipLeft = 1,

		// FlipBack flips backwards.
		FlipBack = 2,

		// FlipRight flips to the right.
		FlipRight = 3,

		// FlipForwardLeft flips forwards and to the left.
		FlipForwardLeft = 4,

		// FlipBackLeft flips backwards and to the left.
		FlipBackLeft = 5,

		// FlipBackRight flips backwards and to the right.
		FlipBackRight = 6,

		// FlipForwardRight flips forewards and to the right.
		FlipForwardRight = 7,
	};

	// VideoBitRate is used to set the bit rate for the streaming video returned by the Tello.
	public enum VideoBitRate
	{
		// VideoBitRateAuto sets the bitrate for streaming video to auto-adjust.
		VideoBitRateAuto = 0,

		// VideoBitRate1M sets the bitrate for streaming video to 1 Mb/s.
		VideoBitRate1M = 1,

		// VideoBitRate15M sets the bitrate for streaming video to 1.5 Mb/s
		VideoBitRate15M = 2,

		// VideoBitRate2M sets the bitrate for streaming video to 2 Mb/s.
		VideoBitRate2M = 3,

		// VideoBitRate3M sets the bitrate for streaming video to 3 Mb/s.
		VideoBitRate3M = 4,

		// VideoBitRate4M sets the bitrate for streaming video to 4 Mb/s.
		VideoBitRate4M = 5,

	};

	override protected void Awake()
	{
		if (!isLoaded) {
			DontDestroyOnLoad(this.gameObject);
			isLoaded = true;
		}
		base.Awake();

		Tello.onConnection += Tello_onConnection;
		Tello.onUpdate += Tello_onUpdate;
		Tello.onVideoData += Tello_onVideoData;

		if (telloVideoTexture == null)
			telloVideoTexture = FindObjectOfType<TelloVideoTexture>();

	}

	private void OnEnable()
	{
		if (telloVideoTexture == null)
			telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
	}

	private void Start()
	{
		if (telloVideoTexture == null)
			telloVideoTexture = FindObjectOfType<TelloVideoTexture>();

		Tello.startConnecting();

		objectPosX = drone.transform.position.x;
		objectPosY = drone.transform.position.y;
		objectPosZ = drone.transform.position.z;
	}

	void OnApplicationQuit()
	{
		Tello.stopConnecting();
	}

	// Update is called once per frame
	void Update () {

		var eular = Tello.state.toEuler();
		Debug.Log("percentage: " + Tello.state.batteryPercentage);

		double pitch = eular[0] * (180 / 3.141592);
		double roll = eular[1] * (180 / 3.141592);
		double yaw = eular[2] * (180 / 3.141592);

		// Debug.Log(" Pitch:" + pitch + " Roll:" + roll + " Yaw:" + yaw);
		// Debug.Log(" AR element yaw: " + drone.transform.localRotation.eulerAngles.y);

		Vector3 rotation = new Vector3((float) -eular[0], (float) eular[2], (float) eular[1]);
		// drone.transform.Rotate(rotation);
		drone.transform.eulerAngles = rotation;

		if (Input.GetKeyDown(KeyCode.T)) {
			Tello.takeOff();
			// Vector3 takeOff = new Vector3(-0.042f, -0.293f, 2);
			// drone.transform.position = takeOff;
		} else if (Input.GetKeyDown(KeyCode.L)) {
			Tello.land();
		}

		float lx = 0f;
		float ly = 0f;
		float rx = 0f;
		float ry = 0f;

		if (drone.transform.position.x != objectPosX){
			Debug.Log("Pos X difference:" + (objectPosX - drone.transform.position.x));
			rx = (objectPosX - drone.transform.position.x) * 500;
			objectPosX = drone.transform.position.x;
		}

		if (drone.transform.position.z != objectPosZ){
			Debug.Log("Pos Z difference:" + (objectPosZ - drone.transform.position.z));
			ry = (objectPosZ - drone.transform.position.z) * 500;
			objectPosZ = drone.transform.position.z;
		}

		if (Input.GetKey(KeyCode.U)) {
			ry = 1;
		}
		if (Input.GetKey(KeyCode.J)) {
			ry = -1;
		}
		if (Input.GetKey(KeyCode.K)) {
			rx = 1;
		}
		if (Input.GetKey(KeyCode.H)) {
			rx = -1;
		}
		if (Input.GetKey(KeyCode.W)) {
			ly = 1;
		}
		if (Input.GetKey(KeyCode.S)) {
			ly = -1;
		}
		if (Input.GetKey(KeyCode.D)) {
			lx = 1;
		}
		if (Input.GetKey(KeyCode.A)) {
			lx = -1;
		}
		Tello.controllerState.setAxis(lx, ly, rx, ry);

	}

	private void Tello_onUpdate(int cmdId)
	{
		//throw new System.NotImplementedException();
		//Debug.Log("Tello_onUpdate : " + Tello.state);
    }

    private void Tello_onConnection(Tello.ConnectionState newState)
	{
		//throw new System.NotImplementedException();
		//Debug.Log("Tello_onConnection : " + newState);
		if (newState == Tello.ConnectionState.Connected) {
            Tello.queryAttAngle();
            Tello.setMaxHeight(50);

			Tello.setPicVidMode(1); // 0: picture, 1: video
			Tello.setVideoBitRate((int)VideoBitRate.VideoBitRateAuto);
			//Tello.setEV(0);
			Tello.requestIframe();
		}
	}

	private void Tello_onVideoData(byte[] data)
	{
		//Debug.Log("Tello_onVideoData: " + data.Length);
		if (telloVideoTexture != null)
			telloVideoTexture.PutVideoData(data);
	}

	public void Takeoff()
    {
		Tello.takeOff();
	}

	public void Land()
    {
		Tello.land();
	}

	public void Tello_onMove()
    {

	}

}
