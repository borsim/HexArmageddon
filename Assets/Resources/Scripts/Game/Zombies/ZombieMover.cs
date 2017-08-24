using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieMover : MonoBehaviour {

	private Vector3 corePosition;		// Core the zombie is trying to destroy
	private Vector3 direction;
	private float speed;

	void Awake ()
	{
		corePosition = GameObject.FindGameObjectWithTag ("Core").transform.position;
		Vector3 diff = corePosition - gameObject.transform.position;
		diff.y = 0;
		direction = diff.normalized;
		speed = 0.5f - Random.Range (0f, 0.3f);
	}

	// Update is called once per frame
	void Update () {
		Vector3 movementVector = direction * speed * Time.deltaTime;
		gameObject.transform.position += movementVector;
		if (getDistanceToCore () <= 0) {
			SceneManager.LoadScene("GameOver");
		}
	}

	private float getDistanceToCore(){
		Vector3 diff = corePosition - gameObject.transform.position;
		return Vector3.Dot (diff, direction);
	}
}
