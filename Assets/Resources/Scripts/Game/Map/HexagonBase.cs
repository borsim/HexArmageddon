using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HexagonBase : MonoBehaviour {

	public abstract void rotateCounterClockwise ();

	public abstract void rotateClockwise ();

	public abstract bool isHexagonPowered ();


	public HexagonField hexField;
	public GridPoint gridPoint;
}
