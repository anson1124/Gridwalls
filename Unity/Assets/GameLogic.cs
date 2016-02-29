using UnityEngine;
using System.Collections;
using System;

public class GameLogic : MonoBehaviour
{
	public GameObject cellPrefab;
	public GameObject skeletonPrefab;

	public MediatorUnity MediatorUnity;

	// Use this for initialization
	void Start () {
		InstantiateCells(5, 4);
		Debug.Log ("Game started");

		//_game = new Game(MediatorUnity.Mediator, new Map(5, 4));
        //_game.Instantiate();

		StartCoroutine(NextTurn());
	}
	
	private void InstantiateCells(int width, int height)
	{
		float oddRowStartPositionForX = 0.33f;

		float xWidth = 0.69f;
		float zWidth = 0.60f;

		for (int z = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				float xPos = x * xWidth;

				Boolean isOddRow = z % 2 == 1;
				if (isOddRow) 
					xPos += oddRowStartPositionForX;

				float zPos = z * zWidth;
				float yPos = 0;

				Vector3 spawnPosition = new Vector3 (xPos, yPos, zPos);
				//Quaternion spawnRotation2 = Quaternion.identity;
				Quaternion spawnRotation = new Quaternion();
				Instantiate(cellPrefab, spawnPosition, Quaternion.Euler(90, 0 ,0));
			}
		}
	}

	// Update is called once per frame
	void Update () {
	}

	IEnumerator NextTurn() {
		while (true)
		{
			//_game.NextTurn();
			yield return new WaitForSeconds(1);
		}
	}

}
