﻿using UnityEngine;
using System.Collections;



public class CameraControl : MonoBehaviour {

	public float edgeSensibility = 10f;

	public float cameraVelocity = 7;
	public float zoomVelocity = 50;
	public float rotVelocity = 5;
	public float currentZoom = 200;
	public float minZoom =20;
	public float maxZoom = 1300;

	Vector3 pos = new Vector3(0,0,0);
	Vector3 rot = new Vector3(45, 0, 0);
	Vector2 pre = new Vector2(-1,-1);
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			currentZoom += zoomVelocity;
			if(currentZoom > maxZoom) currentZoom = maxZoom;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			currentZoom -= zoomVelocity;
			if(currentZoom < minZoom) currentZoom = minZoom;
		}
		else if(Input.GetMouseButton(2)) {
			if(pre.y <=-1) pre.y = Input.mousePosition.y;
			rot.x -= (Input.mousePosition.y-pre.y)*rotVelocity;
			if(rot.x>90) rot.x =90;
			else if(rot.x<5) rot.x =5;
			pre.y = Input.mousePosition.y;
		}/*
		else if(Input.GetMouseButton(1)) {
			if(pre.x <=-1) pre.x = Input.mousePosition.x;
			rot.y -= (Input.mousePosition.x-pre.x)*rotVelocity;
			pre.x = Input.mousePosition.x;
		}*/
		else {
			if(Input.mousePosition.x <= edgeSensibility || Input.GetKey(KeyCode.A)){
				pos.x -= cameraVelocity*(currentZoom/100);
			}
			else if(Input.mousePosition.x >= Screen.width - edgeSensibility || Input.GetKey(KeyCode.D)){
				pos.x += cameraVelocity*(currentZoom/100);
			}
			if(Input.mousePosition.y <= edgeSensibility || Input.GetKey(KeyCode.S)) {
				pos.z -= cameraVelocity*(currentZoom/100);
			}
			else if(Input.mousePosition.y >= Screen.height - edgeSensibility || Input.GetKey(KeyCode.W)) {
				pos.z += cameraVelocity*(currentZoom/100);
			}
			pre.y = -1;
			pre.x = -1;
		}
		transform.rotation = Quaternion.Euler (rot);
		transform.position = new Vector3(pos.x, currentZoom*Mathf.Sin(Mathf.Deg2Rad*rot.x), 
		                                 pos.z-currentZoom* Mathf.Cos(Mathf.Deg2Rad*rot.x));
	}
}
