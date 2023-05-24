using System;
using System.Collections.Generic;
using Db;
using Db.Entity;
using JetBrains.Annotations;
using Level.EventQueue;
using Level.EventQueue.Events;
using Level.TileEntity;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace Level {
    public class FieldController : MonoBehaviour {
        public AudioSource soundSource;
        public AudioSource musicSource;
        public AudioClip clickSound;
        public AudioClip swapSound;
        public AudioClip releaseSound;
        public AudioClip fallSound;
        public AudioClip fillSound;
        public AudioClip createSound;

        public IntPair FieldSize;
        public GameObject tilePrefab;
        public GameObject movingTilePrefab;
        public float tileStep = 1f;

        public LevelController levelController;
        public LevelEventQueue levelEventQueue;
        public Tile chosenTile;
        public Tile[,] Tiles;

        public static FieldController Instance;

        static bool FirstTimeAfterStable = true;

        // Start is called before the first frame update
        void Start() {
            if (Instance == null)
                Instance = this;

            musicSource.Play();
        }

        // Update is called once per frame
        void Update() {
            if (levelEventQueue.IsFieldStable()) {
                if (FirstTimeAfterStable) {
                    SaveRepository.PersistSave(SaveEntity.MakeSaveFromData(Tiles));
                    FirstTimeAfterStable = false;
                }
                var combinations = FindAllCombinations();
                foreach (var combination in combinations) {
                    DeleteCombination(combination);
                }
                SaveRepository.PersistSave(SaveEntity.MakeSaveFromData(Tiles));
            }
        }

        public void GenerateFieldWithGuaranteedCombination(SaveEntity saveEntity) {
            var tryGenerateCounter = 0;
            do {
                Debug.Log("Regenerating field");
                ColorizeFromSave(saveEntity);
                tryGenerateCounter++;
                if (tryGenerateCounter == 3) {
                    Debug.LogError("The field regeneration limit has been reached.");
                    break;
                }
                Debug.Log(GetAllPossibleTurns().Count);
            } while (GetAllPossibleTurns().Count == 0);
        }

        public void Init(SaveEntity saveEntity) {
            var decodedLevel = saveEntity.GetDecodedFieldState();
            FieldSize = new IntPair(decodedLevel.GetLength(0), decodedLevel.GetLength(1));
            Tiles = new Tile[FieldSize.X, FieldSize.Y];
            CreateTiles(Tiles);
            GenerateFieldWithGuaranteedCombination(saveEntity);
        }

        void ColorizeFromSave(SaveEntity saveEntity) {
            ResetField(saveEntity);
            var colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow };

            var tilePersistMatrix = saveEntity.GetDecodedFieldState();
            // Debug.Log($"{tilePersistMatrix.GetLength(0)} {tilePersistMatrix.GetLength(1)}");
            var maxDelay = 0f;
            for (var i = 0; i < Tiles.GetLength(0); i++) {
                for (var j = 0; j < Tiles.GetLength(1); j++) {
                    var tileData = tilePersistMatrix[i, j];
                    var tile = Tiles[i, j];
                    var tileCreatingEvent = new TileCreatingEvent(tile, tileData);
                    var delay = tileCreatingEvent.GetDelay() * (i * Tiles.GetLength(0) + j + 1f);
                    if (delay > maxDelay)
                        maxDelay = delay;
                    levelEventQueue.Enqueue(tileCreatingEvent, delay);
                }
            }
        }

        void CreateTiles(Tile[,] tiles) {
            while (transform.childCount > 0) {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            var leftTopCorner = new Vector2(
                -(FieldSize.X / 2f) * tileStep,
                FieldSize.Y / 2f * tileStep
            );
            for (var i = 0; i < FieldSize.X; i++) {
                for (var j = 0; j < FieldSize.Y; j++) {
                    var point = new Vector3(
                        leftTopCorner.x + (j + 0.5f) * tileStep,
                        leftTopCorner.y - (i + 0.5f) * tileStep,
                        0
                    );
                    tiles[i, j] = CreateTile(i, j, point);
                }
            }
        }

        Tile CreateTile(int i, int j, Vector3 point) {
            var tileObject = Instantiate(tilePrefab, point, Quaternion.identity, transform);
            //var tileObject = Instantiate(tilePrefab, transform, true);
            tileObject.transform.parent = gameObject.transform;
            tileObject.name = $"Tile {i}-{j}";
            var tile = tileObject.GetComponent<Tile>();
            tile.fieldController = this;
            tile.position = new IntPair(i, j);
            tile.SetColor(TileColor.None);
            return tile;
        }

        public void ResetField(SaveEntity saveEntity) {
            var tileMatrix = saveEntity.GetDecodedFieldState();
            for (var i = 0; i < Tiles.GetLength(0); i++) {
                for (var j = 0; j < Tiles.GetLength(1); j++) {
                    var tileType = tileMatrix[i, j].TileType;
                    Tiles[i, j].SetTileType(tileType);
                    if (tileType == TileType.Open)
                        Tiles[i, j].SetColor(tileMatrix[i, j].TileColor);
                }
            }
        }

        [ItemCanBeNull]
        public HashSet<HashSet<IntPair>> GetAllPossibleTurns() {
            var res = new HashSet<HashSet<IntPair>>();

            foreach (var tile in Tiles) {
                res.UnionWith(tile.GetTurns(Tiles));
            }

            return res;
        }

        public HashSet<HashSet<Tile>> FindAllCombinations() {
            var res = new HashSet<HashSet<Tile>>();
            var affectedTiles = new HashSet<Tile>();
            for (var row = 0; row < Tiles.GetLength(0); row++) {
                for (var column = 0; column < Tiles.GetLength(1); column++) {
                    var tile = Tiles[row, column];
                    if (affectedTiles.Contains(tile))
                        continue;
                    if (tile.HaveCombinations(Tiles)) {
                        var combination = GetMaxCombinationWith(tile);
                        res.Add(combination);
                        affectedTiles.UnionWith(combination);
                    }
                }
            }
            return res;
        }

        public void HandleTileClick(Tile tile) {
            soundSource.PlayOneShot(clickSound);

            if (!levelEventQueue.IsFieldStable())
                return;
            Debug.Log($"Click {tile.gameObject.name}");
            if (chosenTile != null) {
                if (chosenTile.CanSwapWith(Tiles, tile)) {
                    soundSource.PlayOneShot(swapSound);
                    FirstTimeAfterStable = true;
                    SwapTileColors(chosenTile, tile);
                    DeletePossibleCombinationsWith(tile);
                    DeletePossibleCombinationsWith(chosenTile);
                    levelController.DecrementTurnCounter();

                    tile.SetViewState(TileViewState.Active);
                    chosenTile.SetViewState(TileViewState.Active);
                    chosenTile = null;
                    return;
                }
                chosenTile.SetViewState(TileViewState.Active);
            }
            tile.SetViewState(TileViewState.Selected);
            chosenTile = tile;
        }

        public void RandomFillEmptyTile(Tile tile) {
            var colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow };
            if (tile.tileType == TileType.Open && tile.tileColor == TileColor.None) {
                var colorIndex = Random.Range(0, colors.Length);
                tile.SetColor(colors[colorIndex]);
            }
        }

        public HashSet<Tile> GetPossibleCombinationsWith(Tile tile) {
            if (!tile.HaveCombinations(Tiles)) {
                return new HashSet<Tile>();
            }

            var res = new HashSet<Tile>();
            HashSet<Tile> horizontalCombination = new(), verticalCombination = new();
            int x1 = tile.position.X, y1 = tile.position.Y;
            int x2 = x1, y2 = y1;
            while (x2 >= 0 && Tiles[x2, y2].tileType == TileType.Open && tile.tileColor == Tiles[x2, y2].tileColor) {
                verticalCombination.Add(Tiles[x2, y2]);
                x2--;
            }

            (x2, y2) = (x1 + 1, y1);
            while (x2 < FieldSize.X && Tiles[x2, y2].tileType == TileType.Open &&
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
            while (y2 < FieldSize.Y && Tiles[x2, y2].tileType == TileType.Open &&
                   tile.tileColor == Tiles[x2, y2].tileColor) {
                horizontalCombination.Add(Tiles[x2, y2]);
                y2++;
            }

            if (horizontalCombination.Count >= 3) {
                res.UnionWith(horizontalCombination);
            }

            return res;
        }

        public HashSet<Tile> GetMaxCombinationWith(Tile tile) {
            var initCombination = GetPossibleCombinationsWith(tile);
            var res = initCombination;
            if (initCombination.Count == 0) {
                return new HashSet<Tile>();
            }
            foreach (var tileInInitCombination in initCombination) {
                var combination = GetPossibleCombinationsWith(tileInInitCombination);
                if (combination.Count > res.Count) {
                    res = combination;
                }
            }
            return res;
        }

        public void DeletePossibleCombinationsWith(Tile tile) {
            var combination = GetPossibleCombinationsWith(tile);
            if (combination.Count > 0) {
                var squashingEvent = new CombinationSquashingEvent(combination);
                levelEventQueue.Enqueue(squashingEvent, squashingEvent.Delay);
            }
        }

        public void DeleteCombination(HashSet<Tile> combination) {
            if (combination.Count > 0) {
                var squashingEvent = new CombinationSquashingEvent(combination);
                levelEventQueue.Enqueue(squashingEvent, squashingEvent.Delay);
            }
        }

        public static void SwapTileColors(Tile tile1, Tile tile2) {
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

        public void ShiftTileDown(Tile tile) {
            var (x, y) = (tile.position.X, tile.position.Y);
            if (tile.CanFallDown(Tiles)) {
                var tileUnder = Tiles[x + 1, y];
                SwapTileColors(tile, tileUnder);
            }
        }
    }
}
