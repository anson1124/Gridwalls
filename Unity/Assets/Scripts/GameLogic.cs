using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class GameLogic : MonoBehaviour
{
    public GameObject CellGroup;
    public GameObject CellPrefab;

    public int MapWidth = 8;
    public int MapHeight = 5;

    private CellInstantiator cellInstantiator;

    public GameLogic()
    {
        cellInstantiator = new CellInstantiator();
    }

    // Use this for initialization
    void Start () {
        Debug.Log("------------------------------------------------- GAME START --------------------------------------------------");
        var config = new CellConfiguration
        {
            CellGroup = CellGroup,
            CellPrefab = CellPrefab,
            MapWidth = MapWidth,
            MapHeight = MapHeight
        };
        cellInstantiator.InstantiateCells(config);
        Debug.Log("------------------------------------------------- GAME START DONE --------------------------------------------------");
        //_game = new Game(MediatorUnity.Mediator, new Map(5, 4));
        //_game.Instantiate();

        //StartCoroutine(NextTurn());
    }

    IEnumerator NextTurn()
    {
        while (true)
        {
            //_game.NextTurn();
            yield return new WaitForSeconds(1);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
