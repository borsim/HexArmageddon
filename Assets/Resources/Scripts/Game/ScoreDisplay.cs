using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {

	private Text scoreText;
	private ScoreCounter scoreTracker;
	// Use this for initialization
	void Start () {
		scoreText = GetComponentInParent <Text> ();
		scoreTracker = GetComponentInParent <ScoreCounter> ();
	}

	// Update is called once per frame
	void Update () {
		scoreText.text = "Score: " + scoreTracker.getCurrentScore ();
	}
}
