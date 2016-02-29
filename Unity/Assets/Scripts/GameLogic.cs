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

    private Game _game;

    public GameLogic()
    {
        cellInstantiator = new CellInstantiator();
    }

    // Use this for initialization
    void Start () {
        Debug.Log("------------------------------------------------- GAME START --------------------------------------------------");
        _game = new Game();
        cellInstantiator.InstantiateCells(config);
        Debug.Log("------------------------------------------------- GAME START DONE --------------------------------------------------");

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
