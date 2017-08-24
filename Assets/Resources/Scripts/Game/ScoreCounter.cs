using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour {
	private int zombieScore = 0;
	private float startTime;
	private const int scorePerSecond = 10;
	private const int scorePerZombie = 100;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		PlayerPrefs.SetInt ("Score", getCurrentScore ());
	}

	public int getCurrentScore () {
		int timeScore = (int) (Time.time - startTime) * scorePerSecond;
		return timeScore + zombieScore;
	}

	public void zombieKilled () {
		zombieScore += scorePerZombie;
	}
}
