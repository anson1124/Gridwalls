using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts;
using GameClient;

public class GameLogic : MonoBehaviour
{
    private readonly CellInstantiator cellInstantiator;

    public GameObject CellGroup;
    public GameObject CellPrefab;

    public int MapWidth = 8;
    public int MapHeight = 5;

    //private GameUnity _game;
    private Game _game;

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

        _game = new Game();
        StartCoroutine(NextTurn());
    }
    
    void Update () // Update is called once per frame
    {
    }

    IEnumerator NextTurn()
    {
        while (true)
        {
            Debug.Log("Next turn.");
            _game.NextTurn();
            yield return new WaitForSeconds(1);
        }
    }

}
