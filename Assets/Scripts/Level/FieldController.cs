using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class FieldController : MonoBehaviour
    {
        public Tile[,] Tiles;
        public Vector2 fieldSize;
        public GameObject tilePrefab;
        public float tileStep = 1f;
        
        // Start is called before the first frame update
        void Start()
        {
            Tiles = new Tile[(int)fieldSize.x, (int)fieldSize.y];
            createTiles();
        }

        void createTiles()
        {
            Vector2 leftTopCorner = new Vector2(
                -(fieldSize.x / 2) * tileStep,
                -(fieldSize.y / 2) * tileStep
            );
            for (var i = 0; i < fieldSize.x; i++)
            {
                for (var j = 0; j < fieldSize.y; j++)
                {
                    Vector3 point = new Vector3(
                        leftTopCorner.x + (i + 0.5f) * tileStep,
                        leftTopCorner.y + (j + 0.5f) * tileStep,
                        0
                    );
                    GameObject tileObject = Instantiate(tilePrefab, point, Quaternion.identity);
                    tileObject.transform.parent = gameObject.transform;
                    tileObject.name = $"Tile {i}-{j}";
                    Tiles[i, j] = tileObject.GetComponent<Tile>();
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
