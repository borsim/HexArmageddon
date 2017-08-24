using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HexagonPower : HexagonBase {

	private int hexagonType;
	public Wire[] wires = new Wire[6];
	private int numActiveWires = 3;
	private int orientation = 0;
	const int rotationSpeed = 300;
	private Transform transform;

	GameObject activeLayer;
	GameObject inactiveLayer;

	void Start () {
		activeLayer = globalPrefabs.getChildGameObject (gameObject, "HexagonActive");
		inactiveLayer = globalPrefabs.getChildGameObject (gameObject, "HexagonInactive");

		hexagonType = UnityEngine.Random.Range (0, 15);
		currentRotation = 0.0f;
		targetRotation = 0.0f;
		transform = GetComponent<Transform> ();
		// 1-6 so that they all check for power on construction
		int offset = UnityEngine.Random.Range (1, 7);
		switch (hexagonType) {
			case 0:
			case 1:
				Wire.newWire (this, 0, 1);
				Wire.newWire (this, 2, 3);
				Wire.newWire (this, 4, 5);
				break;
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
				Wire.newWire (this, 0, 1);
				Wire.newWire (this, 2, 4);
				Wire.newWire (this, 3, 5);
				break;
			case 8:
			case 9:
			case 10:
				Wire.newWire (this, 0, 3);
				Wire.newWire (this, 1, 2);
				Wire.newWire (this, 4, 5);
				break;
			case 11:
			case 12:
			case 13:
				Wire.newWire (this, 0, 2);
				Wire.newWire (this, 1, 4);
				Wire.newWire (this, 3, 5);
				break;
			case 14:
				Wire.newWire (this, 0, 3);
				Wire.newWire (this, 1, 4);
				Wire.newWire (this, 2, 5);
				break;
		}
		for (int i = 0; i < offset; i++) {
			rotateClockwise ();
		}
	}
	
	void FixedUpdate () {
		
		float deltaRotation = 0;
		if (targetRotation > currentRotation) {
			deltaRotation = Mathf.Min (rotationSpeed * Time.deltaTime, targetRotation - currentRotation);
		}
		if (targetRotation < currentRotation) {
			deltaRotation = Mathf.Max(-rotationSpeed * Time.deltaTime, targetRotation - currentRotation);
		}
		transform.RotateAround (rotationPoint, Vector3.up, deltaRotation);
		currentRotation += deltaRotation;
		if (currentRotation < -180.0f) {
			currentRotation += 360.0f;
			targetRotation += 360.0f;
		}
		if (currentRotation > 180.0f) {
			currentRotation -= 360.0f;
			targetRotation -= 360.0f;
		}

		bool illuminate = isHexagonPowered ();
		if (activeLayer != null) {
			activeLayer.SetActive (illuminate);
		}
		if (inactiveLayer != null) {
			inactiveLayer.SetActive (!illuminate);
		}

		if (isTransitioning && !isRotating ()) {
			isTransitioning = false;
			transform.RotateAround (rotationPoint, Vector3.up, targetRotation - currentRotation);
			currentRotation = targetRotation;
			//transform.position = hexField.SpacePoint (gridPoint);
			if (hexField.getHexagon (hexToRotateNext.gridPoint) == hexToRotateNext) {
				hexField.removeHexagon (hexToRotateNext.gridPoint);
			}
		}
	}

	private float targetRotation;
	public float currentRotation{ get; private set; }
		

	public override void rotateCounterClockwise () {
		if (isRotating () && isTransitioning) {
			//Cannot rotate moving hexagon
			return;
		}
		try {
			rotationPoint = transform.position;
		} catch (NullReferenceException e) {
		}
		orientation += 1;
		if (orientation == 6) {
			orientation = 0;
		}
		targetRotation -= 60.0f;
	}

	public override void rotateClockwise () {
		if (isRotating () && isTransitioning) {
			//Cannot rotate moving hexagon
			return;
		}
		rotationPoint = transform.position;
		orientation -= 1;
		if (orientation == -1) {
			orientation = 5;
		}
		targetRotation += 60.0f;
	}

	public void depowerHexagon() {
		foreach (Wire wire in wires) {
			wire.setPowered (false);
		}
	}

	public override bool isHexagonPowered() {
		bool anyIsPowered = false;
		foreach (Wire wire in wires) {
			if (wire != null) {
				if (wire.isPowered ()) {
					anyIsPowered = true; 
				}
			}
		}
		return anyIsPowered;
	}

	public Wire getWireAtEntryPoint(int entry) {
		return wires [(entry - orientation + 18) % 6];
	}

	public int otherEndpointOfWire(int entry) {
		int e = (entry - orientation + 18) % 6;
		if (wires [e] == null) {
			return -12;
		}
		return (wires [e].otherEndpoint (e) + orientation) % 6;
	}

	public bool isTransitioning{ get; private set; }
	private Vector3 rotationPoint;
	private HexagonBase hexToRotateNext;

	public void TransitionTo(GridPoint target, GridPoint pointToMoveThrough, HexagonBase next) {
		if (isTransitioning) {
			return;
		}

		Vector3 t = hexField.SpacePoint (target);
		Vector3 g = hexField.SpacePoint (gridPoint);
		Vector3 m = hexField.SpacePoint (pointToMoveThrough);
		if ((t - g).sqrMagnitude > hexField.gridSize * hexField.gridSize * 2.9f || (m - g).sqrMagnitude > hexField.gridSize * hexField.gridSize * 2.9f) {
			throw new ArgumentException ("Target or moveThrough point not adjacent to hexagon");
		}
		if (Vector3.Cross(t - g, m - g).y < 0) {
			rotateClockwise();
			rotateClockwise();
		}
		else {
			rotateCounterClockwise ();
			rotateCounterClockwise ();
		}
		isTransitioning = true;
		rotationPoint = (t * 2.0f + g * 2.0f - m) / 3.0f;
		gridPoint = target;
		hexToRotateNext = next;
	}

	public bool isRotating() {
		return Mathf.Abs (targetRotation - currentRotation) > 5.0f;
	}

	public void Kill () {
		gameObject.SetActive(false);
		hexField.removeHexagon (gridPoint);
	}

}
