using Db;
using Db.Entity;
using Level.TileEntity;
using UnityEngine;

namespace Level
{
    public class FieldController : MonoBehaviour
    {
        public Vector2 fieldSize;
        public GameObject tilePrefab;
        public float tileStep = 1f;

        public Tile chosenTile;
        public Tile[,] Tiles;

        // Start is called before the first frame update
        void Start()
        {
            Tiles = new Tile[(int)fieldSize.x, (int)fieldSize.y];
            CreateTiles();
            GenerateField();
            SaveRepository.InitDb();
        }

        // Update is called once per frame
        void Update()
        {
        }

        void CreateTiles()
        {
            var leftTopCorner = new Vector2(
                -(fieldSize.x / 2) * tileStep,
                fieldSize.y / 2 * tileStep
            );
            for (var i = 0; i < fieldSize.x; i++)
            {
                for (var j = 0; j < fieldSize.y; j++)
                {
                    var point = new Vector3(
                        leftTopCorner.x + (j + 0.5f) * tileStep,
                        leftTopCorner.y - (i + 0.5f) * tileStep,
                        0
                    );
                    Tiles[i, j] = CreateTile(i, j, point);
                }
            }
        }

        Tile CreateTile(int i, int j, Vector3 point)
        {
            var tileObject = Instantiate(tilePrefab, point, Quaternion.identity);
            tileObject.transform.parent = gameObject.transform;
            tileObject.name = $"Tile {i}-{j}";
            var tile = tileObject.GetComponent<Tile>();
            tile.fieldController = this;
            tile.position = new Vector2(i, j);
            tile.SetColor(TileColor.None);
            return tile;
        }

        void GenerateField()
        {
            var colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow };
            for (var i = 0; i < fieldSize.x; i++)
            {
                for (var j = 0; j < fieldSize.y; j++)
                {
                    var tile = Tiles[i, j];
                    tile.tileType = TileType.Open;
                    var colorIndex = (i + j + Random.Range(0, colors.Length)) % colors.Length;
                    tile.SetColor(colors[colorIndex]);
                    while (tile.HaveCombinations())
                    {
                        colorIndex = (colorIndex + 1) % colors.Length;
                        tile.SetColor(colors[colorIndex]);
                    }
                }
            }
        }

        // Функция для проверки наличия комбинации из трех и более одинаковых тайлов
        static bool CheckCombination(Tile[,] field)
        {
            var numRows = field.GetLength(0);
            var numColumns = field.GetLength(1);

            // Проверяем горизонтальные совпадения
            for (var i = 0; i < numRows; i++)
            {
                for (var j = 0; j < numColumns - 2; j++)
                {
                    if (field[i, j].GetColor() == TileColor.None ||
                        field[i, j].GetColor() != field[i, j + 1].GetColor() ||
                        field[i, j].GetColor() != field[i, j + 2].GetColor())
                    {
                        continue;
                    }

                    Debug.Log($"hor {i} {j} {field[i, j].GetColor()}");
                    return true;
                }
            }

            // Проверяем вертикальные совпадения
            for (var i = 0; i < numRows - 2; i++)
            {
                for (var j = 0; j < numColumns; j++)
                {
                    if (field[i, j].GetColor() == TileColor.None ||
                        field[i, j].GetColor() != field[i + 1, j].GetColor() ||
                        field[i, j].GetColor() != field[i + 2, j].GetColor())
                    {
                        continue;
                    }

                    Debug.Log($"vert {i} {j} {field[i, j].GetColor()}");
                    return true;
                }
            }

            return false;
        }

        public void HandleTileClick(Tile tile)
        {
            Debug.Log($"Click {tile.gameObject.name}");
            if (chosenTile != null)
            {
                if (chosenTile.CanSwapWith(tile))
                {
                    SwapTiles(chosenTile, tile);
                    tile.SetViewState(TileViewState.Active);
                    chosenTile.SetViewState(TileViewState.Active);
                    chosenTile = null;
                    var save = Save.MakeSaveFromData(Tiles);
                    SaveRepository.PersistSave(save);
                    return;
                }

                chosenTile.SetViewState(TileViewState.Active);
            }

            tile.SetViewState(TileViewState.Selected);
            chosenTile = tile;
        }

        static void SwapTiles(Tile tile1, Tile tile2)
        {
            (tile1.tileColor, tile2.tileColor) = (tile2.tileColor, tile1.tileColor);
        }

        public static void HandleTileMouseEnter(Tile tile)
        {
            if (tile.tileViewState == TileViewState.Active)
            {
                tile.SetViewState(TileViewState.Hover);
            }

            Debug.Log($"Cursor enter {tile.gameObject.name}");
        }

        public static void HandleTileMouseExit(Tile tile)
        {
            if (tile.tileViewState == TileViewState.Hover)
            {
                tile.SetViewState(TileViewState.Active);
            }

            Debug.Log($"Cursor exit {tile.gameObject.name}");
        }
    }
}
