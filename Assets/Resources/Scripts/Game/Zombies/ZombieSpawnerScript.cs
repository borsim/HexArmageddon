using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawnerScript : MonoBehaviour {

	private float elapsedTime = 0;
	private float SPAWN_THRESHOLD = 0.3f;
	float spawnCircleRadius = 5;

	int wave=0;
	int leftFromWave;
	float TIME_BETWEEN_WAVES=15f;
	bool waving=false;

	GameObject zombies; //zombie container

	void Start () {
		zombies = new GameObject ();
		zombies.name = "Zombies";
		zombies.transform.SetParent (this.transform);
		//newWave ();
	}

	void Update () 
	{
		elapsedTime += Time.deltaTime;
		if (waving) {
			if (elapsedTime > SPAWN_THRESHOLD) {
				spawnZombie ();
				elapsedTime = 0;
			}
		} else if (elapsedTime > TIME_BETWEEN_WAVES) {
			newWave ();
			elapsedTime = 0;
		}
	}

	void newWave()
	{
		leftFromWave = (int) Mathf.Sqrt (wave) + 3;
		waving = true;
	}

	void endWave()
	{
		waving = false;
		wave++;
	}

	void spawnZombie() {
		float angle = Random.value * Mathf.PI * 2;
		float spawnX = spawnCircleRadius * Mathf.Cos (angle);
		float spawnZ = spawnCircleRadius * Mathf.Sin (angle);
		Vector3 spawnLocation = new Vector3 (spawnX, 0.3f, spawnZ); 
		var newZombie = Instantiate(globalPrefabs.getPrefab("Zombie"), spawnLocation, 
			Quaternion.identity);
		newZombie.transform.SetParent (zombies.transform);

		leftFromWave--;
		if (leftFromWave == 0)
			endWave ();
	}
}
