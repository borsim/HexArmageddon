using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour {

	public void Die()
	{
		gameObject.transform.parent.parent.gameObject.GetComponent<ScoreCounter> ().zombieKilled ();
		Destroy (gameObject);
	}
}
