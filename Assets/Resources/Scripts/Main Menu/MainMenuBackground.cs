using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuBackground : MonoBehaviour {

	public HexagonField hexField;
	private int fieldWidth;
	private int fieldHeight;
	// Use this for initialization
	void Start () {
		//hexField = transform.parent.transform.parent.GetComponent<HexagonField> ();	
		float rotatorDelay = 0.25f;
		fieldWidth = (int)hexField.Field.width;
		fieldHeight = (int)hexField.Field.height;
		InvokeRepeating ("rotateRandom", rotatorDelay, rotatorDelay);
	}

	void rotateRandom() {
		int randomX = Random.Range (-3, 4);
		int randomY = Random.Range (-3, 4);
		int direction = Random.Range (0, 2);
		GridPoint targetGridPoint = new GridPoint (randomX, randomY);
		if (hexField.inRange (targetGridPoint)) {
			HexagonBase targetHex = hexField.getHexagon (targetGridPoint);
			if (direction == 0) {
				targetHex.rotateClockwise ();
			} else {
				targetHex.rotateCounterClockwise ();
			}
		}
	}
	// Update is called once per frame
	void Update () {
		
	}

	public void StartGame()
	{
		SceneManager.LoadScene("Lenin");
	}
}
