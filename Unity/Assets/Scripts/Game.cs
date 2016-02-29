using UnityEngine;

namespace Assets.Scripts
{
    public class Game
    {
        public void NextTurn()
        {
            Debug.Log("WOHO RANDOMIZING " + Random.Range(0f, 10f));
        }
    }
}