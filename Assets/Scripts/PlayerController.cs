﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float mechSpeed;

	public float carSpeed;
	public float carTurnSpeed;

	public GameObject mech;
	public GameObject car;
	public GameObject activeGameObject { get; private set;}
	public static int MODE = 1;

	// Use this for initialization
	void Start () {
		this.activeGameObject = mech;
	}
	
	// Update is called once per frame
	void Update () { 
		float delta = Time.deltaTime;

		if (Input.GetKeyDown (KeyCode.R)) {
			MODE = 1;
			Time.timeScale = 1;
			CameraController.SCORE = 0;
			SceneManager.LoadScene (0);

		}

		if (Time.timeScale != 0) {
			this.UpdateMode ();
		}

		bool up = Input.GetKey(KeyCode.W);
		bool down = Input.GetKey(KeyCode.S);
		bool left = Input.GetKey(KeyCode.A);
		bool right = Input.GetKey(KeyCode.D);

		if (MODE == 1) {
			this.UpdateMech (delta, up, down, left, right);
		} else {
			this.CarMovement (delta, up, down, left, right);
		}
	}

	void UpdateMode() {
		if (Input.GetKeyDown (KeyCode.E) && MODE != 0) {
			MODE = 0;
			this.car.SetActive (true);
			this.mech.SetActive (false);
			this.car.transform.position = this.activeGameObject.transform.position;
			this.activeGameObject = this.car;
			this.car.transform.rotation = this.GetMechTransform ("Lower").rotation;
			this.car.transform.Rotate(new Vector3(0,0,-90));

		} else if (Input.GetKeyDown (KeyCode.Q) && MODE != 1) {
			MODE = 1;
			this.car.SetActive (false);
			this.mech.SetActive (true);
			this.mech.transform.position = this.activeGameObject.transform.position;
			this.GetMechTransform("Lower").rotation = this.car.transform.rotation;
			this.GetMechTransform("Lower").Rotate(new Vector3(0,0,90));
			this.activeGameObject = this.mech;
		}

	}

	void CarMovement(float delta, bool up, bool down, bool left, bool right) {
		Rigidbody2D body = this.activeGameObject.GetComponent<Rigidbody2D> ();
		Transform transform = this.activeGameObject.transform;

		if (up) {
			body.AddRelativeForce (new Vector2 (-carSpeed * delta, 0));
		}
		if (down) {
			body.AddRelativeForce (new Vector2 (carSpeed * delta, 0));
		}
		if (left) {
			body.AddTorque (carSpeed * delta);
		}
		if (right) {
			body.AddTorque (-carSpeed * delta);
		}
	}

	// valid inputs are "Torso" or "Lower"
	private Transform GetMechTransform(string name) {
		foreach(Transform t in this.mech.GetComponentsInChildren<Transform> ()) {
			if(name.Equals(t.name)) {
				return t;
			}
		}
		throw new System.ArgumentException ();
	}

	void UpdateMech(float delta, bool up, bool down, bool left, bool right) {
		Transform torsoTransform = GetMechTransform("Torso");
		Transform lowerTransform= GetMechTransform("Lower");

		int horizontal = 0;
		int vertical = 0;


		if (Input.GetKey(KeyCode.W)) {
			vertical++;
		}
		if (Input.GetKey(KeyCode.S)) {
			vertical--;
		}
		if(Input.GetKey(KeyCode.A)) {
			horizontal--;
		}
		if(Input.GetKey(KeyCode.D)) {
			horizontal++;
		}
		float distance = mechSpeed * delta;

		Vector2 movement = new Vector2 (horizontal, vertical);
		movement.Normalize ();
		movement.Scale (new Vector2(distance, distance));
		this.activeGameObject.transform.Translate (movement);


		// rotate torso towards mouse 
		Vector2 mouse = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		float width = (mouse.x) - this.activeGameObject.transform.position.x;
		float height = (mouse.y) - this.activeGameObject.transform.position.y;

		float angle = Mathf.Atan2 (height, width) * Mathf.Rad2Deg;

		// get component(s)InChildren also searches parent, tehrefore in this case the second Component is needed
		torsoTransform.rotation = Quaternion.Euler(new Vector3(0,0,angle-90));


		//rotate lower towards move direction
		if (!(horizontal == 0 && vertical == 0)) {
			width = horizontal;
			height = vertical;
			angle = Mathf.Atan2 (height, width) * Mathf.Rad2Deg;
			lowerTransform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, angle - 90));
		}
	}

}
