using System.Collections.Generic;
using Db;
using Db.Entity;
using Level.TileEntity;
using Unity.VisualScripting;
using UnityEngine;

namespace Level {
    public class FieldController : MonoBehaviour {
        public Vector2 fieldSize;
        public GameObject tilePrefab;
        public float tileStep = 1f;

        public LevelController levelController;
        public Tile chosenTile;
        public Tile[,] Tiles;

        // Start is called before the first frame update
        void Start() {
        }

        // Update is called once per frame
        void Update() {
        }

        public void Init(int xSize, int ySize) {
            fieldSize = new Vector2(xSize, ySize);
            Tiles = new Tile[(int)fieldSize.x, (int)fieldSize.y];
            CreateTiles();
            GenerateField();
            SaveRepository.InitDb();
        }

        void CreateTiles() {
            var leftTopCorner = new Vector2(
                -(fieldSize.x / 2) * tileStep,
                fieldSize.y / 2 * tileStep
            );
            for (var i = 0; i < fieldSize.x; i++) {
                for (var j = 0; j < fieldSize.y; j++) {
                    var point = new Vector3(
                        leftTopCorner.x + (j + 0.5f) * tileStep,
                        leftTopCorner.y - (i + 0.5f) * tileStep,
                        0
                    );
                    Tiles[i, j] = CreateTile(i, j, point);
                }
            }
        }

        Tile CreateTile(int i, int j, Vector3 point) {
            var tileObject = Instantiate(tilePrefab, point, Quaternion.identity);
            tileObject.transform.parent = gameObject.transform;
            tileObject.name = $"Tile {i}-{j}";
            var tile = tileObject.GetComponent<Tile>();
            tile.fieldController = this;
            tile.position = new Vector2(i, j);
            tile.SetColor(TileColor.None);
            return tile;
        }

        void GenerateField() {
            Random.InitState(42); // TODO: Delete random seed
            var colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow };
            for (var i = 0; i < fieldSize.x; i++) {
                for (var j = 0; j < fieldSize.y; j++) {
                    var tile = Tiles[i, j];
                    tile.tileType = TileType.Open;
                    var colorIndex = (i + j + Random.Range(0, colors.Length)) % colors.Length;
                    tile.SetColor(colors[colorIndex]);
                    while (tile.HaveCombinations()) {
                        Debug.Log($"({i}, {j}): have combinations");
                        colorIndex = (colorIndex + 1) % colors.Length;
                        tile.SetColor(colors[colorIndex]);
                    }
                }
            }
        }

        // Функция для проверки наличия комбинации из трех и более одинаковых тайлов
        static bool CheckCombination(Tile[,] field) {
            var numRows = field.GetLength(0);
            var numColumns = field.GetLength(1);

            // Проверяем горизонтальные совпадения
            for (var i = 0; i < numRows; i++) {
                for (var j = 0; j < numColumns - 2; j++) {
                    if (field[i, j].GetColor() == TileColor.None ||
                        field[i, j].GetColor() != field[i, j + 1].GetColor() ||
                        field[i, j].GetColor() != field[i, j + 2].GetColor()) {
                        continue;
                    }

                    return true;
                }
            }

            // Проверяем вертикальные совпадения
            for (var i = 0; i < numRows - 2; i++) {
                for (var j = 0; j < numColumns; j++) {
                    if (field[i, j].GetColor() == TileColor.None ||
                        field[i, j].GetColor() != field[i + 1, j].GetColor() ||
                        field[i, j].GetColor() != field[i + 2, j].GetColor()) {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        //TODO: Проверка, что может сделать комбинацию у ячейки с соседними ячейками после КАЖДОГО ХОДА
        //TODO: прокинуть в проверку окончания игры
        public Tile FindTileWithCombinations() {
            for (var row = 0; row < Tiles.GetLength(0); row++)
            for (var column = 0; column < Tiles.GetLength(1); column++) {
                var tile = Tiles[row, column];
                if (tile.HaveCombinations()) {
                    return tile;
                }
            }
            return null;
        }


        public void HandleTileClick(Tile tile) {
            Debug.Log($"Click {tile.gameObject.name}");
            if (chosenTile != null) {
                if (chosenTile.CanSwapWith(tile)) {
                    SwapTileColors(chosenTile, tile);
                    DeletePossibleCombinationsWith(tile);
                    DeletePossibleCombinationsWith(chosenTile);
                    CascadeFall();

                    tile.SetViewState(TileViewState.Active);
                    chosenTile.SetViewState(TileViewState.Active);
                    chosenTile = null;
                    levelController.MakeTurn();

                    SaveRepository.PersistSave(Save.MakeSaveFromData(Tiles));
                    return;
                }
                chosenTile.SetViewState(TileViewState.Active);
            }
            tile.SetViewState(TileViewState.Selected);
            chosenTile = tile;
        }

        public HashSet<Tile> GetPossibleCombinationsWith(Tile tile) {
            if (!tile.HaveCombinations()) {
                return new HashSet<Tile>();
            }

            var res = new HashSet<Tile>();
            HashSet<Tile> horizontalCombination = new HashSet<Tile>(), verticalCombination = new HashSet<Tile>();
            int x1 = (int)tile.position.x, y1 = (int)tile.position.y;
            int x2 = x1, y2 = y1;
            while (x2 >= 0 && Tiles[x2, y2].tileType == TileType.Open && tile.tileColor == Tiles[x2, y2].tileColor) {
                verticalCombination.Add(Tiles[x2, y2]);
                x2--;
            }

            (x2, y2) = (x1 + 1, y1);
            while (x2 < fieldSize.x && Tiles[x2, y2].tileType == TileType.Open &&
                   tile.tileColor == Tiles[x2, y2].tileColor) {
                verticalCombination.Add(Tiles[x2, y2]);
                x2++;
            }

            if (verticalCombination.Count >= 3) {
                res.UnionWith(verticalCombination);
            }

            (x2, y2) = (x1, y1);
            while (y2 >= 0 && Tiles[x2, y2].tileType == TileType.Open && tile.tileColor == Tiles[x2, y2].tileColor) {
                horizontalCombination.Add(Tiles[x2, y2]);
                y2--;
            }

            (x2, y2) = (x1, y1);
            while (y2 < fieldSize.y && Tiles[x2, y2].tileType == TileType.Open &&
                   tile.tileColor == Tiles[x2, y2].tileColor) {
                horizontalCombination.Add(Tiles[x2, y2]);
                y2++;
            }

            if (horizontalCombination.Count >= 3) {
                res.UnionWith(horizontalCombination);
            }

            return res;
        }
        public HashSet<Tile> DeletePossibleCombinationsWith(Tile tile) {
            var affectedTiles = GetPossibleCombinationsWith(tile);
            foreach (var affectedTile in affectedTiles) {
                Debug.Log($"Delete Tile ({affectedTile.position.x}, {affectedTile.position.y})");
                affectedTile.SetColor(TileColor.None);
            }

            return affectedTiles;
        }

        public void SwapTileColors(Tile tile1, Tile tile2) {
            var color1 = tile1.tileColor;
            var color2 = tile2.tileColor;
            tile1.SetColor(color2);
            tile2.SetColor(color1);
        }

        public static void HandleTileMouseEnter(Tile tile) {
            if (tile.tileViewState == TileViewState.Active) {
                tile.SetViewState(TileViewState.Hover);
            }
        }

        public static void HandleTileMouseExit(Tile tile) {
            if (tile.tileViewState == TileViewState.Hover) {
                tile.SetViewState(TileViewState.Active);
            }
        }

        public void CascadeFall() {
            for (int i = 0; i < Tiles.GetLength(0); i++) {
                if (!IsFallOver()) {
                    ShiftFieldDown();
                }
                else {
                    break;
                }
            }
        }

        public bool IsFallOver() {
            bool res = true;
            for (int i = 0; i < Tiles.GetLength(0); i++) {
                for (int j = 0; j < Tiles.GetLength(1); j++) {
                    if (Tiles[i, j].CanFallDown())
                        res = false;
                }
            }
            return res;
        }
        public void ShiftFieldDown() {
            for (int i = Tiles.GetLength(0) - 1; i >= 0; i--) {
                for (int j = 0; j < Tiles.GetLength(1); j++) {
                    var tile = Tiles[i, j];
                    if (tile.CanFallDown()) {
                        var underTile = Tiles[i + 1, j];
                        SwapTileColors(tile, underTile);
                    }
                }
            }
        }
    }
}
