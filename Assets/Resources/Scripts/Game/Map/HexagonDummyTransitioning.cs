using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonDummyTransitioning : HexagonBase {



	public override bool isHexagonPowered ()
	{
		return false;
	}

	public override void rotateClockwise ()
	{
		// Dummy hexagons don't rotate
	}

	public override void rotateCounterClockwise ()
	{
		// Dummy hexagons don't rotate
	}

}
