using UnityEngine;

namespace Assets.Scripts
{
    public class CellInstantiator
    {
        public void InstantiateCells(CellConfiguration config)
        {
            float oddRowStartPositionForX = 0.33f;

            float xWidth = 0.69f;
            float zWidth = 0.60f;

            for (int z = 0; z < config.MapHeight; z++)
            {
                for (int x = 0; x < config.MapWidth; x++)
                {
                    float xPos = x * xWidth;

                    bool isOddRow = z % 2 == 1;
                    if (isOddRow)
                        xPos += oddRowStartPositionForX;

                    float zPos = z * zWidth;
                    float yPos = 0;

                    Vector3 spawnPosition = new Vector3(xPos, yPos, zPos);
                    //Quaternion spawnRotation2 = Quaternion.identity;
                    Debug.Log("Instantiating CellPrefab " + config.CellPrefab + " " + x + ", " + z + " at position " + xPos + " - " + yPos + " - " + zPos);
                    GameObject clone = UnityEngine.Object.Instantiate(config.CellPrefab, spawnPosition, Quaternion.Euler(90, 0, 0)) as GameObject;
                    clone.transform.parent = config.CellGroup.transform; // Puts the clone as a child of a parent game object
                }
            }
        }

    }
}