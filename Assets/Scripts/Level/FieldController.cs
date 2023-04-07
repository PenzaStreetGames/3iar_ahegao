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
            CreateTiles();
        }

        private void CreateTiles()
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
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    var hitObject = hit.transform.gameObject;
                    Tile tile = hit.transform.gameObject.GetComponent<Tile>();
                    if (tile != null)
                    {
                        Debug.Log(hitObject.name);
                    }
                }
            }
        }
    }
}
