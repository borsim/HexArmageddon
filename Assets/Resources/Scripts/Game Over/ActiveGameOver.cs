using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGameOver : MonoBehaviour {

	public GameObject zombies;
	public float rotationSpeed = 15f;
	public float pulsingSpeed = 0.3f;
	private bool growing = true;
	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		zombies.transform.Rotate (new Vector3(0, rotationSpeed * Time.deltaTime, 0));
		Vector3 xyScale = zombies.transform.localScale;
		if (xyScale.x > 2.5)
			growing = false;
		if (xyScale.x < 1.75)
			growing = true;
		if (growing) {
			xyScale = new Vector3 (xyScale.x + pulsingSpeed * Time.deltaTime, 1,
					xyScale.x + pulsingSpeed*Time.deltaTime);
			zombies.transform.localScale = xyScale;
		} else {
				xyScale = new Vector3 (xyScale.x - pulsingSpeed * Time.deltaTime, 1,
					xyScale.x - pulsingSpeed*Time.deltaTime);
			zombies.transform.localScale = xyScale;
		}
	}
}
