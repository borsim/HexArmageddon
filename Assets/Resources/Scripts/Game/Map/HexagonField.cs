using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonField : MonoBehaviour {

	public float gridSize = 1.0f;
	public Rect Field = new Rect(-12, -6, 24, 12); //map size

	GameObject map;
	GameObject hexagon;
	GameObject hexagonCore;
	GameObject hexagonDummy;
	GameObject hexagonDummyTrans;
	Material hexagonDefaultMaterial;
	Material hexagonHoverMaterial;
	bool leftClickHappened = false;
	bool rightClickHappened = false;

	int clickableMask;
	float camRayLength = 2000f; 
	GridPoint actDot = new GridPoint(0, 0);
	const float psqrt3 = 0.577350269189625764509148780501957455647601751270126876018f; // 1/sqrt(3) from WolframAlpha

	// sqrt(3)/2 from wolframAlpha
	private const float sqrt3p2 = 0.866025403784438646763723170752936183471402626905190314027f;

	HexagonBase[][] grid;

	void Awake () {
		hexagon = globalPrefabs.getPrefab("Hexagon");
		hexagonCore = globalPrefabs.getPrefab("HexCore");
		hexagonDummy = globalPrefabs.getPrefab ("HexDummy");
		hexagonDummyTrans = globalPrefabs.getPrefab ("HexDummyInTransition");
		hexagonDefaultMaterial = globalPrefabs.getMaterial ("HexagonDefaultMaterial");
		hexagonHoverMaterial = globalPrefabs.getMaterial ("HexagonHoverMaterial");
		clickableMask = LayerMask.GetMask("Clickable");

		map = new GameObject ();
		map.name = "Map";
		map.transform.SetParent (this.transform);

		grid = new HexagonBase[101][];
		for (int i = -50; i < 50; ++i) {
			grid [i + 50] = new HexagonBase[101];
			for (int j = -50; j < 50; ++j) {
				if ((i != 0) || (j != 0)) {
					GridPoint gp = new GridPoint (i, j);
					if (inRange (gp)) {
						grid [i + 50] [j + 50] = ((GameObject)Instantiate (hexagon, SpacePoint (gp), Quaternion.identity)).GetComponent<HexagonBase> ();
						grid [i + 50] [j + 50].transform.SetParent (map.transform);
						grid [i + 50] [j + 50].hexField = this;
						grid [i + 50] [j + 50].gridPoint = gp;
					}
				}
			}
		}
		GridPoint origin = new GridPoint(0, 0);
		grid [50] [50] = ((GameObject)Instantiate (hexagonCore, SpacePoint (origin), Quaternion.identity)).GetComponent<HexCoreScript> ();
		grid [50] [50].transform.SetParent (map.transform);
		grid [50] [50].gridPoint = origin;
	}

	List<Wire> hoverWires = new List<Wire>();

	void hoverWireRecursion(GridPoint currentGridPoint, int powerEntryPoint) {
		GridPoint nextGridPoint = currentGridPoint.AdjacentPointsCircular () [powerEntryPoint + 3];
		if (inRange (nextGridPoint)) {
			HexagonBase hexBase = getHexagon (nextGridPoint);
			if (hexBase is HexagonPower) {
				HexagonPower nextHexPower = (HexagonPower)hexBase;
				Wire activeWire = nextHexPower.getWireAtEntryPoint (powerEntryPoint);
				if ((activeWire != null) && !nextHexPower.isRotating() && !activeWire.hover) {
					activeWire.setHover (true);
					int nextPowerEntryPoint = nextHexPower.otherEndpointOfWire (powerEntryPoint);
					hoverWires.Add (activeWire);
					hoverWireRecursion (nextGridPoint, (nextPowerEntryPoint + 3) % 6);
				}
			}
		}
	}

	private readonly Vector3 corner34 = new Vector3 (sqrt3p2, 0, 0.5f);
	private readonly Vector3 corner45 = new Vector3 (sqrt3p2, 0, -0.5f);

	// Update is called once per frame
	void Update () {
		Vector2 mousexy = Input.mousePosition;
		Ray camRay = GetComponentInChildren<Camera>().ScreenPointToRay(mousexy);
		RaycastHit floorHit;
		Vector2 mousepos = new Vector2();
		foreach (Wire w in hoverWires) {
			w.setHover (false);
		}
		hoverWires.Clear ();
		if (Physics.Raycast (camRay, out floorHit, camRayLength, clickableMask)) {
			mousepos.x = floorHit.point.x;
			mousepos.y = floorHit.point.z;
			if (inRange (closestGridPoint (mousepos))) {
				actDot = closestGridPoint (mousepos);
				HexagonBase actHexagon = getHexagon (actDot);

				float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
				if (scrollWheel < 0) {
					actHexagon.rotateClockwise ();
				} else if (scrollWheel > 0) {
					actHexagon.rotateCounterClockwise ();
				}

				if (actHexagon is HexagonPower) {
					HexagonPower hex = (HexagonPower)actHexagon;
					Vector3 deltaVec = floorHit.point - SpacePoint (actDot);
					int selectedWireEnd;
					if (Vector3.Cross (deltaVec, Vector3.forward).y < 0) {
						//right half
						if (Vector3.Cross (deltaVec, corner34).y < 0) {
							//2&3
							if (Vector3.Cross (deltaVec, corner45).y < 0) {
								selectedWireEnd = 2;
							} else {
								selectedWireEnd = 3;
							}
						} else {
							selectedWireEnd = 4;
						}
					} else {
						if (Vector3.Cross (deltaVec, corner34).y < 0) {
							selectedWireEnd = 1;
						} else if (Vector3.Cross (deltaVec, corner45).y < 0) {
							selectedWireEnd = 0;
						} else {
							selectedWireEnd = 5;
						}
					}
					//selectedWireEnd = Mathf.FloorToInt (
					//	                     13.0f - Mathf.Atan2 (-0.5f * deltaVec.x + sqrt3p2 * deltaVec.z, -sqrt3p2 * deltaVec.x - 0.5f * deltaVec.z) * 3.0f / Mathf.PI) % 6;
					Wire currentWire = hex.getWireAtEntryPoint (selectedWireEnd);
					if (currentWire) {
						currentWire.setHover (true);
						hoverWires.Add (currentWire);
					}
					hoverWireRecursion (actDot, (selectedWireEnd+3)%6);
					int opposite = hex.otherEndpointOfWire (selectedWireEnd);
					hoverWireRecursion (actDot, (opposite+3)%6);
				}
						
				if (Input.GetMouseButtonDown (0)) {
					leftClickHappened = true;
				}
				if (Input.GetMouseButtonDown (1)) {
					rightClickHappened = true;
				}

				if (leftClickHappened && (actHexagon is HexagonPower || actHexagon is HexCoreScript)) {
					actHexagon.rotateCounterClockwise ();
					leftClickHappened = false;
				}
					
				if (rightClickHappened && (actHexagon is HexagonPower || actHexagon is HexCoreScript)) {
					actHexagon.rotateClockwise ();
					rightClickHappened = false;
				}
			}
		}
	}

	public Vector2 PlainPoint(GridPoint gridPoint)
	{
		return gridSize * gridPoint.RawPlainPoint ();
	}

	public Vector3 SpacePoint(GridPoint gridPoint)
	{
		Vector2 raw = gridPoint.RawPlainPoint ();
		return new Vector3 (gridSize * raw.x, 0, gridSize * raw.y);
	}

	public GridPoint closestGridPoint(Vector2 plainPoint)
	{
		GridPoint res = new GridPoint(Mathf.RoundToInt(plainPoint.x - psqrt3 * plainPoint.y), 
			Mathf.RoundToInt(plainPoint.y * 2.0f * psqrt3));
		foreach (GridPoint nei in res.AdjacentPoints ()) {
			if ((PlainPoint (nei) - plainPoint).sqrMagnitude < (PlainPoint (res) - plainPoint).sqrMagnitude) {
				res = nei;
			}
		}
		return res;
	}

	public bool inRange(GridPoint gridPoint)
	{
		return Field.Contains(PlainPoint(gridPoint));
	}

	public bool areAdjacent(GridPoint a, GridPoint b)
	{
		return a.isAdjacentTo (b);
	}

	public HexagonBase getHexagon(GridPoint g) {
		return grid [g.u + 50] [g.v + 50];
	}

	public HexagonBase getHexagon(int u, int v) {
		return grid [u + 50] [v + 50];
	}

	public void removeHexagon(GridPoint g) {
		HexagonBase hb = getHexagon (g);
		if (hb != null) {
			hb.gameObject.SetActive (false);
		}
		grid [g.u + 50] [g.v + 50] = ((GameObject)Instantiate (hexagonDummy, SpacePoint (g), Quaternion.identity)).GetComponent<HexagonDummy> ();
		grid [g.u + 50] [g.v + 50].gridPoint = g;
		grid [g.u + 50] [g.v + 50].hexField = this;
		GridPoint[] neis = g.AdjacentPointsCircular ();
		for (int i = 1; i <= 6; ++i) {
			if ((!inRange (neis [i])) || (!inRange (neis [i - 1])) || (!inRange (neis [i + 1]))) {
				HexagonBase newHex = ((GameObject)Instantiate (hexagon, SpacePoint (g), Quaternion.identity)).GetComponent<HexagonPower> ();
				grid [g.u + 50] [g.v + 50].gameObject.SetActive (false);
				grid [g.u + 50] [g.v + 50] = newHex;
				newHex.transform.SetParent (map.transform);
				newHex.hexField = this;
				newHex.gridPoint = g;
				break;
			}
			if (getHexagon (neis [i]) is HexagonDummy) {
				getHexagon (neis [i]).gameObject.SetActive (false);
				GridPoint gpToRotate = neis [i - 1];
				if (neis [i - 1].RawPlainPoint ().SqrMagnitude () < neis [i + 1].RawPlainPoint ().SqrMagnitude ()) {
					gpToRotate = neis [i + 1];
				}
				HexagonBase hexToRotate = getHexagon (gpToRotate);
				if (hexToRotate is HexagonPower && (!((HexagonPower)hexToRotate).isRotating ())) {
					if (!hexToRotate.gridPoint.Equals(gpToRotate)) {
						throw new MissingComponentException ("Hexagon is not at its grid point "+gpToRotate.ToString()+" but instead at "+hexToRotate.gridPoint);
					}
					HexagonBase hd = ((GameObject)Instantiate (hexagonDummyTrans, SpacePoint (gpToRotate), Quaternion.identity)).GetComponent<HexagonBase> ();
					hd.gridPoint = gpToRotate;
					hd.hexField = this;
					((HexagonPower)hexToRotate).TransitionTo (neis [i], g, hd);
					grid [neis [i].u + 50] [neis [i].v + 50] = hexToRotate;
					grid [gpToRotate.u + 50] [gpToRotate.v + 50] = hd;
					break;
				}
			}
		}
	}
}
