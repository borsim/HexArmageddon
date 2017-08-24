using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCableInteract : MonoBehaviour {

	HexagonField hexHandler;
	const float LETHAL_RANGE = 0.85f;

	void Start()
	{
		hexHandler = gameObject.transform.parent.parent.gameObject.GetComponent<HexagonField> ();
	}

	void Update () {
		if (checkElectrified ()) 
			electrify ();
	}

	bool checkElectrified() 
	{
		GridPoint g = hexHandler.closestGridPoint (new Vector2 (gameObject.transform.position.x, gameObject.transform.position.z));
		if (hexHandler.inRange (g)) 
		{
			HexagonBase nearestHex = hexHandler.getHexagon (g);
			if (nearestHex.isHexagonPowered () && inLethalRange (nearestHex))
				return true;
			else
				foreach (GridPoint gP in nearestHex.gridPoint.AdjacentPoints ())
					if (hexHandler.inRange (gP)) {
						HexagonBase hB = hexHandler.getHexagon (gP);
						if (hB.isHexagonPowered () && inLethalRange (hB))
							return true;
					}
		}
		return false;
	}

	bool inLethalRange (HexagonBase hB)
	{
		float x1 = hB.gameObject.transform.position.x;
		float y1 = hB.gameObject.transform.position.z;
		float x2 = gameObject.transform.position.x;
		float y2 = gameObject.transform.position.z;
		if ( (x1-x2)*(x1-x2)+(y1-y2)*(y1-y2) < LETHAL_RANGE*LETHAL_RANGE )
			return true;
		return false;
	}

	void electrify()
	{
		GridPoint nearestGridPoint = hexHandler.closestGridPoint( new Vector2 (gameObject.transform.position.x, gameObject.transform.position.z) );

		List<Wire> hitWires = new List<Wire>();

		HexagonBase nexHexBase = hexHandler.getHexagon (nearestGridPoint);
		if (nexHexBase is HexagonPower) { //ha vmi megrázza, akkor a egközelebbi hatszög biztosan hatótávon belül van
			foreach (Wire w in ((HexagonPower) nexHexBase).wires) {
				if (w != null) {
					if (w.powered)
						hitWires.Add (w);
				}
			}
		}
		
		foreach (GridPoint gPoint in nearestGridPoint.AdjacentPoints ()) {
			HexagonBase hexBase = hexHandler.getHexagon (gPoint);
			if (hexBase is HexagonPower) {
				HexagonPower hex = (HexagonPower)hexBase;
				if (inLethalRange (hexBase))
					foreach (Wire w in hex.wires)
						if (w != null) {
							if (w.powered)
								hitWires.Add (w);
						}
			}
		}
		
		int killerWireIndex = Random.Range(0, hitWires.Count);
		hitWires [killerWireIndex].Kill ();
		gameObject.GetComponent<ZombieScript> ().Die();
	}
}
