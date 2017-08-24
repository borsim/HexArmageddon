using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverHandler : MonoBehaviour {
	//TODO frissített forráskép
	Text gameOverText;

	void Start() {
		gameOverText = GetComponentInChildren <Text> ();
		gameOverText.text = "Game Over\nScore: " + PlayerPrefs.GetInt ("Score");
	}

	public void Retry()
	{
		SceneManager.LoadScene("Lenin");
	}
}
