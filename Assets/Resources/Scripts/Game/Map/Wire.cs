using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour {

	public int src { get; private set; }
	public int dst { get; private set; }
	public bool powered { get; private set; }
	public HexagonPower hexagon{ get; private set; }

	private static bool prefabsInitialized = false;
	static GameObject prefab0;
	static GameObject prefab1;
	static GameObject prefab2;

	GameObject poweredWire;
	GameObject unpoweredWire;
	GameObject hoverWire;

	public void Awake() {
		poweredWire = globalPrefabs.getChildGameObject (gameObject, "WireActive");
		unpoweredWire = globalPrefabs.getChildGameObject (gameObject, "WireInactive");
		hoverWire = globalPrefabs.getChildGameObject (gameObject, "WireWannaBActive");
	}

	public bool isPowered() {
		return powered;
	}

	public void setPowered(bool pow) {
		powered = pow;
		if (poweredWire != null) {
			poweredWire.SetActive (pow);
		}
		if (unpoweredWire != null) {
			unpoweredWire.SetActive (!pow && !hover);
		}
		if (hoverWire != null) {
			hoverWire.SetActive (hover && !pow);
		}
	}

	public static Wire newWire(HexagonPower hex, int sr, int ds, bool pow = false) {
		int src = sr;
		int dst = ds;
		if ((sr - ds + 6) % 6 > (ds - sr + 6) % 6) {
			src = ds;
			dst = sr;
		}
		if (!prefabsInitialized) {
			prefab0 = globalPrefabs.getPrefab ("WirePrefab0");
			prefab1 = globalPrefabs.getPrefab ("WirePrefab1");
			prefab2 = globalPrefabs.getPrefab ("WirePrefab2");
		}
		GameObject go;
		switch ((src - dst + 6) % 6) {
			case 1:
				go = (GameObject)Instantiate (prefab0, hex.transform.position, Quaternion.identity);
				break;
			case 2:
				go = (GameObject)Instantiate (prefab1, hex.transform.position, Quaternion.identity);
				break;
			case 3:
				go = (GameObject)Instantiate (prefab2, hex.transform.position, Quaternion.identity);
				break;
			default:
				throw new UnityException ("Invalid wire");
		}
		go.transform.RotateAround (hex.transform.position, Vector3.up, hex.currentRotation - 60.0f * (float)src);
		go.transform.parent = hex.transform;
		Wire res = go.GetComponent<Wire> ();
		res.src = src;
		res.dst = dst;
		res.hexagon = hex;
		if ((hex != null)) {
			hex.wires [src] = res;
			hex.wires [dst] = res;
		}
		res.setPowered (pow);
		return res;
	}

	public int otherEndpoint(int end) {
		if (src == end) {
			return dst;
		}
		if (dst == end) {
			return src;
		}
		throw new ArgumentException ("No such end " + end + "of wire between " + src + " and " + dst);
	}


	public void Kill() {
		setPowered (false);
		if (hexagon != null) {
			if (hexagon.wires [src] == this) {
				hexagon.wires [src] = null;
			}
			if (hexagon.wires [dst] == this) {
				hexagon.wires [dst] = null;
			} 
			bool allWiresDead = true;
			foreach (Wire w in hexagon.wires) {
				if (w != null) {
					allWiresDead = false;
					break;
				}
			}
			if (allWiresDead) {
				hexagon.Kill ();
			}
		}
		gameObject.SetActive (false);
	}

	public bool hover{ get; private set; }

	public void setHover(bool isHover) {
		hover = isHover;
		if (poweredWire != null) {
			poweredWire.SetActive (powered);
		}
		if (unpoweredWire != null) {
			unpoweredWire.SetActive (!powered && !hover);
		}
		if (hoverWire != null) {
			hoverWire.SetActive (hover && !powered);
		}
	}

}
