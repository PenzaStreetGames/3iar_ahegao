using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    public class FieldController : MonoBehaviour
    {
        public Tile[,] Tiles;
        public Vector2 fieldSize;
        public GameObject tilePrefab;
        public float tileStep = 1f;
        public Tile chosenTile;
        
        // Start is called before the first frame update
        void Start()
        {
            Tiles = new Tile[(int)fieldSize.x, (int)fieldSize.y];
            CreateTiles();
            GenerateField();
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
                    Tiles[i, j] = CreateTile(i, j, point);
                }
            }
        }

        private Tile CreateTile(int i, int j, Vector3 point)
        {
            var tileObject = Instantiate(tilePrefab, point, Quaternion.identity);
            tileObject.transform.parent = gameObject.transform;
            tileObject.name = $"Tile {i}-{j}";
            var tile = tileObject.GetComponent<Tile>();
            tile.fieldController = this;
            return tile;
        }

        private void GenerateField()
        {
            var colors = Enum.GetValues(typeof(TileColor));
            for (int i = 0; i < fieldSize.x; i++)
            {
                for (int j = 0; j < fieldSize.y; j++)
                {
                    var tile = Tiles[i, j];
                    tile.tileType = TileType.Open;
                    TileColor color = (TileColor) colors.GetValue(Random.Range(0, colors.Length));
                    tile.SetColor(color);
                }
            }
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }

        public void HandleTileClick(Tile tile)
        {
            Debug.Log($"Click {tile.gameObject.name}");
            if (chosenTile != null)
            {
                chosenTile.SetViewState(TileViewState.Active);
            }
            tile.SetViewState(TileViewState.Selected);
            chosenTile = tile;
        }

        public void HandleTileMouseEnter(Tile tile)
        {
            if (tile.tileViewState == TileViewState.Active)
                tile.SetViewState(TileViewState.Hover);
            Debug.Log($"Cursor enter {tile.gameObject.name}");
        }

        public void HandleTileMouseExit(Tile tile)
        {
            if (tile.tileViewState == TileViewState.Hover)
                tile.SetViewState(TileViewState.Active);
            Debug.Log($"Cursor exit {tile.gameObject.name}");
        }
    }
}
