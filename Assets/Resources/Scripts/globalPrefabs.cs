using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;   //Required for Dictionary

public class globalPrefabs : ScriptableObject
{
	public static Dictionary<string, GameObject> prefabList = new Dictionary<String, GameObject>();
	public static Dictionary<string, Material> materialList = new Dictionary<String, Material>();
	private static bool loaded = false;

	public static void LoadAll() 
	{
		loaded = true;

		UnityEngine.Object[] ObjectArray = Resources.LoadAll ("Prefabs");

		foreach (UnityEngine.Object o in ObjectArray) {
			prefabList.Add (o.name, (GameObject)o);
		}


		ObjectArray = Resources.LoadAll ("Materials", typeof(Material));

		foreach (UnityEngine.Object o in ObjectArray)
			materialList.Add (o.name, (Material)o);
	}

	public static GameObject getPrefab(string objName)
	{
		if (!loaded) {
			LoadAll ();
		}

		GameObject obj;

		if (prefabList.TryGetValue (objName, out obj)) {
			return obj;
		}
		else 
		{
			Debug.Log("Object not found " + objName);
			return (null);
		}
	}

	public static Material getMaterial(string objName)
	{
		if (!loaded) {
			LoadAll ();
		}

		Material obj;

		if (materialList.TryGetValue (objName, out obj)) {
			return obj;
		}
		else 
		{
			Debug.Log("Object not found " + objName);
			return (null);
		}
	}

	static public GameObject getChildGameObject(GameObject fromGameObject, string withName) {
		//Author: Isaac Dart, June-13.
		Transform[] ts = fromGameObject.transform.GetComponentsInChildren<Transform>();
		foreach (Transform t in ts) if (t.gameObject.name == withName) return t.gameObject;
		return null;
	}


}