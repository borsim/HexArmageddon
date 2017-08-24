using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoreScript : HexagonBase {

	int outputDirection = 0;
	const int rotationSpeed = 300;

	// Use this for initialization
	void Start () {
		hexField = GetComponentInParent<HexagonField> ();
	}
	
	// Update is called once per frame
	void Update () {
		emitPower ();
	}

	private List<Wire> poweredWires = new List<Wire> ();

	public void emitPower() {
		foreach (Wire w in poweredWires) {
			w.setPowered (false);
		}
		poweredWires.Clear ();
		if (Mathf.Abs (targetRotation - currentRotation) < 5.0f) {
			emitPowerRecursion (gridPoint, outputDirection);
		}
	}

	void emitPowerRecursion(GridPoint currentGridPoint, int powerEntryPoint) {
		GridPoint nextGridPoint = currentGridPoint.AdjacentPointsCircular () [powerEntryPoint + 3];
		if (hexField.inRange (nextGridPoint)) {
			HexagonBase hexBase = hexField.getHexagon (nextGridPoint);
			if (hexBase is HexagonPower) {
				HexagonPower nextHexPower = (HexagonPower)hexBase;
				Wire activeWire = nextHexPower.getWireAtEntryPoint (powerEntryPoint);
				if ((activeWire != null) && !nextHexPower.isRotating()) {
					activeWire.setPowered(true);
					int nextPowerEntryPoint = nextHexPower.otherEndpointOfWire (powerEntryPoint);
					poweredWires.Add (activeWire);
					emitPowerRecursion (nextGridPoint, ((nextPowerEntryPoint + 9) % 6));
				}
			}
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
		transform.RotateAround (transform.position, Vector3.up, deltaRotation);
		currentRotation += deltaRotation;
		if (currentRotation < -180.0f) {
			currentRotation += 360.0f;
			targetRotation += 360.0f;
		}
		if (currentRotation > 180.0f) {
			currentRotation -= 360.0f;
			targetRotation -= 360.0f;
		}
	}

	private float targetRotation;
	public float currentRotation{ get; private set; }

	public override void rotateCounterClockwise ()
	{
		outputDirection += 1;
		if (outputDirection == 6) {
			outputDirection = 0;
		}
		targetRotation -= 60.0f;
	}

	public override void rotateClockwise ()
	{
		outputDirection -= 1;
		if (outputDirection == -1) {
			outputDirection = 5;
		}
		targetRotation += 60.0f;
	}



	public override bool isHexagonPowered() {
		return true;
	}

}
