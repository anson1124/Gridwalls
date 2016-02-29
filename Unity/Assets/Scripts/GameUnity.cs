using UnityEngine;

namespace Assets.Scripts
{
    public class GameUnity
    {
        public void NextTurn()
        {
            Debug.Log("WOHO RANDOMIZING " + Random.Range(0f, 10f));
        }
    }
}